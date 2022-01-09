using SaCodeWhite.Shared.Models.EmergencyDepartment;

namespace SaCodeWhite.ViewModels
{
    public class EmergencyDepartmentHospitalViewModel : HospitalDashboardViewModel<EmergencyDepartmentDashboard>
    {
        public EmergencyDepartmentHospitalViewModel(
            IBvmConstructor bvmConstructor) : base( bvmConstructor)
        {
        }
    }
}