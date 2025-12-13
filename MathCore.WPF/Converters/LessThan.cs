using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки меньше ли значение заданного порога</summary>
/// <remarks>
/// Проверяет, является ли входное значение строго меньше заданного порога.
/// Возвращает null если входное значение равно NaN.
/// <example>
/// <code>
/// &lt;TextBlock Visibility="{Binding Temperature, Converter={converters:LessThan Value=0}}" /&gt;
/// &lt;Border Background="Red" Visibility="{Binding Battery, Converter={converters:LessThan 20}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(LessThan))]
[ValueConversion(typeof(double), typeof(bool?))]
public class LessThan(double value) : DoubleToBool
{
    /// <summary>Инициализирует конвертер с порогом по умолчанию (положительная бесконечность)</summary>
    public LessThan() : this(double.PositiveInfinity) { }

    /// <summary>Пороговое значение для сравнения</summary>
    public double Value { get; set; } = value;

    /// <summary>Проверка значения на меньше порога</summary>
    /// <param name="v">Проверяемое значение</param>
    /// <returns>true если v &lt; Value, false если v >= Value, null если v равно NaN</returns>
    protected override bool? Convert(double v) => v is double.NaN ? null : v < Value;
}