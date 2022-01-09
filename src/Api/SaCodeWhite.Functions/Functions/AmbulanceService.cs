using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SaCodeWhite.Api.Services;
using SaCodeWhite.Functions.Models;
using SaCodeWhite.Functions.Services;
using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Functions
{
    public class AmbulanceService
    {
        private readonly IAmbulanceService _ambulanceService;
        private readonly IBlobService _blobService;

        public AmbulanceService(
            IAmbulanceService ambulanceService,
            IBlobService blobService)
        {
            _ambulanceService = ambulanceService;
            _blobService = blobService;
        }

        [FunctionName("AmbulanceServiceDashboards")]
        public async Task<IActionResult> GetDashboards(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "AmbulanceService/Dashboards")] HttpRequest req,
            ILogger log,
            CancellationToken ct)
        {
            var latestFilePath = Path.Combine(Constants.Ambulance, Constants.LatestJson);
            var versionStamp = await _blobService.DeserialiseAsync<VersionStamp>(latestFilePath, ct);

            if (versionStamp?.UpdatedUtc > DateTime.MinValue)
            {
                var dataFilePath = Path.Combine(Constants.Ambulance, Constants.Data, versionStamp.JsonFileName);
                var blobStream = await _blobService.OpenReadAsync(dataFilePath, ct);
                if (blobStream != null && blobStream != Stream.Null)
                    return new FileStreamResult(blobStream, Constants.JsonContentType);
            }

            return await RunAsync(_ambulanceService.GetDashboardsAsync, log, ct);
        }

        [FunctionName("AmbulanceServiceStatuses")]
        public Task<IActionResult> GetStatuses(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "AmbulanceService/Statuses")] HttpRequest req,
            ILogger log,
            CancellationToken ct)
        {
            return RunAsync(_ambulanceService.GetStatusesAsync, log, ct);
        }

        [FunctionName("AmbulanceServiceTriageCategories")]
        public Task<IActionResult> GetTriageCategories(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "AmbulanceService/TriageCategories")] HttpRequest req,
            ILogger log,
            CancellationToken ct)
        {
            return RunAsync(_ambulanceService.GetTriageCategoriesAsync, log, ct);
        }

        [FunctionName("AmbulanceServiceInpatientBedStatuses")]
        public Task<IActionResult> GetInpatientBedStatuses(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "AmbulanceService/InpatientBedStatuses")] HttpRequest req,
            ILogger log,
            CancellationToken ct)
        {
            return RunAsync(_ambulanceService.GetInpatientBedStatusesAsync, log, ct);
        }

        [FunctionName("AmbulanceServiceSpecialityBedStatuses")]
        public Task<IActionResult> GetSpecialityBedStatuses(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "AmbulanceService/SpecialityBedStatuses")] HttpRequest req,
            ILogger log,
            CancellationToken ct)
        {
            return RunAsync(_ambulanceService.GetSpecialityBedStatusesAsync, log, ct);
        }

        private static async Task<IActionResult> RunAsync<T>(Func<CancellationToken, Task<T>> func, ILogger log, CancellationToken ct, [CallerMemberName] string caller = "")
        {
            try
            {
                var result = await func(ct);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, caller);
            }

            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}
