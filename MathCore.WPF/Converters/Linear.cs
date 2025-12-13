using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Линейный конвертер вещественных величин по формуле result = K*value + B</summary>
/// <remarks>
/// Выполняет линейное преобразование входного значения по формуле y = K*x + B.
/// Это наиболее универсальный арифметический конвертер, объединяющий масштабирование и смещение.
/// <para><b>Формула Convert:</b> result = K * value + B</para>
/// <para><b>Формула ConvertBack:</b> value = (result - B) / K</para>
/// <para><b>ConvertBack:</b> Полностью поддерживается при K ≠ 0. Операция линейного преобразования обратима.
/// При K = 0 обратное преобразование вернёт Infinity или NaN.
/// При двусторонней привязке корректно восстанавливает исходное значение.</para>
/// <para><b>Свойство Inverted:</b> При установке в true меняет местами прямое и обратное преобразование.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Преобразование температуры Цельсия в Кельвины: K = °C + 273.15 --&gt;
/// &lt;TextBlock Text="{Binding Celsius, Converter={converters:Linear K=1, B=273.15}}" /&gt;
/// 
/// &lt;!-- Масштабирование и смещение с двусторонней привязкой --&gt;
/// &lt;Slider Value="{Binding Value, Converter={converters:Linear K=2, B=10}, Mode=TwoWay}" /&gt;
/// 
/// &lt;!-- Только масштабирование (K=2, B=0) --&gt;
/// &lt;TextBlock Text="{Binding Value, Converter={converters:Linear 2}}" /&gt;
/// 
/// &lt;!-- Инвертированное преобразование --&gt;
/// &lt;TextBlock Text="{Binding Value, Converter={converters:Linear K=2, B=10, Inverted=True}}" /&gt;
/// </code>
/// </example>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Linear))]
public class Linear(double K, double B) : DoubleValueConverter
{
    public Linear() : this(1, 0) { }

    public Linear(double K) : this(K, 0) { }

    /// <summary>Линейный множитель (тангенс угла наклона)</summary>
    [ConstructorArgument(nameof(K))]
    public double K { get; set; } = K;

    /// <summary>Аддитивное смещение</summary>
    [ConstructorArgument(nameof(B))]
    public double B { get; set; } = B;

    /// <summary>Инвертировать преобразование</summary>
    public bool Inverted { get; set; }

    private static double To(double x, double k, double b) => k * x + b;
    private static double From(double x, double k, double b) => (x - b) / k;

    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) =>
        Inverted
            ? From(p ?? v, K, B)
            : To(p ?? v, K, B);

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) =>
        Inverted
            ? To(v, K, B)
            : From(v, K, B);
}