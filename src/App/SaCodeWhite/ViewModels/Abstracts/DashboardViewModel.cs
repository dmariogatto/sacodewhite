using MvvmHelpers;
using MvvmHelpers.Commands;
using SaCodeWhite.Models;
using SaCodeWhite.Shared.Localisation;
using SaCodeWhite.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.ViewModels
{
    public abstract class DashboardViewModel<T> : BaseDashboardViewModel<T>, IDashboardViewModel where T : IStatus
    {
        public DashboardViewModel(
            IBvmConstructor bvmConstructor) : base(bvmConstructor)
        {
            Title = Type switch
            {
                DashboardType.AmbulanceService => Resources.AmbulanceService,
                DashboardType.EmergencyDepartment => Resources.EmergencyDepartment,
                _ => string.Empty
            };

            Dashboards = new ObservableRangeCollection<HospitalDashboard>();

            LoadDashboardsCommand = new AsyncCommand<CancellationToken>(LoadDashboardsAsync);
            HospitalTappedCommand = new AsyncCommand<HospitalDashboard>((i) =>
            {
                var navParams = new Dictionary<string, string>()
                {
                    { NavigationKeys.HospitalCodeQueryProperty, i.HospitalCode ?? string.Empty }
                };

                return Type switch
                {
                    DashboardType.AmbulanceService => NavigationService.NavigateToAsync<AmbulanceServiceHospitalViewModel>(navParams),
                    DashboardType.EmergencyDepartment => NavigationService.NavigateToAsync<EmergencyDepartmentHospitalViewModel>(navParams),
                    _ => Task.CompletedTask
                };
            });
        }

        #region Overrides
        public override void OnCreate()
        {
            base.OnCreate();

            TrackEvent(Type switch
            {
                DashboardType.AmbulanceService => AppCenterEvents.PageView.AmbulanceServiceView,
                DashboardType.EmergencyDepartment => AppCenterEvents.PageView.EmergencyDepartmentView,
                _ => string.Empty
            });
        }

        protected override void HasInternetChanged(bool oldValue, bool newValue)
        {
            base.HasInternetChanged(oldValue, newValue);

            if (newValue)
                LoadDashboardsCommand.ExecuteAsync(default);
        }
        #endregion

        public ObservableRangeCollection<HospitalDashboard> Dashboards { get; private set; }

        public AsyncCommand<CancellationToken> LoadDashboardsCommand { get; private set; }
        public AsyncCommand<HospitalDashboard> HospitalTappedCommand { get; private set; }

        private async Task LoadDashboardsAsync(CancellationToken ct)
        {
            if (IsBusy || !HasInternet)
                return;

            IsBusy = true;

            try
            {
                async Task<IList<IStatus>> getDataAsync(CancellationToken ct)
                    => new List<IStatus>(Type switch
                    {
                        DashboardType.AmbulanceService => await CodeWhiteService.GetAmbulanceServiceDashboardsAsync(ct),
                        DashboardType.EmergencyDepartment => await CodeWhiteService.GetEmergencyDepartmentDashboardsAsync(ct),
                        _ => Array.Empty<IStatus>()
                    });

                var dataTask = getDataAsync(ct);

                var dashboards = default(IList<HospitalDashboard>);

                if (!Dashboards.Any())
                {
                    var hospitals = await CodeWhiteService.GetHospitalsAsync(ct);
                    dashboards = hospitals.Select(i => new HospitalDashboard()
                    {
                        Type = Type,
                        HospitalCode = i.Code,
                        HospitalName = i.DisplayName,
                        Latitude = i.Latitude,
                        Longitude = i.Longitude,
                    }).ToList();
                }

                dashboards ??= Dashboards;

                var dataLookup = (await dataTask).ToDictionary(i => i.HospitalCode, i => i);

                var greenCount = 0;
                var amberCount = 0;
                var redCount = 0;
                var whiteCount = 0;

                foreach (var d in dashboards)
                {
                    if (dataLookup.TryGetValue(d.HospitalCode, out var data))
                    {
                        d.Update(data);

                        _ = d.AlertStatus switch
                        {
                            AlertStatusType.Green => greenCount++,
                            AlertStatusType.Amber => amberCount++,
                            AlertStatusType.Red => redCount++,
                            AlertStatusType.White => whiteCount++,
                            _ => throw new NotSupportedException()
                        };
                    }
                }

                if (!ReferenceEquals(Dashboards, dashboards) && !dashboards.SequenceEqual(Dashboards))
                    Dashboards.ReplaceRange(dashboards);

                UpdateAlertStatusCategoryCount(AlertStatusType.Green, greenCount);
                UpdateAlertStatusCategoryCount(AlertStatusType.Amber, amberCount);
                UpdateAlertStatusCategoryCount(AlertStatusType.Red, redCount);
                UpdateAlertStatusCategoryCount(AlertStatusType.White, whiteCount);

                LastUpdatedUtc = Dashboards.Any()
                    ? Dashboards.Max(i => i.UpdatedUtc)
                    : DateTime.MinValue;
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