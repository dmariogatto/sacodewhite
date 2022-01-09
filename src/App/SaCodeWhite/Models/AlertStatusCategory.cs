using MvvmHelpers;
using SaCodeWhite.Shared.Models;

namespace SaCodeWhite.Models
{
    public class AlertStatusCategory : ObservableObject
    {
        public AlertStatusCategory(AlertStatusType alertStatus)
        {
            AlertStatus = alertStatus;
        }

        private AlertStatusType _alertStatus;
        public AlertStatusType AlertStatus
        {
            get => _alertStatus;
            private set => SetProperty(ref _alertStatus, value);
        }

        private int _count;
        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }
    }
}