using System.Globalization;
using System.Windows.Data;

// ReSharper disable VirtualMemberNeverOverridden.Global

namespace MathCore.WPF.Converters.Base;

/// <summary>Базовый класс для конвертеров double -> bool</summary>
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

    /// <summary>Преобразует число в логическое значение</summary>
    protected virtual bool? Convert(double v) => throw new NotImplementedException("Не определён метод прямого преобразования величины");

    /// <summary>Обратное преобразование логического значения в число</summary>
    protected virtual double ConvertBack(bool? v) => throw new NotSupportedException("Обратное преобразование не поддерживается");

    /// <summary>Проверяет параметр p на число и использует его, иначе использует входное значение</summary>
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c)
    {
        // параметр имеет приоритет если он числовой
        if (DoubleValueConverter.TryConvertToDouble(p, c, out var P))
            return _Convert(P);

        if (v is null) return null;

        return DoubleValueConverter.TryConvertToDouble(v, c, out var V) ? _Convert(V) : Binding.DoNothing;
    }

    /// <summary>Обратное преобразование с безопасной проверкой типов</summary>
    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) =>
        v is null ? null : (v is bool b ? _ConvertBack(b) : Binding.DoNothing);
}