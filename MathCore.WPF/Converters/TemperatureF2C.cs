using System.Windows.Markup;

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(TemperatureF2C))]
public class TemperatureF2C : Linear
{
    public TemperatureF2C() : base(1.8, 32) { }
}