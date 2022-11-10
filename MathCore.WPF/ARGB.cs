using System.Collections.Concurrent;
using System.Windows.Markup;
using System.Windows.Media;

// ReSharper disable InconsistentNaming

namespace MathCore.WPF;

/// <summary>Цвет в формате ARGB</summary>
public class ARGB : MarkupExtension
{
    /// <summary>Прозрачность</summary>
    public byte Alpha { get; set; }

    /// <summary>Красный</summary>
    public byte Red { get; set; }

    /// <summary>Зелёный</summary>
    public byte Green { get; set; }

    /// <summary>Синий</summary>
    public byte Blue { get; set; }

    public bool FreezeBrush { get; set; }

    public ARGB() { }

    public ARGB(byte Alpha) => this.Alpha = Alpha;

    public ARGB(byte Red, byte Green, byte Blue)
    {
        this.Red   = Red;
        this.Green = Green;
        this.Blue  = Blue;
    }

    public ARGB(byte Alpha, byte red, byte green, byte blue)
        : this(Alpha)
    {
        Red   = red;
        Green = green;
        Blue  = blue;
    }

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