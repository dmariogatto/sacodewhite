using MvvmHelpers;
using SaCodeWhite.Shared.Models;
using System;

namespace SaCodeWhite.Models
{
    public abstract class AlertModel : ObservableObject
    {
        private readonly CapacityAlertConfig _capacityAlertConfig;

        public AlertModel() : this(new CapacityAlertConfig())
        {
        }

        public AlertModel(CapacityAlertConfig capacityAlertConfig)
        {
            _capacityAlertConfig = capacityAlertConfig;
        }

        private int _occupancy;
        public int Occupancy
        {
            get => _occupancy;
            set => SetProperty(ref _occupancy, value);
        }

        private int _capacity;
        public int Capacity
        {
            get => _capacity;
            set => SetProperty(ref _capacity, value);
        }

        private double _occupiedCapacity;
        public double OccupiedCapacity
        {
            get => _occupiedCapacity;
            set => SetProperty(ref _occupiedCapacity, value);
        }

        public double CapacityGreenPercent
            => OccupiedCapacity <= 1
                ? Math.Min(OccupiedCapacity, _capacityAlertConfig.GreenLimit)
                : _capacityAlertConfig.GreenLimit / OccupiedCapacity;
        public double CapacityAmberPercent
            => OccupiedCapacity <= 1
                ? Math.Max(0, OccupiedCapacity - _capacityAlertConfig.GreenLimit - Math.Max(0, OccupiedCapacity - _capacityAlertConfig.AmberLimit))
                : (OccupiedCapacity - _capacityAlertConfig.GreenLimit - Math.Max(0, OccupiedCapacity - _capacityAlertConfig.AmberLimit)) / OccupiedCapacity;
        public double CapacityRedPercent
            => OccupiedCapacity <= 1
                ? Math.Max(0, OccupiedCapacity - _capacityAlertConfig.AmberLimit - Math.Max(0, OccupiedCapacity - _capacityAlertConfig.RedLimit))
                : (OccupiedCapacity - _capacityAlertConfig.AmberLimit - Math.Max(0, OccupiedCapacity - _capacityAlertConfig.RedLimit)) / OccupiedCapacity;
        public double CapacityWhitePercent
             => OccupiedCapacity > _capacityAlertConfig.RedLimit
                ? (OccupiedCapacity - _capacityAlertConfig.RedLimit) / OccupiedCapacity
                : 0;

        public virtual void UpdateProperties()
        {
            OnPropertyChanged(nameof(CapacityGreenPercent));
            OnPropertyChanged(nameof(CapacityAmberPercent));
            OnPropertyChanged(nameof(CapacityRedPercent));
            OnPropertyChanged(nameof(CapacityWhitePercent));
        }
    }
}