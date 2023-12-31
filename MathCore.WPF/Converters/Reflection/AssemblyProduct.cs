using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;

[MarkupExtensionReturnType(typeof(AssemblyProduct))]
public class AssemblyProduct() : AssemblyConverter(Attribute<AssemblyProductAttribute>(a => a.Product));