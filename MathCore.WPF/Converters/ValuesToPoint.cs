using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double[]), typeof(Point))]
    [MarkupExtensionReturnType(typeof(ValuesToPoint))]
    public class ValuesToPoint : MultiValueValueConverter
    {
        [CanBeNull]
        protected override object? Convert([CanBeNull] object[]? vv, Type? t, object? p, CultureInfo? c) => 
            vv is null 
                ? (object?) null 
                : new Point((double)vv[0], (double)vv[1]);

        [CanBeNull]
        protected override object[]? ConvertBack([CanBeNull] object? v, Type[]? tt, object? p, CultureInfo? c) =>
            v is null 
                ? null 
                : new object[] { ((Point)v).X, ((Point)v).Y };
    }
}