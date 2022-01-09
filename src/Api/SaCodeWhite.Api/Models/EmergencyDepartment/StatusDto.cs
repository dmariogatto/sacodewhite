using Newtonsoft.Json;
using System;

namespace SaCodeWhite.Api.Models.EmergencyDepartment
{
    public class StatusDto : BaseDto
    {
        [JsonProperty("DTM")]
        public DateTime? TimestampAdelaide { get; set; }

        [JsonProperty("EA")]
        public string ExpectedArrivals { get; set; }

        [JsonProperty("WTBS")]
        public string WaitingToBeSeen { get; set; }

        [JsonProperty("COM_TREAT")]
        public string CommencedTreatment { get; set; }

        [JsonProperty("CAP")]
        public string Capacity { get; set; }

        [JsonProperty("AVG_WAIT")]
        public string AverageWaitMins { get; set; }
    }
}
