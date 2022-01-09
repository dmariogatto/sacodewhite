using Newtonsoft.Json;
using System;

namespace SaCodeWhite.Shared.Models.EmergencyDepartment
{
    public class EmergencyDepartmentDeparturesLast1Hr
    {
        [JsonProperty("hos_code")]
        public string HospitalCode { get; set; }

        [JsonProperty("cat")]
        public string Category { get; set; }

        [JsonProperty("cnt")]
        public int Count { get; set; }
    }
}
