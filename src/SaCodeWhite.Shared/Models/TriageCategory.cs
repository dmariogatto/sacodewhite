using Newtonsoft.Json;

namespace SaCodeWhite.Shared.Models
{
    public class TriageCategory
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("max_wait_mins")]
        public int MaxWaitMins { get; set; }
    }
}
