using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки больше ли значение заданного порога</summary>
/// <remarks>
/// Проверяет, является ли входное значение строго больше заданного порога.
/// Возвращает null если входное значение равно NaN.
/// <example>
/// <code>
/// &lt;Button IsEnabled="{Binding Temperature, Converter={converters:GreaterThan Value=100}}" /&gt;
/// &lt;TextBlock Foreground="{Binding Score, Converter={converters:GreaterThan 50}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(GreaterThan))]
[ValueConversion(typeof(double), typeof(bool?))]
public class GreaterThan(double value) : DoubleToBool
{
    /// <summary>Инициализирует конвертер с порогом по умолчанию (отрицательная бесконечность)</summary>
    public GreaterThan() : this(double.NegativeInfinity) { }

    /// <summary>Пороговое значение для сравнения</summary>
    public double Value { get; set; } = value;

    /// <summary>Проверка значения на превышение порога</summary>
    /// <param name="v">Проверяемое значение</param>
    /// <returns>true если v > Value, false если v &lt;= Value, null если v равно NaN</returns>
    protected override bool? Convert(double v) => v.IsNaN() ? null : v > Value;
}