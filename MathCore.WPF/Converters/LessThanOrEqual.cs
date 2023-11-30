using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(LessThanOrEqual))]
[ValueConversion(typeof(double), typeof(bool?))]
public class LessThanOrEqual(double value) : DoubleToBool
{
    public LessThanOrEqual() : this(double.PositiveInfinity) { }

    public double Value { get; set; } = value;

    protected override bool? Convert(double v) => v is double.NaN ? null : v <= Value;
}