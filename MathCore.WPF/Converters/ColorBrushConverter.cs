using System.Collections.Concurrent;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Media;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(ColorBrushConverter))]
public class ColorBrushConverter : ValueConverter
{
    private static readonly ConcurrentDictionary<Color, SolidColorBrush> __Brushes = new();

    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) =>
        v is Color color
            ? __Brushes.GetOrAdd(color, brush_color =>
            {
                var brush = new SolidColorBrush(brush_color);
                brush.Freeze();
                return brush;
            })
            : null;

    protected override object? ConvertBack(object? v, Type t, object? p, CultureInfo c) =>
        v is SolidColorBrush { Color: var color }
            ? color
            : null;
}
