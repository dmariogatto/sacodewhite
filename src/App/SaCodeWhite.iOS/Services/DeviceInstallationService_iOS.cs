using Foundation;
using SaCodeWhite.Services;
using SaCodeWhite.Shared.Models;
using System;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;
using Xamarin.Essentials.Interfaces;

namespace SaCodeWhite.iOS.Services
{
    [Preserve(AllMembers = true)]
    public class DeviceInstallationService_iOS : IDeviceInstallationService
    {
        private const string ApnToken = nameof(ApnToken);

        private readonly IMainThread _mainThread;
        private readonly ISecureStorage _secureStorage;
        private readonly ILogger _logger;

        public DeviceInstallationService_iOS(
            IMainThread mainThread,
            ISecureStorage secureStorage,
            ILogger logger)
        {
            _mainThread = mainThread;
            _secureStorage = secureStorage;
            _logger = logger;
        }

        public bool NotificationsSupported
            => true;

        public string DeviceId
            => UIDevice.CurrentDevice.IdentifierForVendor.ToString();

        public string Token
        {
            get => _secureStorage.GetAsync(ApnToken).Result;
            set => _secureStorage.SetAsync(ApnToken, value).Wait();
        }

        public Task<bool> CheckAndRequestNotificationsPermissionAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            var notificationOptions =
                UNAuthorizationOptions.Alert |
                UNAuthorizationOptions.Badge |
                UNAuthorizationOptions.Sound;

            UNUserNotificationCenter.Current.RequestAuthorization(
                notificationOptions,
                (granted, error) =>
                {
                    if (granted && error == null)
                    {
                        _mainThread.BeginInvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                        tcs.SetResult(true);
                    }
                    else
                    {
                        tcs.SetResult(false);
                    }
                });

            return tcs.Task;
        }

        public DeviceInstallation GetDeviceInstallation()
        {
            if (!NotificationsSupported)
                throw new Exception(GetNotificationsSupportError());

            if (string.IsNullOrWhiteSpace(Token))
                throw new Exception("Unable to resolve token for APNS");

            var installation = new DeviceInstallation
            {
                InstallationId = DeviceId,
                Platform = PlatformType.Apple,
                PushChannel = Token
            };

            if (string.IsNullOrWhiteSpace(installation.InstallationId))
                throw new Exception("Unable to resolve DeviceId for APNS");

            return installation;
        }

        private string GetNotificationsSupportError()
        {
            if (Token == null)
                return $"This app can support notifications but you must enable this in your settings.";

            return "An error occurred preventing the use of push notifications";
        }
    }
}