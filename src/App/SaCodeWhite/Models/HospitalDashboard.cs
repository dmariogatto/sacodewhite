using MvvmHelpers;
using SaCodeWhite.Shared;
using SaCodeWhite.Shared.Localisation;
using SaCodeWhite.Shared.Models;
using SaCodeWhite.Shared.Models.AmbulanceService;
using SaCodeWhite.Shared.Models.EmergencyDepartment;
using System;
using System.Diagnostics;
using System.Linq;

namespace SaCodeWhite.Models
{
    [DebuggerDisplay("{HospitalCode}")]
    public class HospitalDashboard : AlertModel
    {
        public HospitalDashboard() : this(new CapacityAlertConfig())
        {
        }

        public HospitalDashboard(CapacityAlertConfig capacityAlertConfig) : base(capacityAlertConfig)
        {
        }

        private DashboardType _type;
        public DashboardType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        private string _hospitalCode;
        public string HospitalCode
        {
            get => _hospitalCode;
            set => SetProperty(ref _hospitalCode, value);
        }

        private string _hospitalName;
        public string HospitalName
        {
            get => _hospitalName;
            set => SetProperty(ref _hospitalName, value);
        }

        private double _latitude;
        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        private double _longitude;
        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        private int _expectedArrivals;
        public int ExpectedArrivals
        {
            get => _expectedArrivals;
            set => SetProperty(ref _expectedArrivals, value);
        }

        private int _waitingToBeSeen;
        public int WaitingToBeSeen
        {
            get => _waitingToBeSeen;
            set => SetProperty(ref _waitingToBeSeen, value);
        }

        private int _commencedTreatment;
        public int CommencedTreatment
        {
            get => _commencedTreatment;
            set => SetProperty(ref _commencedTreatment, value);
        }

        private int _resuscitation;
        public int Resuscitation
        {
            get => _resuscitation;
            set => SetProperty(ref _resuscitation, value);
        }

        private double _averageWaitMins;
        public double AverageWaitMins
        {
            get => _averageWaitMins;
            set => SetProperty(ref _averageWaitMins, value);
        }

        private int _ambulancesClearedLast3Hrs;
        public int AmbulancesClearedLast3Hrs
        {
            get => _ambulancesClearedLast3Hrs;
            set => SetProperty(ref _ambulancesClearedLast3Hrs, value);
        }

        private int _ambulancesAvgClearedMinsLast3Hrs;
        public int AmbulancesAvgClearedMinsLast3Hrs
        {
            get => _ambulancesAvgClearedMinsLast3Hrs;
            set => SetProperty(ref _ambulancesAvgClearedMinsLast3Hrs, value);
        }

        private int _ambulancesWaitingOver30MinsLast3Hrs;
        public int AmbulancesWaitingOver30MinsLast3Hrs
        {
            get => _ambulancesWaitingOver30MinsLast3Hrs;
            set => SetProperty(ref _ambulancesWaitingOver30MinsLast3Hrs, value);
        }

        private AlertStatusType _alertStatus;
        public AlertStatusType AlertStatus
        {
            get => _alertStatus;
            set => SetProperty(ref _alertStatus, value);
        }

        private DateTime _updatedUtc = DateTime.MinValue;
        public DateTime UpdatedUtc
        {
            get => _updatedUtc;
            set => SetProperty(ref _updatedUtc, value);
        }

        private InpatientBeds _inpatientBeds;
        public InpatientBeds InpatientBeds
        {
            get => _inpatientBeds;
            set => SetProperty(ref _inpatientBeds, value);
        }

        public ObservableRangeCollection<SpecialtyBedOccupancy> SpecialtyBedOccupancies { get; } = new ObservableRangeCollection<SpecialtyBedOccupancy>();
        public ObservableRangeCollection<WaitingTime> WaitingTimes { get; } = new ObservableRangeCollection<WaitingTime>();

        private DateTime _specialtyBedUpdatedUtc = DateTime.MinValue;
        public DateTime SpecialtyBedUpdatedUtc
        {
            get => _specialtyBedUpdatedUtc;
            set => SetProperty(ref _specialtyBedUpdatedUtc, value);
        }

        private int _specialtyBedMax;
        public int SpecialtyBedMax
        {
            get => _specialtyBedMax;
            set => SetProperty(ref _specialtyBedMax, value);
        }

        private int _waitingMax;
        public int WaitingMax
        {
            get => _waitingMax;
            set => SetProperty(ref _waitingMax, value);
        }

        private double _accessBlock;
        public double AccessBlock
        {
            get => _accessBlock;
            set => SetProperty(ref _accessBlock, value);
        }

        public void Update(IStatus data)
        {
            ExpectedArrivals = data.ExpectedArrivals;
            WaitingToBeSeen = data.WaitingToBeSeen;
            CommencedTreatment = data.CommencedTreatment;
            Capacity = data.Capacity;
            UpdatedUtc = data.UpdatedUtc;

            Occupancy = WaitingToBeSeen + CommencedTreatment;

            var (alertStatus, occCap) = data.GetAlertStatus();

            OccupiedCapacity = occCap;
            AlertStatus = alertStatus;

            switch (data)
            {
                case AmbulanceServiceDashboard ambulance:
                    Resuscitation = ambulance.Resuscitation;

                    AmbulancesClearedLast3Hrs = ambulance.ClearanceLast3Hrs?.Cleared ?? -1;
                    AmbulancesAvgClearedMinsLast3Hrs = ambulance.ClearanceLast3Hrs?.AvgClearedMins ?? -1;
                    AmbulancesWaitingOver30MinsLast3Hrs = ambulance.ClearanceLast3Hrs?.WaitingMoreThan30Mins ?? -1;

                    if (ambulance.InpatientBedStatus is not null)
                    {
                        InpatientBeds ??= new InpatientBeds();
                        InpatientBeds.WaitingForBed = ambulance.InpatientBedStatus.WaitingForBed;
                        InpatientBeds.Occupancy = ambulance.InpatientBedStatus.GeneralWardOccupancy;
                        InpatientBeds.Capacity = ambulance.InpatientBedStatus.GeneralWardCapacity;
                        InpatientBeds.UpdateProperties();
                    }
                    else
                    {
                        InpatientBeds = null;

                    }

                    AccessBlock = InpatientBeds is not null && Capacity > 0
                        ? InpatientBeds.WaitingForBed / (double)Capacity
                        : 0d;

                    if (!SpecialtyBedOccupancies.Any())
                    {
                        SpecialtyBedOccupancies.Add(new SpecialtyBedOccupancy(Resources.BurnCode, Resources.BurnName));
                        SpecialtyBedOccupancies.Add(new SpecialtyBedOccupancy(Resources.CcuCode, Resources.CcuName));
                        SpecialtyBedOccupancies.Add(new SpecialtyBedOccupancy(Resources.IcuCode, Resources.IcuName));
                        SpecialtyBedOccupancies.Add(new SpecialtyBedOccupancy(Resources.MhCode, Resources.MhName));
                        SpecialtyBedOccupancies.Add(new SpecialtyBedOccupancy(Resources.NeoCode, Resources.NeoName));
                        SpecialtyBedOccupancies.Add(new SpecialtyBedOccupancy(Resources.ObstCode, Resources.ObstName));
                        SpecialtyBedOccupancies.Add(new SpecialtyBedOccupancy(Resources.PaedCode, Resources.PaedName));
                    }

                    SpecialtyBedOccupancies[0].Occupancy = ambulance.SpecialityBedStatus.BurnOccupancy;
                    SpecialtyBedOccupancies[0].Capacity = ambulance.SpecialityBedStatus.BurnCapacity;

                    SpecialtyBedOccupancies[1].Occupancy = ambulance.SpecialityBedStatus.CoronaryCareOccupancy;
                    SpecialtyBedOccupancies[1].Capacity = ambulance.SpecialityBedStatus.CoronaryCareCapacity;

                    SpecialtyBedOccupancies[2].Occupancy = ambulance.SpecialityBedStatus.IntensiveCareOccupancy;
                    SpecialtyBedOccupancies[2].Capacity = ambulance.SpecialityBedStatus.IntensiveCareCapacity;

                    SpecialtyBedOccupancies[3].Occupancy = ambulance.SpecialityBedStatus.MentalHealthOccupancy;
                    SpecialtyBedOccupancies[3].Capacity = ambulance.SpecialityBedStatus.MentalHealthCapacity;

                    SpecialtyBedOccupancies[4].Occupancy = ambulance.SpecialityBedStatus.NeonatalOccupancy;
                    SpecialtyBedOccupancies[4].Capacity = ambulance.SpecialityBedStatus.NeonatalCapacity;

                    SpecialtyBedOccupancies[5].Occupancy = ambulance.SpecialityBedStatus.ObstetricOccupancy;
                    SpecialtyBedOccupancies[5].Capacity = ambulance.SpecialityBedStatus.ObstetricCapacity;

                    SpecialtyBedOccupancies[6].Occupancy = ambulance.SpecialityBedStatus.PaediatricOccupancy;
                    SpecialtyBedOccupancies[6].Capacity = ambulance.SpecialityBedStatus.PaediatricCapacity;

                    foreach (var i in SpecialtyBedOccupancies)
                    {
                        i.OccupiedCapacity = i.Capacity > 0 ? i.Occupancy / (double)i.Capacity : 0;
                        i.UpdateProperties();
                    }

                    SpecialtyBedUpdatedUtc = ambulance.SpecialityBedStatus.UpdatedUtc;

                    SpecialtyBedMax = SpecialtyBedOccupancies.Any() ? SpecialtyBedOccupancies.Max(i => i.Capacity) : 0;
                    break;
                case EmergencyDepartmentDashboard emergencyDepartment:
                    AverageWaitMins = emergencyDepartment.AverageWaitMins;

                    if (!WaitingTimes.Any() || WaitingTimes.Count != (emergencyDepartment.WaitingTimes?.Count ?? -1))
                    {
                        WaitingTimes.Clear();
                        if (emergencyDepartment.WaitingTimes?.Any() == true)
                            foreach (var wt in emergencyDepartment.WaitingTimes)
                                WaitingTimes.Add(new WaitingTime(wt.Category));
                    }

                    for (var i = 0; i < WaitingTimes.Count; i++)
                    {
                        var wt = WaitingTimes[i];
                        var wtData = emergencyDepartment.WaitingTimes[i];
                        wt.Update(wtData);
                    }

                    WaitingMax = WaitingTimes.Any() ? WaitingTimes.Max(i => i.Total) : 0;

                    AccessBlock =
                        WaitingTimes.LastOrDefault(i => i.DisplayName.Contains("bed", StringComparison.InvariantCultureIgnoreCase)) is WaitingTime wfb &&
                        Capacity > 0
                        ? wfb.Total / (double)Capacity
                        : 0d;
                    break;
                default:
                    break;
            }

            UpdateProperties();
        }

        public override void UpdateProperties()
        {
            base.UpdateProperties();
        }
    }
}