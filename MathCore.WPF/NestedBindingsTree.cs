using System.Globalization;
using System.Windows.Data;

namespace MathCore.WPF;

public class NestedBindingsTree : NestedBindingNode
{
    public NestedBindingsTree() : base(-1) => Nodes = new List<NestedBindingNode>();

    public IMultiValueConverter Converter { get; set; }

    public object ConverterParameter { get; set; }

    public CultureInfo ConverterCulture { get; set; }

    public List<NestedBindingNode> Nodes { get; }
}