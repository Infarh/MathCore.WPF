using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Ctg))]
public class Ctg : DoubleValueConverter
{
    public double K { get; set; } = 1;

    public double B { get; set; } = 0;

    public double W { get; set; } = Consts.pi2;

    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) => double.IsNaN(v) ? v : K / Math.Tan(W * v) + B;

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) => v;
}