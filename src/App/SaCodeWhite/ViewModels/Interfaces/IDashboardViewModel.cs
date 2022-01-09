using MvvmHelpers;
using MvvmHelpers.Commands;
using SaCodeWhite.Models;
using System.Threading;

namespace SaCodeWhite.ViewModels
{
    public interface IDashboardViewModel : IBaseDashboardViewModel
    {
        public ObservableRangeCollection<AlertStatusCategory> AlertStatusCategories { get; }
        public ObservableRangeCollection<HospitalDashboard> Dashboards { get; }

        public AsyncCommand<CancellationToken> LoadDashboardsCommand { get; }
        public AsyncCommand<HospitalDashboard> HospitalTappedCommand { get; }
    }
}