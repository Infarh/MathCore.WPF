using System.Windows.Markup;
using System.Windows.Media;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF;

/// <summary>Трансформация поворота</summary>
/// <remarks>Трансформация поворота</remarks>
/// <param name="angle">Угол</param>
[MarkupExtensionReturnType(typeof(RotateTransform))]
public class Rotate(double angle) : MarkupExtension
{
    /// <summary>Трансформация поворота</summary>
    public Rotate() : this(0) { }

    /// <summary>Угол</summary>
    public double Angle { get; set; } = angle;

    /// <summary>X-центра</summary>
    public double X { get; set; }

    /// <summary>Y-центра</summary>
    public double Y { get; set; }

    #region Overrides of MarkupExtension

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider sp) => new RotateTransform { Angle = Angle, CenterX = X, CenterY = Y };

    #endregion 
}