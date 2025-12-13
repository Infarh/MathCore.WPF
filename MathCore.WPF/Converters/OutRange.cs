using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки выхода значения за пределы заданного диапазона</summary>
/// <remarks>
/// Проверяет, находится ли входное значение вне заданного диапазона [Min, Max].
/// Поддерживает включение/исключение границ через свойства MinInclude и MaxInclude.
/// Возвращает null если входное значение равно NaN.
/// <example>
/// <code>
/// &lt;Border Background="Red" Visibility="{Binding Value, Converter={converters:OutRange Min=0, Max=100}}" /&gt;
/// &lt;TextBlock Text="Вне нормы" Visibility="{Binding Temperature, Converter={converters:OutRange 18, 25}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(OutRange))]
[ValueConversion(typeof(double), typeof(bool?))]
public class OutRange(Interval interval) : DoubleToBool
{
    /// <summary>Инициализирует конвертер с нулевым диапазоном [0, 0]</summary>
    public OutRange() : this(0, 0) { }

    /// <summary>Инициализирует конвертер с симметричным диапазоном [-MinMax, MinMax]</summary>
    /// <param name="MinMax">Абсолютное значение границ диапазона</param>
    public OutRange(double MinMax) : this(new(-MinMax, MinMax)) { }

    /// <summary>Инициализирует конвертер с заданным диапазоном</summary>
    /// <param name="Min">Минимальное значение диапазона</param>
    /// <param name="Max">Максимальное значение диапазона</param>
    public OutRange(double Min, double Max) : this(new(Math.Min(Min, Max), Math.Max(Min, Max))) { }

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

    /// <summary>Включать ли обе границы в диапазон</summary>
    public bool? IncludeLimits
    {
        get => MinInclude && MaxInclude ? true : !MinInclude && !MaxInclude ? false : null;
        set
        {
            switch (value)
            {
                case true:
                    MinInclude = true;
                    MaxInclude = true;
                    break;
                case false:
                    MinInclude = false;
                    MaxInclude = false;
                    break;
            }
        }
    }

    /// <summary>Проверка выхода значения за пределы диапазона</summary>
    /// <param name="v">Проверяемое значение</param>
    /// <returns>true если значение вне диапазона, false если в диапазоне, null если v равно NaN</returns>
    protected override bool? Convert(double v) => v.IsNaN() ? null : !interval.Check(v);
}