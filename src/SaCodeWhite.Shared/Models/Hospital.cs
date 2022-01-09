using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace SaCodeWhite.Shared.Models
{
    [DebuggerDisplay("{Code}")]
    public class Hospital
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("suburb")]
        public string Suburb { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("postcode")]
        public string Postcode { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("fax")]
        public string Fax { get; set; }
        [JsonProperty("website")]
        public string Website { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("alt_codes")]
        public List<string> AlternateCodes { get; set; }
    }
}
