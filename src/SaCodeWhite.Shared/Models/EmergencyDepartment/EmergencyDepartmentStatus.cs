using Newtonsoft.Json;
using System;

namespace SaCodeWhite.Shared.Models.EmergencyDepartment
{
    public class EmergencyDepartmentStatus : IStatus
    {
        [JsonProperty("hos_code")]
        public string HospitalCode { get; set; }

        [JsonProperty("ea")]
        public int ExpectedArrivals { get; set; }

        [JsonProperty("wtbs")]
        public int WaitingToBeSeen { get; set; }

        [JsonProperty("comm_treat")]
        public int CommencedTreatment { get; set; }

        [JsonProperty("cap")]
        public int Capacity { get; set; }

        [JsonProperty("avg_wait_mins")]
        public double AverageWaitMins { get; set; }

        [JsonProperty("updated_utc")]
        public DateTime UpdatedUtc { get; set; }

        [JsonIgnore]
        public DashboardType Dashboard => DashboardType.EmergencyDepartment;

        [JsonIgnore]
        public int PatientTotal => WaitingToBeSeen + CommencedTreatment;
    }
}
