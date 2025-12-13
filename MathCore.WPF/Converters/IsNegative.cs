using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки числа на отрицательность</summary>
/// <remarks>
/// Проверяет, является ли входное значение отрицательным числом (меньше нуля).
/// Возвращает null если входное значение равно NaN.
/// <example>
/// <code>
/// &lt;TextBlock Foreground="{Binding Profit, Converter={converters:IsNegative}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(IsNegative))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsNegative : DoubleToBool
{
    /// <summary>Проверка значения на отрицательность</summary>
    /// <param name="v">Проверяемое значение</param>
    /// <returns>true если v &lt; 0, false если v >= 0, null если v равно NaN</returns>
    protected override bool? Convert(double v) => v.IsNaN() ? null : v < 0;
}