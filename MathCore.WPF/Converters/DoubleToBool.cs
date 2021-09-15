using System;
using System.Globalization;
using System.Windows.Data;
using MathCore.Annotations;
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double?), typeof(bool?))]
    public abstract class DoubleToBool : ValueConverter
    {
        private readonly Func<double, bool?> _Convert;

        private readonly Func<bool?, double> _ConvertBack;

        protected DoubleToBool(Func<double, bool?>? to = null, Func<bool?, double>? from = null)
        {
            _Convert = to ?? Convert;
            _ConvertBack = from ?? ConvertBack;
        }

        protected virtual bool? Convert(double v) => throw new NotImplementedException("Не определён метод прямого преобразования величины");

        protected virtual double ConvertBack(bool? v) => throw new NotSupportedException("Обратное преобразование не поддерживается");

        /// <inheritdoc />
        protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) =>
            DoubleValueConverter.TryConvertToDouble(p, out var P)
                ? _Convert(P)
                : v is null
                    ? null
                    : DoubleValueConverter.TryConvertToDouble(v, out var V) ? _Convert(V) : V;

        /// <inheritdoc />
        protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => 
            v is null 
                ? null 
                : _ConvertBack((bool)v);
    }
}