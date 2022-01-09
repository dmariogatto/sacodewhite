using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Converters
{
    public class MultiConverterTyped<TIn, TOut> : IMultiValueConverter
    {
        private readonly Func<TIn[], object, CultureInfo, TOut> _convertFullFunc;
        private readonly Func<TOut, object, CultureInfo, TIn[]> _convertFullBackFunc;

        private readonly Func<TIn[], TOut> _convertSimpleFunc;
        private readonly Func<TOut, TIn[]> _convertBackSimpleFunc;

        public MultiConverterTyped(Func<TIn[], object, CultureInfo, TOut> convertFunc)
        {
            _convertFullFunc = convertFunc;
        }

        public MultiConverterTyped(
            Func<TIn[], object, CultureInfo, TOut> convertFunc,
            Func<TOut, object, CultureInfo, TIn[]> convertBackFunc)
        {
            _convertFullFunc = convertFunc;
            _convertFullBackFunc = convertBackFunc;
        }

        public MultiConverterTyped(Func<TIn[], TOut> convertFunc)
        {
            _convertSimpleFunc = convertFunc;
        }

        public MultiConverterTyped(
            Func<TIn[], TOut> convertFunc,
            Func<TOut, TIn[]> convertBackFunc)
        {
            _convertSimpleFunc = convertFunc;
            _convertBackSimpleFunc = convertBackFunc;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var typedValues = values?.OfType<TIn>()?.ToArray();

            return _convertFullFunc is not null
                ? _convertFullFunc.Invoke(typedValues, parameter, culture)
                : _convertSimpleFunc.Invoke(typedValues);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var typedValue = value is TOut ? (TOut)value : default(TOut);

            if (_convertFullBackFunc is not null)
                return _convertFullBackFunc.Invoke(typedValue, parameter, culture)?.OfType<object>()?.ToArray();

            if (_convertBackSimpleFunc is not null)
                return _convertBackSimpleFunc.Invoke(typedValue)?.OfType<object>()?.ToArray();

            throw new NotImplementedException();
        }
    }
}