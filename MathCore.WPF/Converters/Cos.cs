using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Cos))]
public class Cos : DoubleValueConverter
{
    public double K { get; set; } = 1;

    public double B { get; set; } = 0;

    public double W { get; set; } = Consts.pi2;

    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) => double.IsNaN(v) ? v : Math.Cos(W * v) * K + B;

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) => v;
}