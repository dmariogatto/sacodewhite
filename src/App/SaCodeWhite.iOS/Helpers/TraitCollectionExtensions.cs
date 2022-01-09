using SaCodeWhite.UI;
using UIKit;
using Xamarin.Essentials;

namespace SaCodeWhite.iOS
{
    public static class TraitCollectionExtensions
    {
        public static void UpdateTheme(this UITraitCollection current, UITraitCollection previous)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0) &&
                current?.UserInterfaceStyle != previous?.UserInterfaceStyle)
            {
                ThemeManager.OsThemeChanged();
            }
        }
    }
}
