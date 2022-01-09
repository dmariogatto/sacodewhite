using SaCodeWhite.Shared.Models.AmbulanceService;

namespace SaCodeWhite.ViewModels
{
    public class AmbulanceServiceHospitalViewModel : HospitalDashboardViewModel<AmbulanceServiceDashboard>
    {
        public AmbulanceServiceHospitalViewModel(
            IBvmConstructor bvmConstructor) : base( bvmConstructor)
        {
        }
    }
}