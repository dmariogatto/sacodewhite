using Newtonsoft.Json;
using System;

namespace SaCodeWhite.Api.Models.AmbulanceService
{
    public class StatusDto : BaseDto
    {
        [JsonProperty("DTM1")]
        public string TimestampAdelaide { get; set; }

        [JsonProperty("ALERT")]
        public string Alert { get; set; }

        [JsonProperty("CAP")]
        public string Capacity { get; set; }

        [JsonProperty("TOT")]
        public string Total { get; set; }

        [JsonProperty("EA")]
        public string ExpectedAmbulanceArrivals { get; set; }

        [JsonProperty("WTBS")]
        public string WaitingToBeSeen { get; set; }

        [JsonProperty("BT")]
        public string CommencedTreatment { get; set; }

        [JsonProperty("RESUS")]
        public string Resus { get; set; }
    }
}
