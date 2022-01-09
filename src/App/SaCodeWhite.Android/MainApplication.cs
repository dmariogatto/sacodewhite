using SaCodeWhite.Droid.Services;
using SaCodeWhite.Services;
using SaCodeWhite.UI.Services;
using Android.App;
using Android.Runtime;
using System;

namespace SaCodeWhite.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
[Application(Debuggable = false)]
#endif
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            IoC.RegisterSingleton<IDeviceInstallationService, DeviceInstallationService_Droid>();
            IoC.RegisterSingleton<ILocalise, LocaliseService_Droid>();
            IoC.RegisterSingleton<IEnvironmentService, EnvironmentService_Droid>();
            IoC.RegisterSingleton<IRendererService, RendererService_Droid>();
            IoC.RegisterSingleton<IRetryPolicyService, RetryPolicyService_Droid>();
        }
    }
}