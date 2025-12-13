using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки меньше или равно ли значение заданного порога</summary>
/// <remarks>
/// Проверяет, является ли входное значение меньше или равно заданному порогу.
/// Возвращает null если входное значение равно NaN.
/// <example>
/// <code>
/// &lt;TextBlock Text="Низкая скорость" Visibility="{Binding Speed, Converter={converters:LessThanOrEqual Value=30}}" /&gt;
/// &lt;ProgressBar Foreground="Green" Value="{Binding Load, Converter={converters:LessThanOrEqual 50}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(LessThanOrEqual))]
[ValueConversion(typeof(double), typeof(bool?))]
public class LessThanOrEqual(double value) : DoubleToBool
{
    /// <summary>Инициализирует конвертер с порогом по умолчанию (положительная бесконечность)</summary>
    public LessThanOrEqual() : this(double.PositiveInfinity) { }

    /// <summary>Пороговое значение для сравнения</summary>
    public double Value { get; set; } = value;

    /// <summary>Проверка значения на меньше или равно порогу</summary>
    /// <param name="v">Проверяемое значение</param>
    /// <returns>true если v &lt;= Value, false если v > Value, null если v равно NaN</returns>
    protected override bool? Convert(double v) => v is double.NaN ? null : v <= Value;
}