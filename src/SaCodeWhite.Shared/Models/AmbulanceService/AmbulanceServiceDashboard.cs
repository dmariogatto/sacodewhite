using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace SaCodeWhite.Shared.Models.AmbulanceService
{
    [DebuggerDisplay("{HospitalCode}")]
    public class AmbulanceServiceDashboard : AmbulanceStatus
    {
        [JsonProperty("bed_stat")]
        public AmbulanceInpatientBedStatus InpatientBedStatus { get; set; }

        [JsonProperty("tri_cats")]
        public List<AmbulanceTriageCategory> TriageCategories { get; set; }

        [JsonProperty("sp_bed_stat")]
        public AmbulanceSpecialityBedStatus SpecialityBedStatus { get; set; }
    }
}
