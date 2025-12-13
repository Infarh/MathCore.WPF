using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки больше или равно ли значение заданного порога</summary>
/// <remarks>
/// Проверяет, является ли входное значение больше или равно заданному порогу.
/// Возвращает null если входное значение равно NaN.
/// <example>
/// <code>
/// &lt;Button IsEnabled="{Binding Age, Converter={converters:GreaterThanOrEqual Value=18}}" /&gt;
/// &lt;ProgressBar Value="{Binding Progress, Converter={converters:GreaterThanOrEqual 0}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(GreaterThanOrEqual))]
[ValueConversion(typeof(double), typeof(bool?))]
public class GreaterThanOrEqual(double value) : DoubleToBool
{
    /// <summary>Инициализирует конвертер с порогом по умолчанию (отрицательная бесконечность)</summary>
    public GreaterThanOrEqual() : this(double.NegativeInfinity) { }

    /// <summary>Пороговое значение для сравнения</summary>
    public double Value { get; set; } = value;

    /// <summary>Проверка значения на превышение или равенство порогу</summary>
    /// <param name="v">Проверяемое значение</param>
    /// <returns>true если v >= Value, false если v &lt; Value, null если v равно NaN</returns>
    protected override bool? Convert(double v) => v.IsNaN() ? null : v >= Value;
}