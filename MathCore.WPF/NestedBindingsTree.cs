using System.Globalization;
using System.Windows.Data;

namespace MathCore.WPF;

public class NestedBindingsTree() : NestedBindingNode(-1)
{
    public IMultiValueConverter Converter { get; set; }

    public object ConverterParameter { get; set; }

    public CultureInfo ConverterCulture { get; set; }

    public List<NestedBindingNode> Nodes { get; } = new List<NestedBindingNode>();
}