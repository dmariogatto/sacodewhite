using SaCodeWhite.Functions.Models;
using SaCodeWhite.Shared.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Functions.Services
{
    public interface INotificationHubService
    {
        Task<bool> CreateOrUpdateInstallationAsync(DeviceInstallation deviceInstallation, CancellationToken cancellationToken);
        Task<bool> DeleteInstallationByIdAsync(string installationId, CancellationToken cancellationToken);
        Task<bool> RequestNotificationsAsync(IList<NotificationRequest> notificationRequest, CancellationToken cancellationToken);
    }
}
