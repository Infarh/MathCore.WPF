using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(IsNaN))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsNaN : DoubleToBool
{
    [ConstructorArgument(nameof(Inverted))]
    public bool Inverted { get; set; }

    public IsNaN() { }

    public IsNaN(bool Inverted) => this.Inverted = Inverted;

    /// <inheritdoc />
    protected override bool? Convert(double v) => Inverted ^ v.IsNaN();
}