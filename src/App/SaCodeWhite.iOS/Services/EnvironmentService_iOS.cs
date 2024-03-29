﻿using SaCodeWhite.Services;
using Foundation;
using System;
using UIKit;

namespace SaCodeWhite.iOS.Services
{
    [Preserve(AllMembers = true)]
    public class EnvironmentService_iOS : IEnvironmentService
    {
        private readonly ILogger _logger;

        public EnvironmentService_iOS(ILogger logger)
        {
            _logger = logger;
        }

        // manual dark mode for the moment
        public bool NativeDarkMode => UIDevice.CurrentDevice.CheckSystemVersion(13, 0);

        public Theme GetOperatingSystemTheme()
        {
            var theme = Theme.Light;

            // 'TraitCollection.UserInterfaceStyle' was introduced in iOS 12.0
            if (UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
            {
                try
                {
                    var userInterfaceStyle = UIScreen.MainScreen.TraitCollection.UserInterfaceStyle;
                    switch (userInterfaceStyle)
                    {
                        case UIUserInterfaceStyle.Light:
                            theme = Theme.Light;
                            break;
                        case UIUserInterfaceStyle.Dark:
                            theme = Theme.Dark;
                            break;
                        default:
                            throw new NotSupportedException($"UIUserInterfaceStyle {userInterfaceStyle} not supported");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }

            return theme;
        }
    }
}