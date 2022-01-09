using System;

namespace SaCodeWhite.Shared.Models
{
    public interface IStatus : IAlertable
    {
        public string HospitalCode { get; set; }

        public int ExpectedArrivals { get; }

        public int WaitingToBeSeen { get; }

        public int CommencedTreatment { get; }

        public DateTime UpdatedUtc { get; }

        public DashboardType Dashboard { get; }
    }
}
