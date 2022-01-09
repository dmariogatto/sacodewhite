using Newtonsoft.Json;
using System;

namespace SaCodeWhite.Api.Models.AmbulanceService
{
    public class SpecialityBedStatusDto : BaseDto
    {
        [JsonProperty("DTM1")]
        public string TimestampAdelaide { get; set; }

        [JsonProperty("BURN_OCC")]
        public string BurnOccupancy { get; set; }

        [JsonProperty("BURN_CAP")]
        public string BurnCapacity { get; set; }

        [JsonProperty("CCU_OCC")]
        public string CoronaryCareOccupancy { get; set; }

        [JsonProperty("CCU_CAP")]
        public string CoronaryCareCapacity { get; set; }

        [JsonProperty("ICU_OCC")]
        public string IntensiveCareOccupancy { get; set; }

        [JsonProperty("ICU_CAP")]
        public string IntensiveCareCapacity { get; set; }

        [JsonProperty("MH_OCC")]
        public string MentalHealthOccupancy { get; set; }

        [JsonProperty("MH_CAP")]
        public string MentalHealthCapacity { get; set; }

        [JsonProperty("NEO_OCC")]
        public string NeonatalOccupancy { get; set; }

        [JsonProperty("NEO_CAP")]
        public string NeonatalCapacity { get; set; }

        [JsonProperty("OBST_OCC")]
        public string ObstetricOccupancy { get; set; }

        [JsonProperty("OBST_CAP")]
        public string ObstetricCapacity { get; set; }

        [JsonProperty("PAED_OCC")]
        public string PaediatricOccupancy { get; set; }

        [JsonProperty("PAED_CAP")]
        public string PaediatricCapacity { get; set; }
    }
}
