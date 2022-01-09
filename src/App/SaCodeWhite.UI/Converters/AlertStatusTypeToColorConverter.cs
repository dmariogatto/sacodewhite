using SaCodeWhite.Shared.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Converters
{
    public class AlertStatusTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = Color.Transparent;

            if (value is AlertStatusType alertStatus)
            {
                result = alertStatus switch
                {
                    AlertStatusType.Green => (Color)Application.Current.Resources[Styles.Keys.CodeGreenColor],
                    AlertStatusType.Amber => (Color)Application.Current.Resources[Styles.Keys.CodeAmberColor],
                    AlertStatusType.Red => (Color)Application.Current.Resources[Styles.Keys.CodeRedColor],
                    AlertStatusType.White => (Color)Application.Current.Resources[Styles.Keys.CodeWhiteColor],
                    _ => Color.Transparent
                };
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}