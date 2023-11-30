using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(IsNaN))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsNaN(bool Inverted) : DoubleToBool
{
    public IsNaN() : this(false) { }

    [ConstructorArgument(nameof(Inverted))]
    public bool Inverted { get; set; } = Inverted;

    /// <inheritdoc />
    protected override bool? Convert(double v) => Inverted ^ v.IsNaN();
}