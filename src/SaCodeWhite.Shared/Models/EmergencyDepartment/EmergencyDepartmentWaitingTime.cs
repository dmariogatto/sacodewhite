using Newtonsoft.Json;
using System;

namespace SaCodeWhite.Shared.Models.EmergencyDepartment
{
    public class EmergencyDepartmentWaitingTime
    {
        [JsonProperty("hos_code")]
        public string HospitalCode { get; set; }

        [JsonProperty("cat")]
        public string Category { get; set; }

        [JsonProperty("lt2hrs")]
        public int LessThan2Hours { get; set; }

        [JsonProperty("b2and4hrs")]
        public int Between2And4Hours { get; set; }

        [JsonProperty("b4and8hrs")]
        public int Between4And8Hours { get; set; }

        [JsonProperty("b8and12hrs")]
        public int Between8And12Hours { get; set; }

        [JsonProperty("b12and24hrs")]
        public int Between12And24Hours { get; set; }

        [JsonProperty("o24hrs")]
        public int Over24Hours { get; set; }

        [JsonIgnore]
        public int Total
            => LessThan2Hours + Between2And4Hours + Between4And8Hours + Between8And12Hours + Between12And24Hours + Over24Hours;
    }
}
