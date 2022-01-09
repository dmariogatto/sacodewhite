using System;

namespace SaCodeWhite.Models
{
    public class InpatientBeds : AlertModel
    {
        public InpatientBeds()
        {
        }

        private int _waitingForBed;
        public int WaitingForBed
        {
            get => _waitingForBed;
            set => SetProperty(ref _waitingForBed, value);
        }

        public int Max
            => Math.Max(WaitingForBed, Math.Max(Occupancy, Capacity));

        public override void UpdateProperties()
        {
            base.UpdateProperties();

            OnPropertyChanged(nameof(Max));
        }
    }
}