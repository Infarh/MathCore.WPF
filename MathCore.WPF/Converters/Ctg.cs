using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Преобразует значение по функции котангенса</summary>
[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Ctg))]
public class Ctg : DoubleValueConverter
{
    /// <summary>Множитель выходного значения</summary>
    public double K { get; set; } = 1;

    /// <summary>Аддитивное смещение</summary>
    public double B { get; set; } = 0;

    /// <summary>Коэффициент перед аргументом функции</summary>
    public double W { get; set; } = Consts.pi2;

    /// <summary>Возвращает NaN для NaN входа, иначе K / Tan(W * v) + B</summary>
    protected override double Convert(double v, double? p = null) => double.IsNaN(v) ? v : K / Math.Tan(W * v) + B;

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) => v;
}