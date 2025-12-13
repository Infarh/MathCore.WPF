using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки попадания значения в заданный диапазон</summary>
/// <remarks>
/// Проверяет, находится ли входное значение в заданном диапазоне [Min, Max].
/// Поддерживает включение/исключение границ через свойства MinInclude и MaxInclude.
/// Возвращает null если входное значение равно NaN.
/// <example>
/// <code>
/// &lt;TextBlock Visibility="{Binding Value, Converter={converters:InRange Min=0, Max=100}}" /&gt;
/// &lt;Border Background="Green" Visibility="{Binding Temperature, Converter={converters:InRange 18, 25}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(InRange))]
[ValueConversion(typeof(double), typeof(bool?))]
public class InRange(Interval interval) : DoubleToBool
{
    /// <summary>Инициализирует конвертер с диапазоном по умолчанию (вся числовая прямая)</summary>
    public InRange() : this(double.NegativeInfinity, double.PositiveInfinity) { }

    /// <summary>Инициализирует конвертер с симметричным диапазоном [-MinMax, MinMax]</summary>
    /// <param name="MinMax">Абсолютное значение границ диапазона</param>
    public InRange(double MinMax) : this(new(-MinMax, MinMax)) { }

    /// <summary>Инициализирует конвертер с заданным диапазоном</summary>
    /// <param name="min">Минимальное значение диапазона</param>
    /// <param name="max">Максимальное значение диапазона</param>
    public InRange(double min, double max) : this(new(Math.Min(min, max), Math.Max(min, max))) { }

    /// <summary>Минимальное значение диапазона</summary>
    [ConstructorArgument(nameof(Min))]
    public double Min { get => interval.Min; set => interval = interval.SetMin(value); }

    /// <summary>Максимальное значение диапазона</summary>
    [ConstructorArgument(nameof(Max))]
    public double Max { get => interval.Max; set => interval = interval.SetMax(value); }

    /// <summary>Включать ли минимальную границу в диапазон</summary>
    public bool MinInclude { get => interval.MinInclude; set => interval = interval.IncludeMin(value); }

    /// <summary>Включать ли максимальную границу в диапазон</summary>
    public bool MaxInclude { get => interval.MaxInclude; set => interval = interval.IncludeMax(value); }

    /// <summary>Проверка попадания значения в диапазон</summary>
    /// <param name="v">Проверяемое значение</param>
    /// <returns>true если значение в диапазоне, false если вне диапазона, null если v равно NaN</returns>
    protected override bool? Convert(double v) => v.IsNaN() ? null : interval.Check(v);
}