using System;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(GreaterThan))]
[ValueConversion(typeof(double), typeof(bool?))]
public class GreaterThan : DoubleToBool
{
    public double Value { get; set; } = double.NegativeInfinity;

    public GreaterThan() { }

    public GreaterThan(double value) => Value = value;

    protected override bool? Convert(double v) => v.IsNaN() ? null : v > Value;
}