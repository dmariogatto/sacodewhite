using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SaCodeWhite.Functions.Models;
using SaCodeWhite.Functions.Repositories;
using SaCodeWhite.Functions.Services;
using SaCodeWhite.Shared.Models;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Functions
{
    public class Notifications
    {
        private readonly INotificationHubService _notificationService;

        private readonly ITableRepository<DeviceInstallationEntity> _deviceInstallationRepo;

        public Notifications(
            INotificationHubService notificationService,
            ITableRepository<DeviceInstallationEntity> deviceInstallationRepo)
        {
            _notificationService = notificationService;

            _deviceInstallationRepo = deviceInstallationRepo;
        }

        [FunctionName("NotificationsRegister")]
        public async Task<IActionResult> RegisterDevice(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Notifications/Installations")] HttpRequest req,
            ILogger log,
            CancellationToken ct)
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();
            var deviceInstallation = JsonConvert.DeserializeObject<DeviceInstallation>(content);

            await _deviceInstallationRepo.CreateIfNotExistsAsync(CancellationToken.None);

            var entityTask = _deviceInstallationRepo.GetEntityAsync(deviceInstallation.Platform.ToString(), deviceInstallation.InstallationId, CancellationToken.None);
            var successTask = _notificationService.CreateOrUpdateInstallationAsync(deviceInstallation, CancellationToken.None);

            await Task.WhenAll(entityTask, successTask);

            if (!successTask.Result)
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);

            var entity = entityTask.Result ?? new DeviceInstallationEntity(deviceInstallation.Platform, deviceInstallation.InstallationId);
            entity.LastSeenUtc = DateTime.UtcNow;

            await _deviceInstallationRepo.InsertOrReplaceAsync(entity, CancellationToken.None);

            return new OkResult();
        }

        [FunctionName("NotificationsDelete")]
        public async Task<IActionResult> DeleteDevice(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Notifications/Installations/{platform}/{installationId}")] HttpRequest req,
            string platform,
            string installationId,
            ILogger log,
            CancellationToken ct)
        {
            var entity = await _deviceInstallationRepo.GetEntityAsync(platform, installationId, ct);

            var success = !string.IsNullOrEmpty(entity.RowKey)
                ? await _notificationService.DeleteInstallationByIdAsync(installationId, CancellationToken.None)
                : false;

            if (!success)
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);

            await _deviceInstallationRepo.DeleteAsync(entity, CancellationToken.None);

            return new OkResult();
        }
    }
}
