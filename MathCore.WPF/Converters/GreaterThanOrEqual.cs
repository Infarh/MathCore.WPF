using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(GreaterThanOrEqual))]
[ValueConversion(typeof(double), typeof(bool?))]
public class GreaterThanOrEqual(double value) : DoubleToBool
{
    public GreaterThanOrEqual() : this(double.NegativeInfinity) { }

    public double Value { get; set; } = value;

    protected override bool? Convert(double v) => v.IsNaN() ? null : v >= Value;
}