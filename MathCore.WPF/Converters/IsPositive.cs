using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки числа на положительность</summary>
/// <remarks>
/// Проверяет, является ли входное значение положительным числом (больше нуля).
/// Возвращает null если входное значение равно NaN.
/// <example>
/// <code>
/// &lt;Button IsEnabled="{Binding Balance, Converter={converters:IsPositive}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(IsPositive))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsPositive : DoubleToBool
{
    /// <summary>Проверка значения на положительность</summary>
    /// <param name="v">Проверяемое значение</param>
    /// <returns>true если v > 0, false если v &lt;= 0, null если v равно NaN</returns>
    protected override bool? Convert(double v) => v is double.NaN ? null : v > 0;
}