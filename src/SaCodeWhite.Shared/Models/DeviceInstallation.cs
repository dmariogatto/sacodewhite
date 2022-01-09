using Newtonsoft.Json;
using System.Collections.Generic;

namespace SaCodeWhite.Shared.Models
{
    public class DeviceInstallation
    {
        [JsonProperty("installation_id")]
        public string InstallationId { get; set; }

        [JsonProperty("platform")]
        public PlatformType Platform { get; set; }

        [JsonProperty("push_channel")]
        public string PushChannel { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = new List<string>();
    }
}
