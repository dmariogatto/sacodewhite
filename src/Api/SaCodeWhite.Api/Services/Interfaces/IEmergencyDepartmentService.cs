using SaCodeWhite.Shared.Models.EmergencyDepartment;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Api.Services
{
    public interface IEmergencyDepartmentService
    {
        Task<(int id, DateTime timestampUtc)> GetEtlAsync(CancellationToken cancellationToken);

        Task<IList<EmergencyDepartmentDashboard>> GetDashboardsAsync(CancellationToken cancellationToken);

        Task<IList<EmergencyDepartmentStatus>> GetStatusesAsync(CancellationToken cancellationToken);
        Task<IList<EmergencyDepartmentDeparturesLast1Hr>> GetDeparturesLast1HrAsync(CancellationToken cancellationToken);
        Task<IList<EmergencyDepartmentTriageCategory>> GetTriageCategoriesAsync(CancellationToken cancellationToken);
        Task<IList<EmergencyDepartmentWaitingTime>> GetWaitingTimesAsync(CancellationToken cancellationToken);
    }
}
