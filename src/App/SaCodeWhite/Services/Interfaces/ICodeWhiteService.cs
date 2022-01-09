using SaCodeWhite.Shared.Models;
using SaCodeWhite.Shared.Models.AmbulanceService;
using SaCodeWhite.Shared.Models.EmergencyDepartment;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Services
{
    public interface ICodeWhiteService
    {
        Task<IList<Hospital>> GetHospitalsAsync(CancellationToken cancellationToken);
        Task<IList<TriageCategory>> GetTriageCategoriesAsync(CancellationToken cancellationToken);

        Task<IList<AmbulanceServiceDashboard>> GetAmbulanceServiceDashboardsAsync(CancellationToken cancellationToken);
        Task<IList<EmergencyDepartmentDashboard>> GetEmergencyDepartmentDashboardsAsync(CancellationToken cancellationToken);

        Task<Hospital> GetHospitalAsync(string code, CancellationToken cancellationToken);
        Task<AmbulanceServiceDashboard> GetAmbulanceServiceDashboardAsync(string code, CancellationToken cancellationToken);
        Task<EmergencyDepartmentDashboard> GetEmergencyDepartmentDashboardAsync(string code, CancellationToken cancellationToken);
    }
}