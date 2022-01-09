using MvvmHelpers;
using SaCodeWhite.Shared.Models;
using System.Diagnostics;

namespace SaCodeWhite.Models
{
    [DebuggerDisplay("{HospitalCode}")]
    public class HospitalOverview : ObservableObject
    {
        public HospitalOverview()
        {
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

        private double _occupiedCapacity;
        public double OccupiedCapacity
        {
            get => _occupiedCapacity;
            set => SetProperty(ref _occupiedCapacity, value);
        }

        private double _accessBlock;
        public double AccessBlock
        {
            get => _accessBlock;
            set => SetProperty(ref _accessBlock, value);
        }

        private double _averageWaitMins;
        public double AverageWaitMins
        {
            get => _averageWaitMins;
            set => SetProperty(ref _averageWaitMins, value);
        }

        private AlertStatusType _alertStatus;
        public AlertStatusType AlertStatus
        {
            get => _alertStatus;
            set => SetProperty(ref _alertStatus, value);
        }
    }
}