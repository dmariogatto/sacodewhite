using SaCodeWhite.Services;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using System;
using Xamarin.Essentials;

namespace SaCodeWhite.Droid.Services
{
    [Preserve(AllMembers = true)]
    public class EnvironmentService_Droid : IEnvironmentService
    {
        private readonly ILogger _logger;

        public EnvironmentService_Droid(
            ILogger logger)
        {
            _logger = logger;
        }

        // manual dark mode for the moment
        public bool NativeDarkMode => (int)Build.VERSION.SdkInt >= 29; // Q

        public Theme GetOperatingSystemTheme()
        {
            var uiModeFlags = Platform.AppContext.Resources.Configuration.UiMode & UiMode.NightMask;
            return uiModeFlags switch
            {
                UiMode.NightYes => Theme.Dark,
                UiMode.NightNo => Theme.Light,
                _ => throw new NotSupportedException($"UiMode {uiModeFlags} not supported"),
            };
        }
    }
}