using Foundation;
using SaCodeWhite.iOS.Services;
using SaCodeWhite.Services;
using SaCodeWhite.UI;
using SaCodeWhite.UI.Services;
using System;
using System.Text.RegularExpressions;
using UIKit;
using Xamarin.Forms;

[assembly: ResolutionGroupName("SaCodeWhite.Effects")]

namespace SaCodeWhite.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            IoC.RegisterSingleton<ILocalise, LocaliseService_iOS>();
            IoC.RegisterSingleton<IDeviceInstallationService, DeviceInstallationService_iOS>();
            IoC.RegisterSingleton<IEnvironmentService, EnvironmentService_iOS>();
            IoC.RegisterSingleton<IRetryPolicyService, RetryPolicyService_iOS>();

            global::Xamarin.Forms.Forms.Init();

            var formsApp = new App();
            LoadApplication(formsApp);

            return base.FinishedLaunching(app, options);
        }

        public override void PerformActionForShortcutItem(UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
        {
            Xamarin.Essentials.Platform.PerformActionForShortcutItem(application, shortcutItem, completionHandler);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            var handle = UIDevice.CurrentDevice.CheckSystemVersion(13, 0)
                ? BitConverter.ToString(deviceToken.ToArray()).Replace("-", string.Empty)
                : Regex.Replace(deviceToken.ToString(), "[^0-9a-zA-Z]+", string.Empty);
            IoC.Resolve<IDeviceInstallationService>().Token = handle.ToUpper();
        }
    }
}
