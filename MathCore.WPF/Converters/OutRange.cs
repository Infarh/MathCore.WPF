using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(OutRange))]
[ValueConversion(typeof(double), typeof(bool?))]
public class OutRange(Interval interval) : DoubleToBool
{
    public OutRange() : this(0, 0) { }

    public OutRange(double MinMax) : this(new Interval(-MinMax, MinMax)) { }

    public OutRange(double Min, double Max) : this(new Interval(Math.Min(Min, Max), Math.Max(Min, Max))) { }

    [ConstructorArgument(nameof(Min))]
    public double Min { get => interval.Min; set => interval = interval.SetMin(value); }

    [ConstructorArgument(nameof(Max))]
    public double Max { get => interval.Max; set => interval = interval.SetMax(value); }

    public bool MinInclude { get => interval.MinInclude; set => interval = interval.IncludeMin(value); }

    public bool MaxInclude { get => interval.MaxInclude; set => interval = interval.IncludeMax(value); }

    public bool? IncludeLimits
    {
        get => MinInclude && MaxInclude ? true : !MinInclude && !MaxInclude ? false : null;
        set
        {
            switch (value)
            {
                case true:
                    MinInclude = true;
                    MaxInclude = true;
                    break;
                case false:
                    MinInclude = false;
                    MaxInclude = false;
                    break;
            }
        }
    }

    /// <inheritdoc />
    protected override bool? Convert(double v) => v.IsNaN() ? null : !interval.Check(v);
}