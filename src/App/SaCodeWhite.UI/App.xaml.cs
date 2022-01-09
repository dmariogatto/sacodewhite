using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.StoreReview;
using SaCodeWhite.Services;
using SaCodeWhite.Shared.Models;
using SaCodeWhite.UI.Services;
using SaCodeWhite.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Device = Xamarin.Forms.Device;

namespace SaCodeWhite.UI
{
    public partial class App : Xamarin.Forms.Application
    {
        public const string Scheme = "adl-codewhite";

        static App()
        {
            IoC.RegisterSingleton<IDialogService, DialogService>();
            IoC.RegisterSingleton<INavigationService, TabbedNavigationService>();
            IoC.RegisterSingleton<IThemeService, ThemeService>();

            AppActions.OnAppAction += (sender, args) =>
            {
                const string uriFormat = "{0}://{1}";

                if (Current is App app)
                {
                    var id = args.AppAction.Id;
                    app.SendOnAppLinkRequestReceived(new Uri(string.Format(uriFormat, Scheme, args.AppAction.Id.ToLower())));
                    IoC.Resolve<ILogger>().Event(AppCenterEvents.Action.AppAction);
                }
            };
        }

        public App()
        {
            Device.SetFlags(new string[] { });

            InitializeComponent();

            var appCenterId = Constants.AppCenterSecret;
            if (!string.IsNullOrEmpty(appCenterId) && Guid.TryParse(appCenterId, out var guid) && guid != Guid.Empty)
                AppCenter.Start(appCenterId, typeof(Analytics), typeof(Crashes));

            VersionTracking.Track();
            UpdateDayCount();

            Current.On<iOS>().SetHandleControlUpdatesOnMainThread(true);
            Current.On<Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            var localise = IoC.Resolve<ILocalise>();
            var culture = localise.GetCurrentCultureInfo();
            culture.DateTimeFormat.SetTimePatterns(localise.Is24Hour);
            localise.SetLocale(culture);

            IoC.Resolve<INavigationService>().Init();
            _ = AppReviewRequestAsync();
        }

        protected override void OnStart()
        {
            // Handle when your app starts

            ThemeManager.LoadTheme();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var setupAppShortcutsTask = SetupAppShortcutsAsync();
            var refreshNotificationsTask = IoC.Resolve<INotificationService>().RefreshDeviceRegistrationAsync();

            Task.WhenAll(setupAppShortcutsTask, refreshNotificationsTask).Wait();

            sw.Stop();

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"{nameof(OnSleep)}: completed in {sw.ElapsedMilliseconds}ms");
#endif
        }

        protected override void OnResume()
        {
            // Handle when your app resumes

            UpdateDayCount();
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);

            if (!uri.Scheme.Equals(Scheme, StringComparison.OrdinalIgnoreCase))
                return;

            var navService = IoC.Resolve<INavigationService>();
            var task = Task.CompletedTask;

            if (Enum.TryParse<DashboardType>(uri.Host, true, out var dashboard) && dashboard != DashboardType.Unknown)
            {
                task = navService.PopToRootAsync(false);

                switch (dashboard)
                {
                    case DashboardType.AmbulanceService:
                        task = navService.PopToRootAsync(false)
                            .ContinueWith(
                                async t => await navService.NavigateToAsync<AmbulanceServiceViewModel>(),
                                TaskScheduler.FromCurrentSynchronizationContext());
                        break;
                    case DashboardType.EmergencyDepartment:
                        task = navService.PopToRootAsync(false)
                            .ContinueWith(
                                async t => await navService.NavigateToAsync<EmergencyDepartmentViewModel>(),
                                TaskScheduler.FromCurrentSynchronizationContext());
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateDayCount()
        {
            var prefService = IoC.Resolve<IAppPreferences>();
            var today = DateTime.Now.Date;
            if (prefService.LastDateOpened < today)
            {
                prefService.LastDateOpened = today;
                prefService.DayCount++;
            }
        }

        private async Task AppReviewRequestAsync()
        {
            var appPrefs = IoC.Resolve<IAppPreferences>();

            if (appPrefs.ReviewRequested)
                return;

            try
            {
                var metroService = IoC.Resolve<ICodeWhiteService>();

                if (appPrefs.DayCount >= 14)
                {
                    var testMode = false;
#if DEBUG
                    testMode = true;
#endif
                    await CrossStoreReview.Current.RequestReview(testMode);
                    appPrefs.ReviewRequested = true;
                    IoC.Resolve<ILogger>().Event(AppCenterEvents.Action.ReviewRequested, new Dictionary<string, string>(1)
                    {
                        { nameof(DeviceInfo.Platform).ToLower(), DeviceInfo.Platform.ToString() }
                    });
                }
            }
            catch (Exception ex)
            {
                IoC.Resolve<ILogger>().Error(ex);
            }
        }

        private async Task SetupAppShortcutsAsync()
        {
            try
            {
                await AppActions.SetAsync(
                    new AppAction(DashboardType.AmbulanceService.ToString(), Shared.Localisation.Resources.Ambulance, icon: "ambulance_shortcut"),
                    new AppAction(DashboardType.EmergencyDepartment.ToString(), Shared.Localisation.Resources.ED, icon: "hospital_shortcut"))
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}