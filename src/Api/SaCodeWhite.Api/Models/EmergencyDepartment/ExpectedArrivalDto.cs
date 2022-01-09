using Newtonsoft.Json;

namespace SaCodeWhite.Api.Models.EmergencyDepartment
{
    public class ExpectedArrivalDto : BaseDto
    {

        [JsonProperty("CATEGORY")]
        public string Category { get; set; }

        [JsonProperty("TOTAL")]
        public string Total { get; set; }
    }
}
