using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Firebase;
using SaCodeWhite.Services;
using SaCodeWhite.UI;
using System.Linq;
using Xamarin.Forms;

[assembly: ResolutionGroupName("SaCodeWhite.Effects")]

namespace SaCodeWhite.Droid
{
    [Activity(
        Label = "SA CodeWhite",
        Icon = "@mipmap/icon",
        RoundIcon = "@mipmap/icon_round",
        Theme = "@style/SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTask,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        Exported = true)]
    [IntentFilter(
        new[] { Xamarin.Essentials.Platform.Intent.ActionAppAction },
        Categories = new[] { Intent.CategoryDefault })]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const string CodeWhiteChannelId = "sacodewhite_code_white_notifications";

        private static App FormsApp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(FormsApp ??= new App());

            SetTheme(Resource.Style.MainTheme);

            CreateNotificationChannels();

            if (IoC.Resolve<IDeviceInstallationService>().NotificationsSupported)
            {
                FirebaseApp.InitializeApp(this);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            Xamarin.Essentials.Platform.OnResume(this);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            Xamarin.Essentials.Platform.OnNewIntent(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void CreateNotificationChannels()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);

                foreach (var nc in notificationManager.NotificationChannels.Where(i => i.Id != CodeWhiteChannelId).ToList())
                    notificationManager.DeleteNotificationChannel(nc.Id);

                var codeWhiteChannel = new NotificationChannel(CodeWhiteChannelId, "SA CodeWhite", NotificationImportance.Default);
                notificationManager.CreateNotificationChannel(codeWhiteChannel);
            }
        }
    }
}