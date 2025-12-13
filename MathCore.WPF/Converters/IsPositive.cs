using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки числового значения на положительность</summary>
/// <remarks>
/// Проверяет является ли входное значение типа double положительным числом (больше нуля)
/// <para>Возвращаемые значения:</para>
/// <list type="bullet">
/// <item><description>v &gt; 0 → true</description></item>
/// <item><description>v ≤ 0 → false</description></item>
/// <item><description>NaN → null</description></item>
/// <item><description>+∞ → true</description></item>
/// <item><description>-∞ → false</description></item>
/// </list>
/// <para>ConvertBack не поддерживается, так как невозможно однозначно восстановить исходное числовое значение из булевого результата</para>
/// </remarks>
/// <example>
/// <code language="xaml"><![CDATA[
/// <!-- Отображение индикатора для положительных значений -->
/// <TextBlock Text="Положительное" 
///            Visibility="{Binding Value, Converter={converters:IsPositive}}" />
/// 
/// <!-- Активация кнопки только для положительных коэффициентов -->
/// <Button IsEnabled="{Binding Coefficient, Converter={converters:IsPositive}}" />
/// ]]></code>
/// </example>
[MarkupExtensionReturnType(typeof(IsPositive))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsPositive : DoubleToBool
{
    /// <inheritdoc />
    protected override bool? Convert(double v) => v is double.NaN ? null : v > 0;
}