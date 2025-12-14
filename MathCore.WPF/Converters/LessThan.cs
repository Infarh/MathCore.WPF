using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Проверяет что число меньше заданного порога</summary>
[MarkupExtensionReturnType(typeof(LessThan))]
[ValueConversion(typeof(double), typeof(bool?))]
public class LessThan(double value) : DoubleToBool
{
    public LessThan() : this(double.PositiveInfinity) { }

    /// <summary>Пороговое значение</summary>
    public double Value { get; set; } = value;

    /// <summary>Возвращает null при NaN, иначе true если v &lt; Value</summary>
    protected override bool? Convert(double v) => double.IsNaN(v) ? null : v < Value;
}