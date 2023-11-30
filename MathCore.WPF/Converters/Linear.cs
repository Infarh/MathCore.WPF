using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Линейный конвертер вещественных величин по формуле result = K*value + B</summary>
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