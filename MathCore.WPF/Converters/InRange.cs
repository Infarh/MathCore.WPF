using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер проверки вхождения значения в диапазон</summary>
/// <remarks>
/// Проверяет, находится ли входное значение в заданном диапазоне [Min, Max].
/// Возвращает <c>true</c>, если значение находится в диапазоне, <c>false</c> в противном случае, и <c>null</c> если значение равно NaN.
/// Можно настроить включение границ диапазона через свойства MinInclude и MaxInclude.
/// <para><b>Формула:</b> result = Min &lt;= value &lt;= Max (с учётом включения границ)</para>
/// <para><b>ConvertBack:</b> Не поддерживается, так как невозможно восстановить конкретное число из булевого значения.
/// Результат <c>true</c> может означать любое число в диапазоне, <c>false</c> - любое число вне диапазона.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Проверка что значение в диапазоне 0-100 --&gt;
/// &lt;CheckBox IsChecked="{Binding Value, Converter={converters:InRange Min=0, Max=100}}" /&gt;
/// 
/// &lt;!-- Симметричный диапазон -50 до 50 --&gt;
/// &lt;TextBlock Visibility="{Binding Temperature, Converter={converters:InRange 50}}" /&gt;
/// 
/// &lt;!-- С включением/исключением границ --&gt;
/// &lt;Button IsEnabled="{Binding Age, Converter={converters:InRange Min=18, Max=65, MinInclude=True, MaxInclude=False}}" /&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(InRange))]
[ValueConversion(typeof(double), typeof(bool?))]
public class InRange(Interval interval) : DoubleToBool
{
    public InRange() : this(double.NegativeInfinity, double.PositiveInfinity) { }

    public InRange(double MinMax) : this(new(-MinMax, MinMax)) { }

    public InRange(double min, double max) : this(new(Math.Min(min, max), Math.Max(min, max))) { }

    /// <summary>Минимальная граница диапазона</summary>
    [ConstructorArgument(nameof(Min))]
    public double Min { get => interval.Min; set => interval = interval.SetMin(value); }

    /// <summary>Максимальная граница диапазона</summary>
    [ConstructorArgument(nameof(Max))]
    public double Max { get => interval.Max; set => interval = interval.SetMax(value); }

    /// <summary>Включать минимальную границу в диапазон</summary>
    public bool MinInclude { get => interval.MinInclude; set => interval = interval.IncludeMin(value); }

    /// <summary>Включать максимальную границу в диапазон</summary>
    public bool MaxInclude { get => interval.MaxInclude; set => interval = interval.IncludeMax(value); }

    /// <inheritdoc />
    protected override bool? Convert(double v) => v.IsNaN() ? null : interval.Check(v);
}