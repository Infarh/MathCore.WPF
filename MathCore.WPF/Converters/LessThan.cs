using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(LessThan))]
[ValueConversion(typeof(double), typeof(bool?))]
public class LessThan : DoubleToBool
{
    public double Value { get; set; } = double.PositiveInfinity;

    public LessThan() { }

    public LessThan(double value) => Value = value;

    protected override bool? Convert(double v) => v.IsNaN() ? null : v < Value;
}