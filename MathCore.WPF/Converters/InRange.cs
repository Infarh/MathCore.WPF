using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Проверяет, что значение входит в указанный интервал</summary>
[MarkupExtensionReturnType(typeof(InRange))]
[ValueConversion(typeof(double), typeof(bool?))]
public class InRange(Interval interval) : DoubleToBool
{
    public InRange() : this(double.NegativeInfinity, double.PositiveInfinity) { }

    public InRange(double MinMax) : this(new(-MinMax, MinMax)) { }

    public InRange(double min, double max) : this(new(Math.Min(min, max), Math.Max(min, max))) { }

    [ConstructorArgument(nameof(Min))]
    public double Min { get => interval.Min; set => interval = interval.SetMin(value); }

    [ConstructorArgument(nameof(Max))]
    public double Max { get => interval.Max; set => interval = interval.SetMax(value); }

    public bool MinInclude { get => interval.MinInclude; set => interval = interval.IncludeMin(value); }

    public bool MaxInclude { get => interval.MaxInclude; set => interval = interval.IncludeMax(value); }

    /// <summary>Возвращает null для NaN входа, иначе true если значение в интервале</summary>
    protected override bool? Convert(double v) => double.IsNaN(v) ? null : interval.Check(v);
}