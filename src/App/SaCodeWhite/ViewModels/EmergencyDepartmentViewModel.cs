using SaCodeWhite.Shared.Models.EmergencyDepartment;

namespace SaCodeWhite.ViewModels
{
    public class EmergencyDepartmentViewModel : DashboardViewModel<EmergencyDepartmentDashboard>
    {
        public EmergencyDepartmentViewModel(
            IBvmConstructor bvmConstructor) : base( bvmConstructor)
        {
        }
    }
}