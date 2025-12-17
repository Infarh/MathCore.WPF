using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Проверяет что значение больше заданного порога</summary>
[MarkupExtensionReturnType(typeof(GreaterThan))]
[ValueConversion(typeof(double), typeof(bool?))]
public class GreaterThan(double value) : DoubleToBool
{
    public GreaterThan() : this(double.NegativeInfinity) { }

    /// <summary>Пороговое значение</summary>
    public double Value { get; set; } = value;

    /// <summary>
    /// Возвращает null при NaN входе, иначе true если v &gt; Value
    /// </summary>
    protected override bool? Convert(double v) => double.IsNaN(v) ? null : v > Value;
}