using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.WindowTest;

[MarkupExtensionReturnType(typeof(TestConverter))]
public class TestConverter : DoubleValueConverter
{
    public Func<double, double> Code { get; set; } = x => x * 2;

    protected override double Convert(double v, double? p = null) => Code(v);
}
