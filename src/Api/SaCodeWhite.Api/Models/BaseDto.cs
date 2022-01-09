using Newtonsoft.Json;

namespace SaCodeWhite.Api.Models
{
    public class BaseDto
    {
        [JsonProperty("HOSP_SHORT")]
        public string HospitalCode { get; set; }
    }
}
