using Android.App;
using Android.Gms.Common;
using Android.Runtime;
using SaCodeWhite.Services;
using SaCodeWhite.Shared.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;
using static Android.Provider.Settings;

namespace SaCodeWhite.Droid.Services
{
    [Preserve(AllMembers = true)]
    public class DeviceInstallationService_Droid : IDeviceInstallationService
    {
        private const string FcmToken = nameof(FcmToken);

        private readonly ISecureStorage _secureStorage;
        private readonly ILogger _logger;

        public DeviceInstallationService_Droid(
            ISecureStorage secureStorage,
            ILogger logger)
        {
            _secureStorage = secureStorage;
            _logger = logger;
        }

        public bool NotificationsSupported
            => GoogleApiAvailability.Instance
                .IsGooglePlayServicesAvailable(Application.Context) == ConnectionResult.Success;

        public string DeviceId
            => Secure.GetString(Application.Context.ContentResolver, Secure.AndroidId);

        public string Token
        {
            get => _secureStorage.GetAsync(FcmToken).Result;
            set => _secureStorage.SetAsync(FcmToken, value).Wait();
        }

        public Task<bool> CheckAndRequestNotificationsPermissionAsync()
            => Task.FromResult(true);

        public DeviceInstallation GetDeviceInstallation()
        {
            if (!NotificationsSupported)
                throw new Exception(GetPlayServicesError());

            if (string.IsNullOrWhiteSpace(Token))
                throw new Exception("Unable to resolve token for FCM");

            var installation = new DeviceInstallation
            {
                InstallationId = DeviceId,
                Platform = PlatformType.Google,
                PushChannel = Token
            };

            if (string.IsNullOrWhiteSpace(installation.InstallationId))
                throw new Exception("Unable to resolve DeviceId for FCM");

            return installation;
        }

        private string GetPlayServicesError()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Application.Context);

            if (resultCode != ConnectionResult.Success)
                return GoogleApiAvailability.Instance.IsUserResolvableError(resultCode) ?
                           GoogleApiAvailability.Instance.GetErrorString(resultCode) :
                           "This device is not supported";

            return "An error occurred preventing the use of push notifications";
        }
    }
}