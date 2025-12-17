using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь сравнения значения с порогом (меньше или равно)</summary>
[MarkupExtensionReturnType(typeof(LessThanOrEqual))]
[ValueConversion(typeof(double), typeof(bool?))]
public class LessThanOrEqual(double value) : DoubleToBool
{
    public LessThanOrEqual() : this(double.PositiveInfinity) { }

    /// <summary>Пороговое значение</summary>
    public double Value { get; set; } = value;

    /// <summary>Преобразует входное значение в логическое, возвращая null для NaN</summary>
    protected override bool? Convert(double v) => double.IsNaN(v) ? null : v <= Value;
}