using MvvmHelpers.Commands;
using Plugin.StoreReview.Abstractions;
using SaCodeWhite.Services;
using SaCodeWhite.Shared.Localisation;
using SaCodeWhite.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace SaCodeWhite.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IDeviceInfo _deviceInfo;
        private readonly IAppInfo _appInfo;
        private readonly IEmail _email;

        private readonly IStoreReview _storeReview;

        private readonly INotificationService _notificationService;
        private readonly IThemeService _themeService;

        public SettingsViewModel(
            IDeviceInfo deviceInfo,
            IAppInfo appInfo,
            IEmail email,
            IStoreReview storeReview,
            INotificationService notificationService,
            IThemeService themeService,
            IBvmConstructor bvmConstructor) : base(bvmConstructor)
        {
            Title = Resources.Settings;

            _deviceInfo = deviceInfo;
            _appInfo = appInfo;
            _email = email;
            _storeReview = storeReview;

            _themeService = themeService;

            _notificationService = notificationService;

            SendFeedbackCommand = new AsyncCommand(SendFeedbackAsync);
            RateAppCommand = new Command(RateApp);
            OpenAppSettingsCommand = new Command(_appInfo.ShowSettingsUI);
        }

        #region Overrides
        public override void OnAppearing()
        {
            base.OnAppearing();

            TrackEvent(AppCenterEvents.PageView.SettingsView);
        }
        #endregion

        public string Version => _appInfo.VersionString;
        public string Build => _appInfo.BuildString;

        public long LogDataSize => Logger.LogInBytes();

        public Theme AppTheme
        {
            get => AppPrefs.AppTheme;
            set
            {
                if (AppTheme != value)
                {
                    _themeService.SetTheme(value);
                    OnPropertyChanged(nameof(AppTheme));
                }
            }
        }

        public bool CodeWhiteNotifications
        {
            get => _notificationService.HasTag(Shared.Constants.NotificationKeys.CodeWhiteTag);
            set
            {
                if (value)
                    _notificationService.AddTag(Shared.Constants.NotificationKeys.CodeWhiteTag);
                else
                    _notificationService.RemoveTag(Shared.Constants.NotificationKeys.CodeWhiteTag);

                OnPropertyChanged(nameof(CodeWhiteNotifications));
            }
        }

        public AsyncCommand SendFeedbackCommand { get; private set; }
        public Command RateAppCommand { get; private set; }
        public Command OpenAppSettingsCommand { get; private set; }

        private async Task SendFeedbackAsync()
        {
            try
            {
                var builder = new StringBuilder();
                builder.AppendLine($"App: {_appInfo.VersionString} | {_appInfo.BuildString}");
                builder.AppendLine($"OS: {_deviceInfo.Platform} | {_deviceInfo.VersionString}");
                builder.AppendLine($"Device: {_deviceInfo.Manufacturer} | {_deviceInfo.Model}");
                builder.AppendLine();
                builder.AppendLine(string.Format(Resources.ItemComma, Resources.AddYourMessageBelow));
                builder.AppendLine("----");
                builder.AppendLine();

                var message = new EmailMessage
                {
                    Subject = string.Format(Resources.FeedbackSubjectItem, _deviceInfo.Platform),
                    Body = builder.ToString(),
                    To = new List<string>(1) { Constants.Email },
                };

                await _email.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                DialogService.Alert(Resources.EmailDirectly, Resources.UnableToSendEmail, Resources.OK);
            }
        }

        private void RateApp()
        {
            var id = Constants.AppId;

            if (!string.IsNullOrEmpty(id))
                _storeReview.OpenStoreReviewPage(id);
        }
    }
}