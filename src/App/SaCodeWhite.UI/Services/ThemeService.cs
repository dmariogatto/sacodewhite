using SaCodeWhite.Services;
using System.Drawing;

namespace SaCodeWhite.UI.Services
{
    public class ThemeService : IThemeService
    {
        public Theme Current => ThemeManager.CurrentTheme;
        public void SetTheme(Theme theme) => ThemeManager.ChangeTheme(theme);

        public Color PrimaryColor
            => (Xamarin.Forms.Color)App.Current.Resources[Styles.Keys.PrimaryColor];
    }
}