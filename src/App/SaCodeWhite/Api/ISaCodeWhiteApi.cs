using SaCodeWhite.Shared.Models;
using Refit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SaCodeWhite.Shared.Models.AmbulanceService;
using SaCodeWhite.Shared.Models.EmergencyDepartment;

namespace SaCodeWhite.Api
{
    public interface ISaCodeWhiteApi
    {
        [Get("/Metadata/Hospitals")]
        Task<List<Hospital>> GetHospitalsAsync([Header(Constants.AuthHeader)] string code, CancellationToken cancellationToken);

        [Get("/Metadata/TriageCategories")]
        Task<List<TriageCategory>> GetTriageCategoriesAsync([Header(Constants.AuthHeader)] string code, CancellationToken cancellationToken);

        [Get("/AmbulanceService/Dashboards")]
        Task<List<AmbulanceServiceDashboard>> GetAmbulanceServiceDashboardsAsync([Header(Constants.AuthHeader)] string code, CancellationToken cancellationToken);

        [Get("/EmergencyDepartment/Dashboards")]
        Task<List<EmergencyDepartmentDashboard>> GetEmergencyDepartmentDashboardsAsync([Header(Constants.AuthHeader)] string code, CancellationToken cancellationToken);

        [Post("/Notifications/Installations")]
        Task RegisterDeviceAsync([Header(Constants.AuthHeader)] string code, [Body] DeviceInstallation deviceInstallation, CancellationToken cancellationToken);

        [Delete("/Notifications/Installations/{platform}/{installationId}")]
        Task DeleteDeviceAsync([Header(Constants.AuthHeader)] string code, PlatformType platform, string installationId, CancellationToken cancellationToken);
    }
}