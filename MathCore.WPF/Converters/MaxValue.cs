using System.Collections;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(IEnumerable), typeof(object))]
public class MaxValue : MarkupExtension, IMultiValueConverter, IValueConverter
{
    public override object ProvideValue(IServiceProvider sp) => this;

    public object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => vv is not { Length: > 0 } ? null : vv.Max();

    public object[] ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) => throw new NotSupportedException();

    public object? Convert(object? v, Type? t, object? p, CultureInfo? c) => (v as IEnumerable)?.Cast<object>().Max();

    public object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => throw new NotSupportedException();
}