using Newtonsoft.Json;
using System;

namespace SaCodeWhite.Shared.Models.AmbulanceService
{
    public class AmbulanceInpatientBedStatus
    {
        [JsonProperty("hos_code")]
        public string HospitalCode { get; set; }

        [JsonProperty("wfb")]
        public int WaitingForBed { get; set; }

        [JsonProperty("gen_ward_occ")]
        public int GeneralWardOccupancy { get; set; }

        [JsonProperty("gen_ward_cap")]
        public int GeneralWardCapacity { get; set; }

        [JsonProperty("updated_utc")]
        public DateTime UpdatedUtc { get; set; }
    }
}
