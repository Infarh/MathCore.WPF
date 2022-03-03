using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;

[MarkupExtensionReturnType(typeof(AssemblyDescription))]
public class AssemblyDescription : AssemblyConverter
{
    public AssemblyDescription() : base(Attribute<AssemblyDescriptionAttribute>(a => a.Description)) { }
}