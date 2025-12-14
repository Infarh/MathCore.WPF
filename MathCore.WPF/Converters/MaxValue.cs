using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Возвращает максимальное значение из набора входных значений</summary>
[ValueConversion(typeof(IEnumerable), typeof(object))]
[MarkupExtensionReturnType(typeof(MaxValue))]
public class MaxValue : MarkupExtension, IMultiValueConverter, IValueConverter
{
    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider sp) => this;

    /// <summary>Возвращает максимальное значение из массива значений или Binding.DoNothing при некорректных входах</summary>
    public object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
    {
        if (vv is not { Length: > 0 }) return Binding.DoNothing;

        try
        {
            return vv.Max();
        }
        catch
        {
            return Binding.DoNothing;
        }
    }

    /// <inheritdoc />
    public object[] ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) => throw new NotSupportedException();

    /// <summary>Возвращает максимальный элемент перечисления или Binding.DoNothing</summary>
    public object? Convert(object? v, Type? t, object? p, CultureInfo? c)
    {
        if (v is not IEnumerable enumerable) return Binding.DoNothing;
        try
        {
            return enumerable.Cast<object>().Max();
        }
        catch
        {
            return Binding.DoNothing;
        }
    }

    /// <inheritdoc />
    public object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => throw new NotSupportedException();
}