using MvvmHelpers.Commands;
using SaCodeWhite.Models;
using SaCodeWhite.Shared.Models;
using System.Threading;

namespace SaCodeWhite.ViewModels
{
    public interface IHospitalDashboardViewModel : IBaseDashboardViewModel
    {
        public Hospital Hospital { get; }
        public HospitalDashboard Dashboard { get; }

        public AsyncCommand<(string, CancellationToken)> LoadHospitalCommand { get; }
    }
}