using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Возвращает минимальное значение из набора входных значений</summary>
[ValueConversion(typeof(IEnumerable), typeof(object))]
[MarkupExtensionReturnType(typeof(MinValue))]
public class MinValue : MarkupExtension, IMultiValueConverter, IValueConverter
{
    public override object ProvideValue(IServiceProvider sp) => this;

    /// <summary>Возвращает минимальное значение из массива или Binding.DoNothing</summary>
    public object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
    {
        if (vv is not { Length: > 0 }) return Binding.DoNothing;
        try
        {
            return vv.Min();
        }
        catch
        {
            return Binding.DoNothing;
        }
    }

    public object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) => throw new NotSupportedException();

    /// <summary>Возвращает минимальный элемент перечисления или Binding.DoNothing</summary>
    public object? Convert(object? v, Type? t, object? p, CultureInfo? c)
    {
        if (v is not IEnumerable enumerable) return Binding.DoNothing;
        try
        {
            return enumerable.Cast<object>().Min();
        }
        catch
        {
            return Binding.DoNothing;
        }
    }

    public object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => throw new NotSupportedException();
}