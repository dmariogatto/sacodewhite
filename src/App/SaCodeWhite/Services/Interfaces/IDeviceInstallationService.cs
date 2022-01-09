using SaCodeWhite.Shared.Models;
using System.Threading.Tasks;

namespace SaCodeWhite.Services
{
    public interface IDeviceInstallationService
    {
        bool NotificationsSupported { get; }
        string DeviceId { get; }

        string Token { get; set; }

        Task<bool> CheckAndRequestNotificationsPermissionAsync();
        DeviceInstallation GetDeviceInstallation();
    }
}