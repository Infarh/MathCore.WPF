using System.Windows.Markup;

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(TemperatureC2F))]
public class TemperatureC2F : Linear
{
    public TemperatureC2F() : base(1 / 1.8, - 32 / 1.8) { }
}