using System.Windows.Markup;
using System.Windows.Media;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global

// ReSharper disable UnusedMember.Global

namespace MathCore.WPF;

/// <summary>Трансформация масштабирования</summary>
/// <remarks>Трансформация масштабирования</remarks>
/// <param name="x">Масштаб по X</param>
/// <param name="y">Масштаб по Y</param>
[MarkupExtensionReturnType(typeof(ScaleTransform))]
public class Scale(double x, double y) : MarkupExtension
{
    /// <summary>Трансформация масштабирования</summary>
    public Scale() : this(1, 1) { }

    /// <summary>Масштаб по X</summary>
    public double X { get; set; } = x;     

    /// <summary>Масштаб по Y</summary>
    public double Y { get; set; } = y;

    /// <summary>X-центра</summary>
    public double X0 { get; set; }

    /// <summary>Y-центра</summary>
    public double Y0 { get; set; }

    #region Overrides of MarkupExtension

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider sp) => new ScaleTransform { ScaleX = X, ScaleY = Y, CenterX = X0, CenterY = Y0 };

    #endregion 
}