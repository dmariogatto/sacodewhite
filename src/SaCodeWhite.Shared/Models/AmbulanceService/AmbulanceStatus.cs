using Newtonsoft.Json;
using System;

namespace SaCodeWhite.Shared.Models.AmbulanceService
{
    public class AmbulanceStatus : IStatus
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

        [JsonProperty("resus")]
        public int Resuscitation { get; set; }

        [JsonProperty("clr_last_3hrs")]
        public AmbulanceLast3Hrs ClearanceLast3Hrs { get; set; }

        [JsonProperty("updated_utc")]
        public DateTime UpdatedUtc { get; set; }

        [JsonIgnore]
        public DashboardType Dashboard => DashboardType.AmbulanceService;

        [JsonIgnore]
        public int PatientTotal => WaitingToBeSeen + CommencedTreatment;
    }
}
