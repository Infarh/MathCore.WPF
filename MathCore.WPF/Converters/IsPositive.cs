using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки на положительное значение</summary>
/// <remarks>
/// Проверяет, является ли входное число положительным (больше нуля).
/// Возвращает <c>true</c>, если значение больше 0, <c>false</c> если меньше или равно 0, и <c>null</c> если значение равно NaN.
/// <para><b>Формула:</b> result = value &gt; 0</para>
/// <para><b>ConvertBack:</b> Не поддерживается, так как невозможно восстановить конкретное число из булевого значения.
/// Результат <c>true</c> может означать любое положительное число, <c>false</c> - любое неположительное.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Показать элемент только для положительных значений --&gt;
/// &lt;TextBlock Text="Положительное" Visibility="{Binding Value, Converter={converters:IsPositive}}" /&gt;
/// 
/// &lt;!-- Включить кнопку для положительного баланса --&gt;
/// &lt;Button IsEnabled="{Binding Balance, Converter={converters:IsPositive}}" /&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(IsPositive))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsPositive : DoubleToBool
{
    /// <inheritdoc />
    protected override bool? Convert(double v) => v is double.NaN ? null : v > 0;
}