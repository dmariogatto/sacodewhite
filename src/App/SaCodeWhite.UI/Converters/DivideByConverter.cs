using System;
using System.Globalization;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Converters
{
    public class DivideByConverter : IValueConverter
    {
        public double? MaxValue { get; set; }
        public double? MinValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = System.Convert.ToDouble(value) / System.Convert.ToDouble(parameter);

            if (MaxValue.HasValue)
                result = Math.Min(result, MaxValue.Value);

            if (MinValue.HasValue)
                result = Math.Max(result, MinValue.Value);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter);

            if (MaxValue.HasValue)
                result = Math.Min(result, MaxValue.Value);

            if (MinValue.HasValue)
                result = Math.Max(result, MinValue.Value);

            return result;
        }
    }
}