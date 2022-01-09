using Newtonsoft.Json;
using System;

namespace SaCodeWhite.Shared.Models
{
    public abstract class TriageCategoryStatus
    {
        [JsonProperty("hos_code")]
        public string HospitalCode { get; set; }

        [JsonProperty("cat_id")]
        public int TriageCategoryId { get; set; }

        [JsonProperty("wtbs")]
        public int WaitingToBeSeen { get; set; }

        [JsonProperty("wot")]
        public int WaitingOverTime { get; set; }

        [JsonProperty("comm_treat")]
        public int CommencedTreatment { get; set; }
    }
}
