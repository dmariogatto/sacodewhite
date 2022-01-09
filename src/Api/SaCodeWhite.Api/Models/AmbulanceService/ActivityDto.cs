using Newtonsoft.Json;
using System;

namespace SaCodeWhite.Api.Models.AmbulanceService
{
    public class ActivityDto : BaseDto
    {
        [JsonProperty("DTM")]
        public DateTime? TimestampAdelaide { get; set; }

        [JsonProperty("TOTAL")]
        public string Total { get; set; }

        [JsonProperty("CAPACITY")]
        public string Capacity { get; set; }

        [JsonProperty("WTBS")]
        public string WaitingToBeSeen { get; set; }

        [JsonProperty("COM_TREAT")]
        public string CommencedTreatment { get; set; }

        [JsonProperty("DEPARTURES")]
        public string Departures { get; set; }
    }
}
