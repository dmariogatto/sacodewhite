using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SaCodeWhite.Api.Services;
using SaCodeWhite.Functions.Models;
using SaCodeWhite.Functions.Services;
using SaCodeWhite.Shared;
using SaCodeWhite.Shared.Models;
using SaCodeWhite.Shared.Models.AmbulanceService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Functions
{
    public class SyncData
    {
        private readonly IAmbulanceService _ambulanceService;
        private readonly IEmergencyDepartmentService _emergencyDepartmentService;
        private readonly IHospitalService _hospitalService;
        private readonly INotificationHubService _notificationHubService;
        private readonly IBlobService _blobService;

        public SyncData(
            IAmbulanceService ambulanceService,
            IEmergencyDepartmentService emergencyDepartmentService,
            IHospitalService hospitalService,
            INotificationHubService notificationHubService,
            IBlobService blobService)
        {
            _ambulanceService = ambulanceService;
            _emergencyDepartmentService = emergencyDepartmentService;
            _hospitalService = hospitalService;
            _notificationHubService = notificationHubService;
            _blobService = blobService;
        }

        [FunctionName(nameof(SyncAmbulanceService))]
        public async Task SyncAmbulanceService(
            [TimerTrigger("45 */5 * * * *")] TimerInfo myTimer,
            ILogger log,
            CancellationToken ct)
        {
            var ambulanceData = await _ambulanceService.GetDashboardsAsync(ct);
            ambulanceData = ambulanceData
                ?.Where(i => i.UpdatedUtc > DateTime.MinValue)
                ?.ToList();

            if (ambulanceData?.Any() == true)
            {
                var versionStamp = new VersionStamp() { UpdatedUtc = ambulanceData.Max(i => i.UpdatedUtc) };

                var latestFilePath = Path.Combine(Constants.Ambulance, Constants.LatestJson);
                var previousVersionStamp = await _blobService.DeserialiseAsync<VersionStamp>(latestFilePath, ct);

                if (!versionStamp.Equals(previousVersionStamp))
                {
                    var dataFilePath = Path.Combine(Constants.Ambulance, Constants.Data, versionStamp.JsonFileName);

                    await _blobService.SerialiseAsync(ambulanceData, dataFilePath, ct);
                    await _blobService.SerialiseAsync(versionStamp, latestFilePath, ct);

                    if (previousVersionStamp?.UpdatedUtc > DateTime.MinValue)
                    {
                        var previousDataFilePath = Path.Combine(Constants.Ambulance, Constants.Data, previousVersionStamp.JsonFileName);
                        var previousData = await _blobService.DeserialiseAsync<List<AmbulanceServiceDashboard>>(previousDataFilePath, ct);
                        if (previousData?.Any() == true)
                            await SendCodeWhiteNotificationsAsync(previousData, ambulanceData, ct);
                    }
                }
            }
        }

        [FunctionName(nameof(SyncEmergencyDepartment))]
        public async Task SyncEmergencyDepartment(
            [TimerTrigger("45 */10 * * * *")] TimerInfo myTimer,
            ILogger log,
            CancellationToken ct)
        {
            var edEtl = await _emergencyDepartmentService.GetEtlAsync(ct);

            if (edEtl.timestampUtc > DateTime.MinValue)
            {
                var edData = await _emergencyDepartmentService.GetDashboardsAsync(ct);
                edData = edData
                    ?.Where(i => i.UpdatedUtc > DateTime.MinValue)
                    ?.ToList();

                if (edData?.Any() == true)
                {
                    var versionStamp = new VersionStamp() { Id = edEtl.id, UpdatedUtc = edEtl.timestampUtc };

                    var latestFilePath = Path.Combine(Constants.EmergencyDepartment, Constants.LatestJson);
                    var previousVersionStamp = await _blobService.DeserialiseAsync<VersionStamp>(latestFilePath, ct);

                    if (!versionStamp.Equals(previousVersionStamp))
                    {
                        var dataFilePath = Path.Combine(Constants.EmergencyDepartment, Constants.Data, versionStamp.JsonFileName);

                        await _blobService.SerialiseAsync(edData, dataFilePath, ct);
                        await _blobService.SerialiseAsync(versionStamp, latestFilePath, ct);
                    }
                }
            }
        }

        [FunctionName(nameof(DeleteOldDataFiles))]
        public async Task DeleteOldDataFiles(
            [TimerTrigger("0 30 15 * * *")] TimerInfo myTimer,
            ILogger log,
            CancellationToken ct)
        {
            const int batchSize = 32;

            var blobFiles = await _blobService.GetBlobFilesAsync(ct);
            var toDelete = blobFiles
                .Where(i => i.ModifiedUtc < DateTime.UtcNow.AddHours(-24))
                .ToList();

            log.LogInformation($"Found {toDelete.Count} files to delete");

            for (var i = 0; i < toDelete.Count; i += batchSize)
            {
                var tasks = toDelete.Skip(i).Take(batchSize)
                    .Select(i => _blobService.DeleteAsync(i.Name, ct))
                    .ToList();

                await Task.WhenAll(tasks);

                log.LogInformation($"Deleted {i + batchSize} of {toDelete.Count} files");
            }

            log.LogInformation($"Deleted all old files");
        }

        private async Task SendCodeWhiteNotificationsAsync(IList<AmbulanceServiceDashboard> previousData, IList<AmbulanceServiceDashboard> data, CancellationToken ct)
        {
            var notify =
                (from d in data
                 join pd in previousData on d.HospitalCode equals pd.HospitalCode
                 where
                    d.GetAlertStatus().alertStatus is AlertStatusType.White &&
                    pd.GetAlertStatus().alertStatus is not AlertStatusType.White
                 select true).FirstOrDefault();

            if (notify)
            {
                var toNotify =
                    (from d in data
                     let status = d.GetAlertStatus()
                     where status.alertStatus is AlertStatusType.White
                     select new
                     {
                         d.HospitalCode,
                         OccupiedCapacity = status.occupiedCapacity * 100d,
                     }).ToList();

                var bodyBuilder = new StringBuilder();

                for (var i = 0; i < toNotify.Count; i++)
                {
                    var item = toNotify[i];

                    bodyBuilder.Append($"{item.HospitalCode} - {item.OccupiedCapacity:0.##}% of total capacity");
                    if (i < toNotify.Count - 1)
                        bodyBuilder.Append("\\n");
                }

                var request = new NotificationRequest()
                {
                    Title = Shared.Localisation.Resources.CodeWhite,
                    Body = bodyBuilder.ToString(),
                    AppleThreadId = Shared.Constants.NotificationKeys.CodeWhiteTag,
                    AndroidData = new Dictionary<string, string>(),
                    Tags = new[] { Shared.Constants.NotificationKeys.CodeWhiteTag }
                };

                await _notificationHubService.RequestNotificationsAsync(new [] { request }, ct);
            }
        }
    }
}
