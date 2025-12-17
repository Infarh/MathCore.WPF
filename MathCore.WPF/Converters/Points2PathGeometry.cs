using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Linq;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразует массив точек в PathGeometry</summary>
[ValueConversion(typeof(Point[]), typeof(PathGeometry))]
[MarkupExtensionReturnType(typeof(Points2PathGeometry))]
public class Points2PathGeometry : ValueConverter
{
    /// <summary>Преобразует массив точек в PathGeometry, соединяя точки последовательными отрезками</summary>
    protected override object? Convert(object? v, Type? t, object? p, System.Globalization.CultureInfo? c) =>
        v is Point[] and [var start, .. { Length: > 0 } tail]
            ? new PathGeometry
            {
                Figures = { new(start, tail.Select(pt => new LineSegment(pt, true)), false) }
            }
            : null;
}