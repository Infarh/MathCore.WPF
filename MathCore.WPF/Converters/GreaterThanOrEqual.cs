using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер сравнения "больше или равно"</summary>
/// <remarks>
/// Проверяет, является ли входное значение больше или равно заданному пороговому значению.
/// Возвращает <c>true</c>, если value &gt;= Value, <c>false</c> в противном случае, и <c>null</c> если значение равно NaN.
/// <para><b>Формула:</b> result = value &gt;= Value</para>
/// <para><b>ConvertBack:</b> Не поддерживается, так как невозможно восстановить конкретное число из булевого значения.
/// Результат <c>true</c> может означать любое число больше или равное порогу, <c>false</c> - любое число меньше порога.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Включить кнопку если баланс не меньше 0 --&gt;
/// &lt;Button IsEnabled="{Binding Balance, Converter={converters:GreaterThanOrEqual Value=0}}" /&gt;
/// 
/// &lt;!-- Проверка на совершеннолетие --&gt;
/// &lt;CheckBox IsChecked="{Binding Age, Converter={converters:GreaterThanOrEqual 18}}" /&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(GreaterThanOrEqual))]
[ValueConversion(typeof(double), typeof(bool?))]
public class GreaterThanOrEqual(double value) : DoubleToBool
{
    public GreaterThanOrEqual() : this(double.NegativeInfinity) { }

    /// <summary>Пороговое значение для сравнения</summary>
    public double Value { get; set; } = value;

    protected override bool? Convert(double v) => v.IsNaN() ? null : v >= Value;
}