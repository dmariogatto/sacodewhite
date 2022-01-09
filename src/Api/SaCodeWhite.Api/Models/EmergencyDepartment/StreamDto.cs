using Newtonsoft.Json;

namespace SaCodeWhite.Api.Models.EmergencyDepartment
{
    public class StreamDto : BaseDto
    {
        [JsonProperty("WTS_TOT")]
        public string WaitingToBeSeenTotal { get; set; }

        [JsonProperty("WTS_WOT")]
        public string WaitingOverTime { get; set; }

        [JsonProperty("WTS_ALERT")]
        public string WaitingToBeSeenAlert { get; set; }

        [JsonProperty("TREAT_TOT")]
        public string BeingTreatedTotal { get; set; }

        [JsonProperty("WFB_TOT")]
        public string WaitingForBedTotal { get; set; }

        [JsonProperty("WFB_WOT")]
        public string WaitingForBedOverTime { get; set; }

        [JsonProperty("WFB_ALERT")]
        public string WaitingForBedAlert { get; set; }

        [JsonProperty("TOT_0_2")]
        public string LessThan2HoursTotal { get; set; }

        [JsonProperty("TOT_2_3")]
        public string Between2And3HoursTotal { get; set; }

        [JsonProperty("TOT_3_4")]
        public string Between3And4HoursTotal { get; set; }

        [JsonProperty("TOT_4_8")]
        public string Between4And8HoursTotal { get; set; }

        [JsonProperty("TOT_8Ples")]
        public string Over8HoursTotal { get; set; }

        [JsonProperty("EECU")]
        public string ExtendedEmergencyCareUnitTotal { get; set; }

        [JsonProperty("TOT")]
        public string Total { get; set; }
    }
}
