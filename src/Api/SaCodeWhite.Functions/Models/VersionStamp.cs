using Newtonsoft.Json;
using System;

namespace SaCodeWhite.Functions.Models
{
    public class VersionStamp
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("updated_utc")]
        public DateTime UpdatedUtc { get; set; }

        public string JsonFileName
            => $"{DateTime.MaxValue.Ticks - UpdatedUtc.Ticks:D19}_{Id}.json";

        public override bool Equals(object obj)
            => obj is VersionStamp stamp &&
               Id == stamp.Id &&
               UpdatedUtc == stamp.UpdatedUtc;

        public override int GetHashCode() => HashCode.Combine(Id, UpdatedUtc);
    }
}
