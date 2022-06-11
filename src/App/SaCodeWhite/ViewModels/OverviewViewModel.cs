using MvvmHelpers;
using MvvmHelpers.Commands;
using SaCodeWhite.Models;
using SaCodeWhite.Shared;
using SaCodeWhite.Shared.Models;
using SaCodeWhite.Shared.Models.EmergencyDepartment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.ViewModels
{
    public class OverviewViewModel : BaseAlertStatusCategoriesViewModel
    {
        public OverviewViewModel(
            IBvmConstructor bvmConstructor) : base(bvmConstructor)
        {
            Title = Shared.Localisation.Resources.Pulse;

            Overviews = new ObservableRangeCollection<HospitalOverview>();

            LoadOverviewsCommand = new AsyncCommand<CancellationToken>(LoadOverviewsAsync);
        }

        #region Overrides
        public override void OnCreate()
        {
            base.OnCreate();

            TrackEvent(AppCenterEvents.PageView.OverviewView);
        }
        #endregion

        public ObservableRangeCollection<HospitalOverview> Overviews { get; private set; }

        public AsyncCommand<CancellationToken> LoadOverviewsCommand { get; private set; }

        private async Task LoadOverviewsAsync(CancellationToken ct)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var hospitalTask = CodeWhiteService.GetHospitalsAsync(ct);
                var ambulanceTask = CodeWhiteService.GetAmbulanceServiceDashboardsAsync(ct);
                var emergencyDepartmentTask = CodeWhiteService.GetEmergencyDepartmentDashboardsAsync(ct);

                await Task.WhenAll(hospitalTask, ambulanceTask, emergencyDepartmentTask);

                var overviews = default(IList<HospitalOverview>);

                if (!Overviews.Any())
                {
                    var hospitals = await CodeWhiteService.GetHospitalsAsync(ct);
                    overviews = hospitals.Select(i => new HospitalOverview()
                    {
                        HospitalCode = i.Code,
                        HospitalName = i.DisplayName,
                    }).ToList();
                }

                overviews ??= Overviews;

                var ambulanceLookup = ambulanceTask.Result.ToDictionary(i => i.HospitalCode, i => i);
                var emergencyDepartmentLookup = emergencyDepartmentTask.Result.ToDictionary(i => i.HospitalCode, i => i);

                var greenCount = 0;
                var amberCount = 0;
                var redCount = 0;
                var whiteCount = 0;

                foreach (var o in overviews)
                {
                    if (ambulanceLookup.TryGetValue(o.HospitalCode, out var ambulance) &&
                        emergencyDepartmentLookup.TryGetValue(o.HospitalCode, out var emergencyDepartment))
                    {
                        var amboStats = ambulance.GetAlertStatus();
                        var edStats = emergencyDepartment.GetAlertStatus();

                        o.AlertStatus = (AlertStatusType)Math.Max((int)amboStats.alertStatus, (int)edStats.alertStatus);
                        o.OccupiedCapacity = Math.Max(amboStats.occupiedCapacity, edStats.occupiedCapacity);

                        o.AverageWaitMins = emergencyDepartment.AverageWaitMins;

                        var amboAccessBlock = ambulance.InpatientBedStatus is not null && ambulance.Capacity > 0
                            ? ambulance.InpatientBedStatus.WaitingForBed / (double)ambulance.Capacity
                            : 0d;
                        var edAccessBlock =
                            emergencyDepartment.WaitingTimes
                                ?.LastOrDefault(i => i.Category.Contains("bed", StringComparison.InvariantCultureIgnoreCase)) is EmergencyDepartmentWaitingTime wfb &&
                            emergencyDepartment.Capacity > 0
                            ? wfb.Total / (double)emergencyDepartment.Capacity
                            : 0d;

                        o.AccessBlock = Math.Max(amboAccessBlock, edAccessBlock);

                        _ = o.AlertStatus switch
                        {
                            AlertStatusType.Green => greenCount++,
                            AlertStatusType.Amber => amberCount++,
                            AlertStatusType.Red => redCount++,
                            AlertStatusType.White => whiteCount++,
                            _ => throw new NotSupportedException()
                        };
                    }
                }

                if (!ReferenceEquals(Overviews, overviews) && !overviews.SequenceEqual(Overviews))
                    Overviews.ReplaceRange(overviews);

                UpdateAlertStatusCategoryCount(AlertStatusType.Green, greenCount);
                UpdateAlertStatusCategoryCount(AlertStatusType.Amber, amberCount);
                UpdateAlertStatusCategoryCount(AlertStatusType.Red, redCount);
                UpdateAlertStatusCategoryCount(AlertStatusType.White, whiteCount);
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