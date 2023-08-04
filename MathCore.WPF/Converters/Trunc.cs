using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(double), typeof(double)), MarkupExtensionReturnType(typeof(Trunc))]
public class Trunc : DoubleValueConverter
{
    protected override double Convert(double v, double? p = null) => Math.Truncate(v);

    protected override double ConvertBack(double v, double? p = null) => v;
}