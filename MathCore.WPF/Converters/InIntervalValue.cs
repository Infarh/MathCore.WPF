using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(InIntervalValue))]
[ValueConversion(typeof(double), typeof(double))]
public class InIntervalValue(Interval interval) : DoubleValueConverter
{
    public InIntervalValue() : this(double.NegativeInfinity, double.PositiveInfinity) { }

    public InIntervalValue(double MinMax) : this(new Interval(-MinMax, MinMax)) { }

    public InIntervalValue(double Min, double Max) : this(new Interval(Math.Min(Min, Max), Math.Max(Min, Max))) { }


    [ConstructorArgument(nameof(Min))]
    public double Min { get => interval.Min; set => interval = interval.SetMin(value); }

    [ConstructorArgument(nameof(Max))]
    public double Max { get => interval.Max; set => interval = interval.SetMax(value); }

    public bool MinInclude { get => interval.MinInclude; set => interval = interval.IncludeMin(value); }

    public bool MaxInclude { get => interval.MaxInclude; set => interval = interval.IncludeMax(value); }

    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) => (p ?? v) is not double.NaN and var value ? interval.Normalize(value) : double.NaN;
}