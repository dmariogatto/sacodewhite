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
    public class EmergencyDepartment
    {
        private readonly IEmergencyDepartmentService _emergencyDepartmentService;
        private readonly IBlobService _blobService;

        public EmergencyDepartment(
            IEmergencyDepartmentService emergencyDepartmentService,
            IBlobService blobService)
        {
            _emergencyDepartmentService = emergencyDepartmentService;
            _blobService = blobService;
        }

        [FunctionName("EmergencyDepartmentDashboards")]
        public async Task<IActionResult> GetDashboards(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "EmergencyDepartment/Dashboards")] HttpRequest req,
            ILogger log,
            CancellationToken ct)
        {
            var latestFilePath = Path.Combine(Constants.EmergencyDepartment, Constants.LatestJson);
            var versionStamp = await _blobService.DeserialiseAsync<VersionStamp>(latestFilePath, ct);

            if (versionStamp?.UpdatedUtc > DateTime.MinValue)
            {
                var dataFilePath = Path.Combine(Constants.EmergencyDepartment, Constants.Data, versionStamp.JsonFileName);
                var blobStream = await _blobService.OpenReadAsync(dataFilePath, ct);
                if (blobStream != null && blobStream != Stream.Null)
                    return new FileStreamResult(blobStream, Constants.JsonContentType);
            }

            return await RunAsync(_emergencyDepartmentService.GetDashboardsAsync, log, ct);
        }

        [FunctionName("EmergencyDepartmentStatuses")]
        public Task<IActionResult> GetStatuses(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "EmergencyDepartment/Statuses")] HttpRequest req,
            ILogger log,
            CancellationToken ct)
        {
            return RunAsync(_emergencyDepartmentService.GetStatusesAsync, log, ct);
        }

        [FunctionName("EmergencyDepartmentDepartures")]
        public Task<IActionResult> GetDepartures(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "EmergencyDepartment/Departures")] HttpRequest req,
            ILogger log,
            CancellationToken ct)
        {
            return RunAsync(_emergencyDepartmentService.GetDeparturesLast1HrAsync, log, ct);
        }

        [FunctionName("EmergencyDepartmentTriageCategories")]
        public Task<IActionResult> GetTriageCategories(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "EmergencyDepartment/TriageCategories")] HttpRequest req,
            ILogger log,
            CancellationToken ct)
        {
            return RunAsync(_emergencyDepartmentService.GetTriageCategoriesAsync, log, ct);
        }

        [FunctionName("EmergencyDepartmentWaitingTimes")]
        public Task<IActionResult> GetWaitingTimes(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "EmergencyDepartment/WaitingTimes")] HttpRequest req,
            ILogger log,
            CancellationToken ct)
        {
            return RunAsync(_emergencyDepartmentService.GetWaitingTimesAsync, log, ct);
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
