using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер сравнения "больше чем"</summary>
/// <remarks>
/// Проверяет, является ли входное значение больше заданного порогового значения.
/// Возвращает <c>true</c>, если value &gt; Value, <c>false</c> в противном случае, и <c>null</c> если значение равно NaN.
/// <para><b>Формула:</b> result = value &gt; Value</para>
/// <para><b>ConvertBack:</b> Не поддерживается, так как невозможно восстановить конкретное число из булевого значения.
/// Результат <c>true</c> может означать любое число больше порога, <c>false</c> - любое число меньше или равное порогу.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Включить кнопку если значение больше 100 --&gt;
/// &lt;Button IsEnabled="{Binding Count, Converter={converters:GreaterThan Value=100}}" /&gt;
/// 
/// &lt;!-- Показать предупреждение если температура выше 50 --&gt;
/// &lt;TextBlock Text="Перегрев!" Visibility="{Binding Temperature, Converter={converters:GreaterThan 50}}" /&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(GreaterThan))]
[ValueConversion(typeof(double), typeof(bool?))]
public class GreaterThan(double value) : DoubleToBool
{
    public GreaterThan() : this(double.NegativeInfinity) { }

    /// <summary>Пороговое значение для сравнения</summary>
    public double Value { get; set; } = value;

    protected override bool? Convert(double v) => v.IsNaN() ? null : v > Value;
}