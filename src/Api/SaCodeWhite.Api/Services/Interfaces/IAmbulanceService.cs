using SaCodeWhite.Shared.Models.AmbulanceService;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Api.Services
{
    public interface IAmbulanceService
    {
        Task<IList<AmbulanceServiceDashboard>> GetDashboardsAsync(CancellationToken cancellationToken);

        Task<IList<AmbulanceStatus>> GetStatusesAsync(CancellationToken cancellationToken);
        Task<IList<AmbulanceTriageCategory>> GetTriageCategoriesAsync(CancellationToken cancellationToken);
        Task<IList<AmbulanceInpatientBedStatus>> GetInpatientBedStatusesAsync(CancellationToken cancellationToken);
        Task<IList<AmbulanceSpecialityBedStatus>> GetSpecialityBedStatusesAsync(CancellationToken cancellationToken);
    }
}
