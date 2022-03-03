using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;

[MarkupExtensionReturnType(typeof(AssemblyTrademark))]
public class AssemblyTrademark : AssemblyConverter
{
    public AssemblyTrademark() : base(Attribute<AssemblyTrademarkAttribute>(a => a.Trademark)) { }
}