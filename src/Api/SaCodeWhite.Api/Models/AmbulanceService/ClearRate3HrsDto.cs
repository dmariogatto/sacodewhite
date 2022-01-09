using Newtonsoft.Json;

namespace SaCodeWhite.Api.Models.AmbulanceService
{
    public class ClearRate3HrsDto : BaseDto
    {
        [JsonProperty("CLR")]
        public string Cleared { get; set; }

        [JsonProperty("ACT")]
        public string AvgClearTimeMins { get; set; }

        [JsonProperty("Plus30Min")]
        public string WaitingMoreThan30Mins { get; set; }
    }
}
