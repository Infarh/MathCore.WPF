using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(double[]), typeof(Point))]
[MarkupExtensionReturnType(typeof(ValuesToPoint))]
public class ValuesToPoint : MultiValueValueConverter
{
    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => 
        vv is null 
            ? null 
            : new Point((double)vv[0], (double)vv[1]);

    protected override object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) =>
        v is null 
            ? null 
            : new object[] { ((Point)v).X, ((Point)v).Y };
}