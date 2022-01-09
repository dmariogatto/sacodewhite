using Newtonsoft.Json;

namespace SaCodeWhite.Api.Models.EmergencyDepartment
{
    public class TriageCategoryDto : BaseDto
    {
        [JsonProperty("CAT")]
        public string Category { get; set; }

        [JsonProperty("WTS")]
        public string WaitingToBeSeen { get; set; }

        [JsonProperty("WOT")]
        public string WaitingOverTime { get; set; }

        [JsonProperty("ALERT")]
        public string Alert { get; set; }

        [JsonProperty("OTH")]
        public string CommencedTreatment { get; set; }

        [JsonProperty("TOT")]
        public string Total { get; set; }
    }
}
