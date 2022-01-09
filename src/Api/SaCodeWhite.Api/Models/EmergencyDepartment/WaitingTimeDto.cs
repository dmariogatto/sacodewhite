using Newtonsoft.Json;

namespace SaCodeWhite.Api.Models.EmergencyDepartment
{
    public class WaitingTimeDto : BaseDto
    {
        [JsonProperty("CATEGORY")]
        public string Category { get; set; }

        [JsonProperty("c0_2")]
        public string LessThan2Hours { get; set; }

        [JsonProperty("c2_4")]
        public string Between2And4Hours { get; set; }

        [JsonProperty("c4_8")]
        public string Between4And8Hours { get; set; }

        [JsonProperty("c8_12")]
        public string Between8And12Hours { get; set; }

        [JsonProperty("c12_24")]
        public string Between12And24Hours { get; set; }

        [JsonProperty("c24Plus")]
        public string Over24Hours { get; set; }

        [JsonProperty("cTOT")]
        public string Total { get; set; }
    }
}
