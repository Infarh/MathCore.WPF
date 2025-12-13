using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер сравнения "меньше или равно"</summary>
/// <remarks>
/// Проверяет, является ли входное значение меньше или равно заданному пороговому значению.
/// Возвращает <c>true</c>, если value &lt;= Value, <c>false</c> в противном случае, и <c>null</c> если значение равно NaN.
/// <para><b>Формула:</b> result = value &lt;= Value</para>
/// <para><b>ConvertBack:</b> Не поддерживается, так как невозможно восстановить конкретное число из булевого значения.
/// Результат <c>true</c> может означать любое число меньше или равное порогу, <c>false</c> - любое число больше порога.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Показать предупреждение если температура не выше допустимой --&gt;
/// &lt;TextBlock Text="Температура в норме" Visibility="{Binding Temperature, Converter={converters:LessThanOrEqual Value=100}}" /&gt;
/// 
/// &lt;!-- Ограничить доступ по возрасту --&gt;
/// &lt;Button IsEnabled="{Binding Age, Converter={converters:LessThanOrEqual 120}}" /&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(LessThanOrEqual))]
[ValueConversion(typeof(double), typeof(bool?))]
public class LessThanOrEqual(double value) : DoubleToBool
{
    public LessThanOrEqual() : this(double.PositiveInfinity) { }

    /// <summary>Пороговое значение для сравнения</summary>
    public double Value { get; set; } = value;

    protected override bool? Convert(double v) => v is double.NaN ? null : v <= Value;
}