using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки на отрицательное значение</summary>
/// <remarks>
/// Проверяет, является ли входное число отрицательным (меньше нуля).
/// Возвращает <c>true</c>, если значение меньше 0, <c>false</c> если больше или равно 0, и <c>null</c> если значение равно NaN.
/// <para><b>Формула:</b> result = value &lt; 0</para>
/// <para><b>ConvertBack:</b> Не поддерживается, так как невозможно восстановить конкретное число из булевого значения.
/// Результат <c>true</c> может означать любое отрицательное число, <c>false</c> - любое неотрицательное.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Показать предупреждение для отрицательного баланса --&gt;
/// &lt;TextBlock Text="Задолженность!" Visibility="{Binding Balance, Converter={converters:IsNegative}}" /&gt;
/// 
/// &lt;!-- Изменить цвет для отрицательных значений --&gt;
/// &lt;TextBlock Foreground="{Binding Value, Converter={converters:IsNegative}}" /&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(IsNegative))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsNegative : DoubleToBool
{
    /// <inheritdoc />
    protected override bool? Convert(double v) => v.IsNaN() ? null : v < 0;
}