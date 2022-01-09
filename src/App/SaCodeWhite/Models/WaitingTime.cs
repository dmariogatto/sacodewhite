using MvvmHelpers;
using SaCodeWhite.Shared.Models.EmergencyDepartment;

namespace SaCodeWhite.Models
{
    public class WaitingTime : ObservableObject
    {
        public WaitingTime(string displayName)
        {
            DisplayName = displayName;
        }

        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        private int _lessThan2Hours;
        public int LessThan2Hours
        {
            get => _lessThan2Hours;
            set => SetProperty(ref _lessThan2Hours, value);
        }

        private int _between2And4Hours;
        public int Between2And4Hours
        {
            get => _between2And4Hours;
            set => SetProperty(ref _between2And4Hours, value);
        }

        private int _between4And8Hours;
        public int Between4And8Hours
        {
            get => _between4And8Hours;
            set => SetProperty(ref _between4And8Hours, value);
        }

        private int _between8And12Hours;
        public int Between8And12Hours
        {
            get => _between8And12Hours;
            set => SetProperty(ref _between8And12Hours, value);
        }

        private int _between12And24Hours;
        public int Between12And24Hours
        {
            get => _between12And24Hours;
            set => SetProperty(ref _between12And24Hours, value);
        }

        private int _over24Hours;
        public int Over24Hours
        {
            get => _over24Hours;
            set => SetProperty(ref _over24Hours, value);
        }

        public int Total
            => LessThan2Hours + Between2And4Hours + Between4And8Hours + Between8And12Hours + Between12And24Hours + Over24Hours;

        public double LessThan2HoursPercent => LessThan2Hours / (double)Total;
        public double Between2And4HoursPercent => Between2And4Hours / (double)Total;
        public double Between4And8HoursPercent => Between4And8Hours / (double)Total;
        public double Between8And12HoursPercent => Between8And12Hours / (double)Total;
        public double Between12And24HoursPercent => Between12And24Hours / (double)Total;
        public double Over24HoursPercent => Over24Hours / (double)Total;

        public void Update(EmergencyDepartmentWaitingTime data)
        {
            LessThan2Hours = data.LessThan2Hours;
            Between2And4Hours = data.Between2And4Hours;
            Between4And8Hours = data.Between4And8Hours;
            Between8And12Hours = data.Between8And12Hours;
            Between12And24Hours = data.Between12And24Hours;
            Over24Hours = data.Over24Hours;

            UpdateProperties();
        }

        private void UpdateProperties()
        {
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(LessThan2HoursPercent));
            OnPropertyChanged(nameof(Between2And4HoursPercent));
            OnPropertyChanged(nameof(Between4And8HoursPercent));
            OnPropertyChanged(nameof(Between8And12HoursPercent));
            OnPropertyChanged(nameof(Between12And24HoursPercent));
            OnPropertyChanged(nameof(Over24HoursPercent));
        }
    }
}