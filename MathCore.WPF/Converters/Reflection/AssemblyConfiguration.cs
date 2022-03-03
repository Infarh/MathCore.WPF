using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;

[MarkupExtensionReturnType(typeof(AssemblyCompany))]
public class AssemblyConfiguration : AssemblyConverter
{
    public AssemblyConfiguration() : base(Attribute<AssemblyConfigurationAttribute>(a => a.Configuration)) { }
}