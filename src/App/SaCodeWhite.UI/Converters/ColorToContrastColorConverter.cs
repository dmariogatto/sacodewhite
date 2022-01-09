using System;
using System.Globalization;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Converters
{
    public class ColorToContrastColorConverter : IValueConverter
    {
        public int Threashold { get; set; } = 130;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = Color.Black;

            if (value is Color background)
            {
                var brightness = PerceivedBrightness(background);
                result = brightness > Threashold ? Color.Black : Color.White;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private int PerceivedBrightness(Color c)
        {
            return (int)Math.Sqrt(
                Math.Pow(c.R * 255, 2) * 0.299 +
                Math.Pow(c.G * 255, 2) * 0.587 +
                Math.Pow(c.B * 255, 2) * 0.114);
        }
    }
}