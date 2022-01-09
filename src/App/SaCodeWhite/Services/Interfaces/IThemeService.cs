using System.Drawing;

namespace SaCodeWhite.Services
{
    public interface IThemeService
    {
        Theme Current { get; }

        void SetTheme(Theme theme);

        Color PrimaryColor { get; }
    }
}