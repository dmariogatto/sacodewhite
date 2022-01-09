using Newtonsoft.Json;
using System;

namespace SaCodeWhite.Shared.Models.AmbulanceService
{
    public class AmbulanceSpecialityBedStatus
    {
        [JsonProperty("hos_code")]
        public string HospitalCode { get; set; }

        [JsonProperty("burn_occ")]
        public int BurnOccupancy { get; set; }

        [JsonProperty("burn_cap")]
        public int BurnCapacity { get; set; }

        [JsonProperty("ccu_occ")]
        public int CoronaryCareOccupancy { get; set; }

        [JsonProperty("ccu_cap")]
        public int CoronaryCareCapacity { get; set; }

        [JsonProperty("icu_occ")]
        public int IntensiveCareOccupancy { get; set; }

        [JsonProperty("icu_cap")]
        public int IntensiveCareCapacity { get; set; }

        [JsonProperty("mh_occ")]
        public int MentalHealthOccupancy { get; set; }

        [JsonProperty("mh_cap")]
        public int MentalHealthCapacity { get; set; }

        [JsonProperty("neo_occ")]
        public int NeonatalOccupancy { get; set; }

        [JsonProperty("neo_cap")]
        public int NeonatalCapacity { get; set; }

        [JsonProperty("obst_occ")]
        public int ObstetricOccupancy { get; set; }

        [JsonProperty("obst_cap")]
        public int ObstetricCapacity { get; set; }

        [JsonProperty("paed_occ")]
        public int PaediatricOccupancy { get; set; }

        [JsonProperty("paed_cap")]
        public int PaediatricCapacity { get; set; }

        [JsonProperty("updated_utc")]
        public DateTime UpdatedUtc { get; set; }
    }
}
