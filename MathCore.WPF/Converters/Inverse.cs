using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(double))]
public class Inverse : SimpleDoubleValueConverter
{
    protected override double Convert(double v, double? p = null) => p is { } k ? k / v : 1 / v;
}