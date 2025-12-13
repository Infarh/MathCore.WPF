using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки числового значения на отрицательность</summary>
/// <remarks>
/// Проверяет является ли входное значение типа double отрицательным числом (меньше нуля)
/// <para>Возвращаемые значения:</para>
/// <list type="bullet">
/// <item><description>v &lt; 0 → true</description></item>
/// <item><description>v ≥ 0 → false</description></item>
/// <item><description>NaN → null</description></item>
/// <item><description>-∞ → true</description></item>
/// <item><description>+∞ → false</description></item>
/// </list>
/// <para>ConvertBack не поддерживается, так как невозможно однозначно восстановить исходное числовое значение из булевого результата</para>
/// </remarks>
/// <example>
/// <code language="xaml"><![CDATA[
/// <!-- Отображение предупреждения для отрицательных значений -->
/// <TextBlock Text="Отрицательное значение" Foreground="Red"
///            Visibility="{Binding Balance, Converter={converters:IsNegative}}" />
/// 
/// <!-- Блокировка элемента при отрицательном значении -->
/// <Button IsEnabled="{Binding Delta, Converter={converters:IsNegative}}" 
///         Content="Уменьшить" />
/// ]]></code>
/// </example>
[MarkupExtensionReturnType(typeof(IsNegative))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsNegative : DoubleToBool
{
    /// <inheritdoc />
    protected override bool? Convert(double v) => v.IsNaN() ? null : v < 0;
}