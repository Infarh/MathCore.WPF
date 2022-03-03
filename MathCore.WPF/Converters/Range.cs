using System;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Converters;

public class Range : DoubleValueConverter
{
    private Interval _Interval = new(double.NegativeInfinity, double.PositiveInfinity);

    [ConstructorArgument(nameof(Min))]
    public double Min { get => _Interval.Min; set => _Interval = _Interval.SetMin(value); }

    [ConstructorArgument(nameof(Max))]
    public double Max { get => _Interval.Max; set => _Interval = _Interval.SetMax(value); }

    public bool MinInclude { get => _Interval.MinInclude; set => _Interval = _Interval.IncludeMin(value); }

    public bool MaxInclude { get => _Interval.MaxInclude; set => _Interval = _Interval.IncludeMax(value); }

    public Range() { }

    public Range(double MinMax) : this(new Interval(-MinMax, MinMax)) { }

    public Range(double Min, double Max) : this(new Interval(Math.Min(Min, Max), Math.Max(Min, Max))) { }

    public Range(Interval interval) => _Interval = interval;

    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) => _Interval.Normalize(v);
}