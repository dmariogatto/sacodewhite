using System.Globalization;

namespace SaCodeWhite.Services
{
    public interface ILocalise
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);

        bool Is24Hour { get; }
    }

}