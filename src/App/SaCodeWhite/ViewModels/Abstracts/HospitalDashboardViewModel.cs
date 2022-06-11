using MvvmHelpers.Commands;
using SaCodeWhite.Models;
using SaCodeWhite.Shared.Localisation;
using SaCodeWhite.Shared.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.ViewModels
{
    public abstract class HospitalDashboardViewModel<T> : BaseDashboardViewModel<T>, IHospitalDashboardViewModel where T : IStatus
    {
        public HospitalDashboardViewModel(
            IBvmConstructor bvmConstructor) : base(bvmConstructor)
        {
            Title = Resources.Hospital;

            LoadHospitalCommand = new AsyncCommand<(string code, CancellationToken ct)>(p => LoadHospitalAsync(p.code, p.ct));
        }

        #region Overrides
        public override void OnCreate()
        {
            base.OnCreate();

            TrackEvent(AppCenterEvents.PageView.HospitalView);
        }
        #endregion

        private Hospital _hospital;
        public Hospital Hospital
        {
            get => _hospital;
            private set => SetProperty(ref _hospital, value);
        }

        private HospitalDashboard _dashboard;
        public HospitalDashboard Dashboard
        {
            get => _dashboard;
            private set => SetProperty(ref _dashboard, value);
        }

        public AsyncCommand<(string, CancellationToken)> LoadHospitalCommand { get; private set; }

        private async Task LoadHospitalAsync(string code, CancellationToken ct)
        {
            if (IsBusy || string.IsNullOrEmpty(code))
                return;

            IsBusy = true;

            try
            {
                if (Hospital?.Code != code)
                {
                    Hospital = await CodeWhiteService.GetHospitalAsync(code, ct);
                    Title = Hospital?.DisplayName ?? Resources.Hospital;
                }

                if (!string.IsNullOrEmpty(Hospital?.Code))
                {
                    var data = Type switch
                    {
                        DashboardType.AmbulanceService => await CodeWhiteService.GetAmbulanceServiceDashboardAsync(Hospital.Code, ct),
                        DashboardType.EmergencyDepartment => await CodeWhiteService.GetEmergencyDepartmentDashboardAsync(Hospital.Code, ct),
                        _ => default(IStatus)
                    };

                    if (data is not null)
                    {
                        Dashboard ??= new HospitalDashboard();

                        Dashboard.HospitalCode = Hospital.Code;
                        Dashboard.HospitalName = Hospital.DisplayName;
                        Dashboard.Latitude = Hospital.Latitude;
                        Dashboard.Longitude = Hospital.Longitude;

                        Dashboard.Update(data);

                        LastUpdatedUtc = Dashboard.UpdatedUtc;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}