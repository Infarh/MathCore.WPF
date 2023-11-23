using System.Collections.Concurrent;
using System.Windows.Markup;
using System.Windows.Media;

// ReSharper disable InconsistentNaming

namespace MathCore.WPF;

/// <summary>Цвет в формате ARGB</summary>
public class ARGB(byte alpha, byte red, byte green, byte blue) : MarkupExtension
{
    public ARGB() : this(0) { }

    public ARGB(byte Alpha) : this(Alpha, 0, 0, 0) { }

    public ARGB(byte Red, byte Green, byte Blue) : this(0, Red, Green, Blue) { }

    /// <summary>Прозрачность</summary>
    public byte Alpha { get; set; } = alpha;

    /// <summary>Красный</summary>
    public byte Red { get; set; } = red;

    /// <summary>Зелёный</summary>
    public byte Green { get; set; } = green;

    /// <summary>Синий</summary>
    public byte Blue { get; set; } = blue;

    public bool FreezeBrush { get; set; }

    private static readonly ConcurrentDictionary<(Color Color, bool Freeze), SolidColorBrush> __Brushes = new();

    public override object ProvideValue(IServiceProvider sp)
    {
        var color = Color.FromArgb(Alpha, Red, Green, Blue);

        var destination_type = sp.GetDestinationTypeProvider()?.GetDestinationType() ?? typeof(object);

        if (destination_type.IsAssignableFrom(typeof(Color)))
            return color;

        if (destination_type.IsAssignableFrom(typeof(Brush)))
            return __Brushes.GetOrAdd((color, FreezeBrush), c =>
            {
                var brush = new SolidColorBrush(c.Color);
                if (c.Freeze)
                    brush.Freeze();
                return brush;
            });

        throw new NotSupportedException();
    }
}