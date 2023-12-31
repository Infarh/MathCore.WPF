using System.Windows.Markup;

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(TemperatureF2C))]
public class TemperatureF2C() : Linear(1.8, 32);