using Newtonsoft.Json;

namespace SaCodeWhite.Api.Models.AmbulanceService
{
    public class TriageCategoryDto : BaseDto
    {
        [JsonProperty("HOST_CAT")]
        public string HospitalCategory { get; set; }

        [JsonProperty("CAT")]
        public string Category { get; set; }

        [JsonProperty("BT")]
        public string CommencedTreatment { get; set; }

        [JsonProperty("WTBS")]
        public string WaitingToBeSeen { get; set; }
    }
}
