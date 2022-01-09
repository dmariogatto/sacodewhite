using Polly;
using Polly.Retry;
using SaCodeWhite.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace SaCodeWhite.Services
{
    public class NotificationService : BaseService, INotificationService
    {
        private const string NotificationDevicePushChannel = nameof(NotificationDevicePushChannel);
        private const string NotificationTags = nameof(NotificationTags);
        private const string NotificationRegisteredUtc = nameof(NotificationRegisteredUtc);
        private const string Comma = ",";

        private readonly HashSet<string> _validTags = new HashSet<string>()
        {
            Shared.Constants.NotificationKeys.CodeWhiteTag
        };

        private readonly IConnectivity _connectivity;
        private readonly IPreferences _preferences;
        private readonly ISecureStorage _secureStorage;

        private readonly IDeviceInstallationService _deviceInstallationService;

        private readonly ISaCodeWhiteApi _api;

        private readonly AsyncRetryPolicy _retryPolicy;

        private readonly SemaphoreSlim _refreshSemaphore = new SemaphoreSlim(1, 1);

        private readonly HashSet<string> _tags;

        public NotificationService(
            IConnectivity connectivity,
            IPreferences preferences,
            ISecureStorage secureStorage,
            IDeviceInstallationService deviceInstallationService,
            ISaCodeWhiteApi api,
            IRetryPolicyFactory retryPolicyFactory,
            ILogger logger) : base(logger)
        {
            _connectivity = connectivity;
            _preferences = preferences;
            _secureStorage = secureStorage;

            _deviceInstallationService = deviceInstallationService;

            _api = api;

            _retryPolicy =
                retryPolicyFactory.GetNetRetryPolicy()
                    .WaitAndRetryAsync
                    (
                        retryCount: 2,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    );

            _tags = new HashSet<string>(GetSavedTags());

            foreach (var t in _tags.Except(_validTags).ToList())
                RemoveTag(t);
        }

        public DateTime RegisteredUtc
        {
            get => _preferences.Get(NotificationRegisteredUtc, DateTime.MinValue);
            private set
            {
                if (value == DateTime.MinValue)
                    _preferences.Remove(NotificationRegisteredUtc);
                else
                    _preferences.Set(NotificationRegisteredUtc, value);
            }
        }

        public async Task<bool> RefreshDeviceRegistrationAsync()
        {
            var success = false;

            await _refreshSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                    return success;

                var pushChannel = await _secureStorage.GetAsync(NotificationDevicePushChannel).ConfigureAwait(false);
                var device = _deviceInstallationService.GetDeviceInstallation();
                var needsRefresh = RegisteredUtc > DateTime.MinValue && RegisteredUtc < DateTime.UtcNow.AddDays(-30);

                if (needsRefresh || pushChannel != device.PushChannel || !_tags.SetEquals(GetSavedTags()))
                {
                    if (_tags.Any())
                    {
                        device.Tags.AddRange(_tags);

                        await _retryPolicy.ExecuteAsync(ct => _api.RegisterDeviceAsync(Constants.ApiKeyRegisterDevice, device, ct), CancellationToken.None).ConfigureAwait(false);
                        await _secureStorage.SetAsync(NotificationDevicePushChannel, device.PushChannel).ConfigureAwait(false);
                        RegisteredUtc = DateTime.UtcNow;

                        success = true;
                    }
                    else if (!string.IsNullOrEmpty(pushChannel))
                    {
                        await _retryPolicy.ExecuteAsync(ct => _api.DeleteDeviceAsync(Constants.ApiKeyDeleteDevice, device.Platform, device.InstallationId, ct), CancellationToken.None).ConfigureAwait(false);

                        _secureStorage.Remove(NotificationDevicePushChannel);
                        _secureStorage.Remove(NotificationTags);
                        RegisteredUtc = DateTime.MinValue;

                        success = true;
                    }

                    SaveTags();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                _refreshSemaphore.Release();
            }

            return success;
        }

        public bool HasTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            return _tags.Contains(tag);
        }

        public string[] GetTags()
            => _tags.ToArray();

        public void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag) && !_validTags.Contains(tag))
                return;

            _tags.Add(tag);
        }

        public void RemoveTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return;

            _tags.Remove(tag);
        }

        private HashSet<string> GetSavedTags()
        {
            var csv = _preferences.Get(NotificationTags, string.Empty);
            return new HashSet<string>(csv.Split(Comma, StringSplitOptions.RemoveEmptyEntries));
        }

        private void SaveTags()
        {
            var csv = string.Join(Comma, _tags);

            if (string.IsNullOrEmpty(csv))
                _preferences.Remove(NotificationTags);
            else
                _preferences.Set(NotificationTags, csv);
        }
    }
}