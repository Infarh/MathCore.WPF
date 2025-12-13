using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь вычисления косинуса по формуле: result = K * cos(W * value) + B</summary>
/// <remarks>Обратное преобразование не поддерживается, так как cos(x) = a имеет бесконечное множество решений</remarks>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Cos))]
public class Cos : DoubleValueConverter
{
    /// <summary>Коэффициент масштабирования амплитуды</summary>
    public double K { get; set; } = 1;

    /// <summary>Смещение результата</summary>
    public double B { get; set; } = 0;

    /// <summary>Угловая частота (по умолчанию 2π)</summary>
    public double W { get; set; } = Consts.pi2;

    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) => double.IsNaN(v) ? v : Math.Cos(W * v) * K + B;

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) => throw new NotSupportedException("Обратное преобразование косинуса не поддерживается");
}