using System.Collections;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(IEnumerable), typeof(object))]
public class MinValue : MarkupExtension, IMultiValueConverter, IValueConverter
{
    public override object ProvideValue(IServiceProvider sp) => this;

    public object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => vv?.Min();

    public object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) => throw new NotSupportedException();

    public object? Convert(object? v, Type? t, object? p, CultureInfo? c) => (v as IEnumerable)?.Cast<object>().Min();

    public object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => throw new NotSupportedException();
}