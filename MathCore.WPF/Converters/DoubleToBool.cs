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

        protected DoubleToBool([CanBeNull] Func<double, bool?>? to = null, [CanBeNull] Func<bool?, double>? from = null)
        {
            _Convert = to ?? Convert;
            _ConvertBack = from ?? ConvertBack;
        }

        protected virtual bool? Convert(double v) => throw new NotImplementedException("�� �������� ����� ������� �������������� ��������");

        protected virtual double ConvertBack(bool? v) => throw new NotSupportedException("�������� �������������� �� ��������������");

        /// <inheritdoc />
        protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) =>
            DoubleValueConverter.TryConvertToDouble(p, out var P)
                ? _Convert(P)
                : v is null
                    ? null
                    : DoubleValueConverter.TryConvertToDouble(v, out var V) ? (object?) _Convert(V) : V;

        /// <inheritdoc />
        protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => 
            v is null 
                ? (double?)null 
                : _ConvertBack((bool)v);
    }
}