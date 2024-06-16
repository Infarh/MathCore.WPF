using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF;

/// <summary>Эффект тени</summary>
/// <remarks>Эффект тени</remarks>
/// <param name="depth">Глубина</param>
[MarkupExtensionReturnType(typeof(DropShadowEffect))]
public class Shadow(double depth, Color color) : MarkupExtension
{
    /// <summary>Эффект тени</summary>
    public Shadow() : this(5, Colors.Black) { }

    public Shadow(Color color) : this(0, color) { }

    /// <summary>Эффект тени</summary>
    public Shadow(Color color, double depth) : this(depth, color) { }

    /// <summary>Глубина тени</summary>
    [ConstructorArgument("depth")]
    public double Depth { get; set; } = depth;

    /// <summary>Алгоритм рендринга</summary>
    public RenderingBias Bias { get; set; } = RenderingBias.Performance;

    /// <summary>Прозрачность</summary>
    public double Opacity { get; set; } = 1;

    /// <summary>Направление</summary>
    public double Direction { get; set; } = 315;

    /// <summary>Цвет</summary>
    [ConstructorArgument("color")]
    public Color Color { get; set; } = color;

    /// <summary>Размытие</summary>
    public double Blur { get; set; } = 5;

    #region Overrides of MarkupExtension

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider sp) =>
        new DropShadowEffect
        {
            BlurRadius    = Blur,
            Color         = Color,
            Direction     = Direction,
            Opacity       = Opacity,
            RenderingBias = Bias,
            ShadowDepth   = Depth
        };

    #endregion  
}