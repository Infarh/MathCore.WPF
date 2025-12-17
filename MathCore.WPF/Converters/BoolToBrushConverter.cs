using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Преобразует булево значение в Brush</summary>
[MarkupExtensionReturnType(typeof(BoolToBrushConverter))]
[ValueConversion(typeof(bool?), typeof(Brush))]
public class BoolToBrushConverter : ValueConverter
{
    /// <summary>Brush для true</summary>
    public Brush TrueColorBrush { get; set; } = new SolidColorBrush(Colors.Green);

    /// <summary>Brush для false</summary>
    public Brush FalseColorBrush { get; set; } = new SolidColorBrush(Colors.Orange);

    /// <summary>Brush для null</summary>
    public Brush NullColorBrush { get; set; } = new SolidColorBrush(Colors.Transparent);

    /// <summary>Преобразует булево значение в соответствующий Brush</summary>
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => v switch
    {
        null => NullColorBrush,
        true => TrueColorBrush,
        false => FalseColorBrush,
        _ => Binding.DoNothing
    };

    /// <summary>Обратное преобразование не реализовано и возвращает Binding.DoNothing</summary>
    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => Binding.DoNothing;
}