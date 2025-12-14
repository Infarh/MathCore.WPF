using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Проверяет что значение больше или равно заданному порогу</summary>
[MarkupExtensionReturnType(typeof(GreaterThanOrEqual))]
[ValueConversion(typeof(double), typeof(bool?))]
public class GreaterThanOrEqual(double value) : DoubleToBool
{
    public GreaterThanOrEqual() : this(double.NegativeInfinity) { }

    /// <summary>Пороговое значение</summary>
    public double Value { get; set; } = value;

    /// <summary>Возвращает null для NaN, иначе true если v >= Value</summary>
    protected override bool? Convert(double v) => double.IsNaN(v) ? null : v >= Value;
}