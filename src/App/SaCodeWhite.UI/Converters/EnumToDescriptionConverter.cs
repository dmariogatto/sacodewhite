using SaCodeWhite.Shared;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Converters
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        public bool AllCaps { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Enum enumValue
                ? AllCaps ? enumValue.GetDescription().ToUpper() : enumValue.GetDescription()
                : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}