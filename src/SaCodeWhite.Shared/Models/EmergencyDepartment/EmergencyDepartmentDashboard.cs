using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace SaCodeWhite.Shared.Models.EmergencyDepartment
{
    [DebuggerDisplay("{HospitalCode}")]
    public class EmergencyDepartmentDashboard : EmergencyDepartmentStatus
    {
        [JsonProperty("dep_last_1hr")]
        public List<EmergencyDepartmentDeparturesLast1Hr> DeparturesLast1Hr { get; set; }

        [JsonProperty("tri_cat")]
        public List<EmergencyDepartmentTriageCategory> TriageCategories { get; set; }

        [JsonProperty("wait_times")]
        public List<EmergencyDepartmentWaitingTime> WaitingTimes { get; set; }
    }
}
