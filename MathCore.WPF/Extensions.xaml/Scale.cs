using System.Windows.Markup;
using System.Windows.Media;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global

// ReSharper disable UnusedMember.Global

namespace MathCore.WPF;

/// <summary>Трансформация масштабирования</summary>
[MarkupExtensionReturnType(typeof(ScaleTransform))]
public class Scale : MarkupExtension
{
    /// <summary>Масштаб по X</summary>
    public double X { get; set; } = 1;
    /// <summary>Масштаб по Y</summary>
    public double Y { get; set; } = 1;

    /// <summary>X-центра</summary>
    public double X0 { get; set; }

    /// <summary>Y-центра</summary>
    public double Y0 { get; set; }

    /// <summary>Трансформация масштабирования</summary>
    public Scale() { }

    /// <summary>Трансформация масштабирования</summary>
    /// <param name="x">Масштаб по X</param>
    /// <param name="y">Масштаб по Y</param>
    public Scale(double x, double y) { X = x; Y = y; }

    #region Overrides of MarkupExtension

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider sp) => new ScaleTransform { ScaleX = X, ScaleY = Y, CenterX = X0, CenterY = Y0 };

    #endregion 
}