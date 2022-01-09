using Newtonsoft.Json;

namespace SaCodeWhite.Api.Models.EmergencyDepartment
{
    public class EtlDto
    {
        [JsonProperty("ID")]
        public string Id { get; set; }

        [JsonProperty("CURR_DTM")]
        public string TimestampAdelaide { get; set; }
    }
}
