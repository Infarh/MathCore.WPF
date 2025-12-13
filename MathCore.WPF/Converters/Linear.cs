using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Линейный конвертер вещественных величин по формуле result = K*value + B</summary>
/// <remarks>
/// Выполняет линейное преобразование: result = K * value + B.
/// Поддерживает двустороннее связывание: ConvertBack выполняет обратное преобразование (result - B) / K.
/// Свойство Inverted позволяет инвертировать направление преобразования.
/// <example>
/// <code>
/// &lt;!-- Преобразование Цельсия в Фаренгейт: F = 1.8 * C + 32 --&gt;
/// &lt;Slider Value="{Binding Celsius, Converter={converters:Linear K=1.8, B=32}}" /&gt;
/// 
/// &lt;!-- Масштабирование с смещением --&gt;
/// &lt;ProgressBar Value="{Binding Progress, Converter={converters:Linear 2, 50}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Linear))]
public class Linear(double K, double B) : DoubleValueConverter
{
    /// <summary>Инициализирует конвертер без преобразования (K=1, B=0)</summary>
    public Linear() : this(1, 0) { }

    /// <summary>Инициализирует конвертер только с множителем (B=0)</summary>
    /// <param name="K">Линейный множитель</param>
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

    /// <summary>Прямое преобразование: result = K * value + B</summary>
    /// <param name="v">Входное значение</param>
    /// <param name="p">Параметр (не используется)</param>
    /// <returns>Преобразованное значение</returns>
    protected override double Convert(double v, double? p = null) =>
        Inverted
            ? From(p ?? v, K, B)
            : To(p ?? v, K, B);

    /// <summary>Обратное преобразование: value = (result - B) / K</summary>
    /// <param name="v">Преобразованное значение</param>
    /// <param name="p">Параметр (не используется)</param>
    /// <returns>Исходное значение</returns>
    protected override double ConvertBack(double v, double? p = null) =>
        Inverted
            ? To(v, K, B)
            : From(v, K, B);
}