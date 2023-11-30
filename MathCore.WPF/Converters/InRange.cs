using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(InRange))]
[ValueConversion(typeof(double), typeof(bool?))]
public class InRange(Interval interval) : DoubleToBool
{
    public InRange() : this(double.NegativeInfinity, double.PositiveInfinity) { }

    public InRange(double MinMax) : this(new Interval(-MinMax, MinMax)) { }

    public InRange(double min, double max) : this(new Interval(Math.Min(min, max), Math.Max(min, max))) { }

    [ConstructorArgument(nameof(Min))]
    public double Min { get => interval.Min; set => interval = interval.SetMin(value); }

    [ConstructorArgument(nameof(Max))]
    public double Max { get => interval.Max; set => interval = interval.SetMax(value); }

    public bool MinInclude { get => interval.MinInclude; set => interval = interval.IncludeMin(value); }

    public bool MaxInclude { get => interval.MaxInclude; set => interval = interval.IncludeMax(value); }

    /// <inheritdoc />
    protected override bool? Convert(double v) => v.IsNaN() ? null : interval.Check(v);
}