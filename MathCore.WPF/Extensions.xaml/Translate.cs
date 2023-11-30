using System.Windows.Markup;
using System.Windows.Media;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF;

/// <summary>Трансформация смещения</summary>
/// <remarks>Трансформация смещения</remarks>
/// <param name="x">Смещение по X</param>
/// <param name="y">Смещение по Y</param>
[MarkupExtensionReturnType(typeof(TranslateTransform))]
public class Translate(double x, double y) : MarkupExtension
{
    /// <summary>Трансформация смещения</summary>
    public Translate() : this(0, 0) { }

    /// <summary>Смещение по X</summary>
    public double X { get; set; } = x;     
    
    /// <summary>Смещение по Y</summary>
    public double Y { get; set; } = y;
    #region Overrides of MarkupExtension

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider sp) => new TranslateTransform { X = X, Y = Y };

    #endregion 
}