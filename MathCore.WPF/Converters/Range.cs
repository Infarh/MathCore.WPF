using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(Range))]
public class Range(Interval interval) : DoubleValueConverter
{
    public Range() : this(double.NegativeInfinity, double.PositiveInfinity) { }

    public Range(double MinMax) : this(new Interval(-MinMax, MinMax)) { }

    public Range(double Min, double Max) : this(new Interval(Math.Min(Min, Max), Math.Max(Min, Max))) { }

    [ConstructorArgument(nameof(Min))]
    public double Min { get => interval.Min; set => interval = interval.SetMin(value); }

    [ConstructorArgument(nameof(Max))]
    public double Max { get => interval.Max; set => interval = interval.SetMax(value); }

    public bool MinInclude { get => interval.MinInclude; set => interval = interval.IncludeMin(value); }

    public bool MaxInclude { get => interval.MaxInclude; set => interval = interval.IncludeMax(value); }

    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) => interval.Normalize(v);
}