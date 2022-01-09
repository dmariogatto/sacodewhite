using Newtonsoft.Json;
using System;

namespace SaCodeWhite.Api.Models.AmbulanceService
{
    public class InpatientBedStatusDto : BaseDto
    {
        [JsonProperty("HOST_TYPE")]
        public string HospitalType { get; set; }

        [JsonProperty("DTM1")]
        public string TimestampAdelaide { get; set; }

        [JsonProperty("WFB")]
        public string WaitingForBed { get; set; }

        [JsonProperty("OCC")]
        public string GeneralWardOccupancy { get; set; }

        [JsonProperty("BASE")]
        public string GeneralWardCapacity { get; set; }
    }
}
