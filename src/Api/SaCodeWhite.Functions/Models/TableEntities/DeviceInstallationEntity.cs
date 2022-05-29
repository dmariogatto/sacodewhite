using SaCodeWhite.Shared.Models;
using System;

namespace SaCodeWhite.Functions.Models
{
    public class DeviceInstallationEntity : BaseTableStoreEntity
    {
        public DeviceInstallationEntity() { }

        public DeviceInstallationEntity(PlatformType platform, string intallationId)
        {
            PartitionKey = platform.ToString();
            RowKey = intallationId;

            CreatedUtc = DateTime.UtcNow;
        }

        public DateTime LastSeenUtc { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}
