using System;
using System.Globalization;
using System.Windows.Data;

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double?), typeof(bool?))]
    public abstract class DoubleToBoolConverter : ValueConverter
    {
        private readonly Func<double, bool?> _Convert;
        private readonly Func<bool?, double> _ConvertBack;

        protected DoubleToBoolConverter(Func<double, bool?> to = null, Func<bool?, double> from = null)
        {
            _Convert = to ?? Convert;
            _ConvertBack = from ?? ConvertBack;
        }

        protected virtual bool? Convert(double v) => throw new NotImplementedException("Неопределён метод прямого преобразования величины");

        protected virtual double ConvertBack(bool? v) => throw new NotSupportedException("Обратное преобразование не поддерживается");

        /// <inheritdoc />
        protected override object Convert(object v, Type t, object p, CultureInfo c)
        {
            return DoubleValueConverter.TryConvertToDouble(p, out var P)
                ? _Convert(P)
                : (v == null
                    ? null
                    : (DoubleValueConverter.TryConvertToDouble(v, out var V) ? (object) _Convert(V) : V));
        }

        /// <inheritdoc />
        protected override object ConvertBack(object v, Type t, object p, CultureInfo c) => v == null ? (double?)null : _ConvertBack((bool)v);
    }
}