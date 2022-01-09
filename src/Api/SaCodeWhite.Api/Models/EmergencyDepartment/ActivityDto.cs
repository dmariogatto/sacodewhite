using Newtonsoft.Json;
using System;

namespace SaCodeWhite.Api.Models.EmergencyDepartment
{
    public class ActivityDto : BaseDto
    {
        [JsonProperty("DTM")]
        public DateTime? TimestampAdelaide { get; set; }

        [JsonProperty("ARR")]
        public string Arrivals { get; set; }

        [JsonProperty("OCC")]
        public string Occupancy { get; set; }

        [JsonProperty("CAP")]
        public string Capacity { get; set; }

        [JsonProperty("WTBS")]
        public string WaitingToBeSeen { get; set; }

        [JsonProperty("TREAT_COM")]
        public string CommencedTreatment { get; set; }

        [JsonProperty("DEP")]
        public string Departures { get; set; }
    }
}
