using Newtonsoft.Json;

namespace SaCodeWhite.Shared.Models.AmbulanceService
{
    public class AmbulanceLast3Hrs
    {
        [JsonProperty("clr")]
        public int Cleared { get; set; }

        [JsonProperty("avg_clr_mins")]
        public int AvgClearedMins { get; set; }

        [JsonProperty("gt_30mins")]
        public int WaitingMoreThan30Mins { get; set; }
    }
}
