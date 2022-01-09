using Newtonsoft.Json;

namespace SaCodeWhite.Api.Models.EmergencyDepartment
{
    public class DepartureLast1HrDto : BaseDto
    {
        [JsonProperty("DEP_TYPE")]
        public string Type { get; set; }

        [JsonProperty("TOTAL")]
        public string Total { get; set; }
    }
}
