using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Converters;

/// <summary>Создаёт нормализатор значений в пределах заданного интервала</summary>
[MarkupExtensionReturnType(typeof(Range))]
public class Range(Interval interval) : DoubleValueConverter
{
    /// <summary>Создаёт нормализатор значений в пределах заданного интервала</summary>
    public Range() : this(double.NegativeInfinity, double.PositiveInfinity) { }

    public Range(double MinMax) : this(new(-MinMax, MinMax)) { }

    public Range(double Min, double Max) : this(new(Math.Min(Min, Max), Math.Max(Min, Max))) { }

    [ConstructorArgument(nameof(Min))]
    public override double? Min { get => interval.Min; set => interval = interval.SetMin(value ?? double.NegativeInfinity); }

    [ConstructorArgument(nameof(Max))]
    public override double? Max { get => interval.Max; set => interval = interval.SetMax(value ?? double.PositiveInfinity); }

    public bool MinInclude { get => interval.MinInclude; set => interval = interval.IncludeMin(value); }

    public bool MaxInclude { get => interval.MaxInclude; set => interval = interval.IncludeMax(value); }

    /// <summary>Нормализует значение в пределах интервала</summary>
    protected override double Convert(double v, double? p = null) => interval.Normalize(v);
}