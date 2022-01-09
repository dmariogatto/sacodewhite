using System;
using System.Globalization;
using Xamarin.Forms;

namespace SaCodeWhite.UI.Converters
{
    public class ConverterTyped<TIn, TOut> : IValueConverter
    {
        private readonly Func<TIn, object, CultureInfo, TOut> _convertFullFunc;
        private readonly Func<TOut, object, CultureInfo, TIn> _convertFullBackFunc;

        private readonly Func<TIn, TOut> _convertSimpleFunc;
        private readonly Func<TOut, TIn> _convertBackSimpleFunc;

        public ConverterTyped(Func<TIn, object, CultureInfo, TOut> convertFunc)
        {
            _convertFullFunc = convertFunc;
        }

        public ConverterTyped(
            Func<TIn, object, CultureInfo, TOut> convertFunc,
            Func<TOut, object, CultureInfo, TIn> convertBackFunc)
        {
            _convertFullFunc = convertFunc;
            _convertFullBackFunc = convertBackFunc;
        }

        public ConverterTyped(Func<TIn, TOut> convertFunc)
        {
            _convertSimpleFunc = convertFunc;
        }

        public ConverterTyped(
            Func<TIn, TOut> convertFunc,
            Func<TOut, TIn> convertBackFunc)
        {
            _convertSimpleFunc = convertFunc;
            _convertBackSimpleFunc = convertBackFunc;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typedValue = value is TIn ? (TIn)value : default(TIn);

            return _convertFullFunc is not null
                ? _convertFullFunc.Invoke(typedValue, parameter, culture)
                : _convertSimpleFunc.Invoke(typedValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typedValue = value is TOut ? (TOut)value : default(TOut);

            if (_convertFullBackFunc is not null)
                return _convertFullBackFunc.Invoke(typedValue, parameter, culture);

            if (_convertBackSimpleFunc is not null)
                return _convertBackSimpleFunc.Invoke(typedValue);

            throw new NotImplementedException();
        }
    }
}