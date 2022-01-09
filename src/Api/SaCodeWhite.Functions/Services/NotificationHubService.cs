using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SaCodeWhite.Functions.Models;
using SaCodeWhite.Shared.Models;

namespace SaCodeWhite.Functions.Services
{
    public class NotificationHubService : INotificationHubService
    {
        private readonly NotificationHubClient _hub;
        private readonly ILogger<NotificationHubService> _logger;

        private readonly Dictionary<PlatformType, NotificationPlatform> _installationPlatforms = new Dictionary<PlatformType, NotificationPlatform>
        {
            { PlatformType.Apple, NotificationPlatform.Apns },
            { PlatformType.Google, NotificationPlatform.Fcm }
        };

        public NotificationHubService(
            IOptions<NotificationHubOptions> options,
            ILogger<NotificationHubService> logger)
        {
            _logger = logger;

            if (!string.IsNullOrEmpty(options?.Value?.ConnectionString) &&
                !string.IsNullOrEmpty(options?.Value?.Name))
            {
                _hub = NotificationHubClient.CreateClientFromConnectionString(
                    options.Value.ConnectionString,
                    options.Value.Name);
            }
        }

        public async Task<bool> CreateOrUpdateInstallationAsync(DeviceInstallation deviceInstallation, CancellationToken cancellationToken)
        {
            if (_hub is null)
                return false;

            if (string.IsNullOrWhiteSpace(deviceInstallation?.InstallationId) ||
                string.IsNullOrWhiteSpace(deviceInstallation.PushChannel) ||
                !_installationPlatforms.TryGetValue(deviceInstallation.Platform, out var platform))
                return false;

            var installation = new Installation()
            {
                Platform = platform,
                InstallationId = deviceInstallation.InstallationId,
                PushChannel = deviceInstallation.PushChannel,
                Tags = deviceInstallation.Tags
            };

            try
            {
                await _hub.CreateOrUpdateInstallationAsync(installation, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(NotificationHubService));
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteInstallationByIdAsync(string installationId, CancellationToken cancellationToken)
        {
            if (_hub is null)
                return false;

            if (string.IsNullOrWhiteSpace(installationId))
                return false;

            try
            {
                await _hub.DeleteInstallationAsync(installationId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(NotificationHubService));
                return false;
            }

            return true;
        }

        public async Task<bool> RequestNotificationsAsync(IList<NotificationRequest> notificationRequest, CancellationToken cancellationToken)
        {
            if (_hub is null)
                return false;

            try
            {
                foreach (var request in notificationRequest)
                {
                    var applePayload = request.ToApplePayload();
                    var androidPayload = request.ToAndroidPayload();

                    if (request.Tags.Length == 0)
                    {
                        // This will broadcast to all users registered in the notification hub
                        await SendPlatformNotificationsAsync(androidPayload, applePayload, cancellationToken).ConfigureAwait(false);
                    }
                    else if (request.Tags.Length <= 20)
                    {
                        await SendPlatformNotificationsAsync(androidPayload, applePayload, request.Tags, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        var notificationTasks = request.Tags
                            .Select((value, index) => (value, index))
                            .GroupBy(g => g.index / 20, i => i.value)
                            .Select(tags => SendPlatformNotificationsAsync(androidPayload, applePayload, tags, cancellationToken));

                        await Task.WhenAll(notificationTasks).ConfigureAwait(false);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error sending notification");
                return false;
            }
        }

        private Task SendPlatformNotificationsAsync(string androidPayload, string iOSPayload, CancellationToken token)
        {
            return Task.WhenAll(
                _hub.SendFcmNativeNotificationAsync(androidPayload, token),
                _hub.SendAppleNativeNotificationAsync(iOSPayload, token));
        }

        private Task SendPlatformNotificationsAsync(string androidPayload, string iOSPayload, IEnumerable<string> tags, CancellationToken token)
        {
            return Task.WhenAll(
                _hub.SendFcmNativeNotificationAsync(androidPayload, tags, token),
                _hub.SendAppleNativeNotificationAsync(iOSPayload, tags, token));
        }
    }
}
