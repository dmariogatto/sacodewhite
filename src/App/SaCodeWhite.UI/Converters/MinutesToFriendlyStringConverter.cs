using System;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Converters
{
    public class MinutesToFriendlyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ts = TimeSpan.FromMinutes(System.Convert.ToDouble(value));

            var hours = ts.Days * 24 + ts.Hours;
            var minutes = ts.Minutes + (ts.Seconds >= 30 ? 1 : 0);

            var sb = new StringBuilder();

            if (hours > 0)
                sb.AppendFormat(Shared.Localisation.Resources.ItemH, hours);

            if (minutes > 0)
            {
                if (sb.Length > 0) sb.Append(Shared.Localisation.Resources.Space);
                sb.AppendFormat(Shared.Localisation.Resources.ItemM, minutes);
            }

            if (sb.Length == 0)
                sb.AppendFormat(Shared.Localisation.Resources.ItemM, 0);

            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}