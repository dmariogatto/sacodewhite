using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SaCodeWhite.Api.Services;
using SaCodeWhite.Shared.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Functions
{
    public class Metadata
    {
        private readonly IHospitalService _hospitalService;
        private readonly ITriageCategoryService _triageService;

        public Metadata(
            IHospitalService hospitalService,
            ITriageCategoryService triageService)
        {
            _hospitalService = hospitalService;
            _triageService = triageService;
        }

        [FunctionName("Hospitals")]
        public Task<IList<Hospital>> GetHospitals(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Metadata/Hospitals")] HttpRequest req,
            ILogger log,
            CancellationToken ct)
        {
            return _hospitalService.GetHospitalsAsync(ct);
        }

        [FunctionName("TriageCategories")]
        public Task<IList<TriageCategory>> GetTriageCategories(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Metadata/TriageCategories")] HttpRequest req,
            ILogger log,
            CancellationToken ct)
        {
            return _triageService.GetTriageCategories(ct);
        }
    }
}
