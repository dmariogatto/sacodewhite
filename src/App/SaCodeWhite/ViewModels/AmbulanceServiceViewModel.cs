using SaCodeWhite.Shared.Models.AmbulanceService;

namespace SaCodeWhite.ViewModels
{
    public class AmbulanceServiceViewModel : DashboardViewModel<AmbulanceServiceDashboard>
    {
        public AmbulanceServiceViewModel(
            IBvmConstructor bvmConstructor) : base( bvmConstructor)
        {
        }
    }
}