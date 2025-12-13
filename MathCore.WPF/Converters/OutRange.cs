using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки выхода значения за пределы диапазона</summary>
/// <remarks>
/// Проверяет, находится ли входное значение ВНЕ заданного диапазона [Min, Max].
/// Возвращает <c>true</c>, если значение находится вне диапазона, <c>false</c> если внутри, и <c>null</c> если значение равно NaN.
/// Можно настроить включение границ диапазона через свойства MinInclude и MaxInclude.
/// <para><b>Формула:</b> result = !(Min &lt;= value &lt;= Max) (с учётом включения границ)</para>
/// <para><b>ConvertBack:</b> Не поддерживается, так как невозможно восстановить конкретное число из булевого значения.
/// Результат <c>true</c> может означать любое число вне диапазона, <c>false</c> - любое число внутри диапазона.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Предупреждение если значение вне допустимого диапазона --&gt;
/// &lt;TextBlock Text="Значение вне диапазона!" Visibility="{Binding Value, Converter={converters:OutRange Min=0, Max=100}}" /&gt;
/// 
/// &lt;!-- Симметричная проверка выхода за пределы ±50 --&gt;
/// &lt;Border BorderBrush="Red" Visibility="{Binding Temperature, Converter={converters:OutRange 50}}" /&gt;
/// 
/// &lt;!-- С настройкой включения границ --&gt;
/// &lt;Button IsEnabled="{Binding Age, Converter={converters:OutRange Min=18, Max=65, IncludeLimits=True}}" /&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(OutRange))]
[ValueConversion(typeof(double), typeof(bool?))]
public class OutRange(Interval interval) : DoubleToBool
{
    public OutRange() : this(0, 0) { }

    public OutRange(double MinMax) : this(new(-MinMax, MinMax)) { }

    public OutRange(double Min, double Max) : this(new(Math.Min(Min, Max), Math.Max(Min, Max))) { }

    /// <summary>Минимальная граница диапазона</summary>
    [ConstructorArgument(nameof(Min))]
    public double Min { get => interval.Min; set => interval = interval.SetMin(value); }

    /// <summary>Максимальная граница диапазона</summary>
    [ConstructorArgument(nameof(Max))]
    public double Max { get => interval.Max; set => interval = interval.SetMax(value); }

    /// <summary>Включать минимальную границу в диапазон</summary>
    public bool MinInclude { get => interval.MinInclude; set => interval = interval.IncludeMin(value); }

    /// <summary>Максимальная граница диапазона</summary>
    public bool MaxInclude { get => interval.MaxInclude; set => interval = interval.IncludeMax(value); }

    /// <summary>Одновременная установка включения обеих границ</summary>
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

    /// <inheritdoc />
    protected override bool? Convert(double v) => v.IsNaN() ? null : !interval.Check(v);
}