using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(BoolToBrushConverter))]
[ValueConversion(typeof(bool?), typeof(Brush))]
public class BoolToBrushConverter : ValueConverter
{
    public Brush TrueColorBrush { get; set; } = new SolidColorBrush(Colors.Green);
    public Brush FalseColorBrush { get; set; } = new SolidColorBrush(Colors.Orange);
    public Brush NullColorBrush { get; set; } = new SolidColorBrush(Colors.Transparent);
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => v switch
    {
        null => NullColorBrush,
        true => TrueColorBrush,
        false => FalseColorBrush,
        _ => Binding.DoNothing
    };

    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => Binding.DoNothing;
}