using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(Point[]), typeof(PathGeometry))]
[MarkupExtensionReturnType(typeof(Points2PathGeometry))]
public class Points2PathGeometry : ValueConverter
{
    #region IValueConverter Members

    protected override object? Convert(object? v, Type? t, object? p, System.Globalization.CultureInfo? c) =>
        v is Point[] and [var start, .. { Length: > 0 } tail]
            ? new PathGeometry
            {
                Figures = { new(start, tail.Select(p => new LineSegment(p, true)), false) }
            }
            : null;

    #endregion
}