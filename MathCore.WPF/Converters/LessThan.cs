using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер сравнения "меньше чем"</summary>
/// <remarks>
/// Проверяет, является ли входное значение меньше заданного порогового значения.
/// Возвращает <c>true</c>, если value &lt; Value, <c>false</c> в противном случае, и <c>null</c> если значение равно NaN.
/// <para><b>Формула:</b> result = value &lt; Value</para>
/// <para><b>ConvertBack:</b> Не поддерживается, так как невозможно восстановить конкретное число из булевого значения.
/// Результат <c>true</c> может означать любое число меньше порога, <c>false</c> - любое число больше или равное порогу.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Показать предупреждение если запас меньше 10 --&gt;
/// &lt;TextBlock Text="Низкий запас!" Visibility="{Binding Stock, Converter={converters:LessThan Value=10}}" /&gt;
/// 
/// &lt;!-- Заблокировать действие если значение меньше минимума --&gt;
/// &lt;Button IsEnabled="{Binding Amount, Converter={converters:LessThan 0}}" Content="Возврат" /&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(LessThan))]
[ValueConversion(typeof(double), typeof(bool?))]
public class LessThan(double value) : DoubleToBool
{
    public LessThan() : this(double.PositiveInfinity) { }

    /// <summary>Пороговое значение для сравнения</summary>
    public double Value { get; set; } = value;

    protected override bool? Convert(double v) => v is double.NaN ? null : v < Value;
}