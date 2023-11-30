using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;

[MarkupExtensionReturnType(typeof(AssemblyCompany))]
public class AssemblyConfiguration() : AssemblyConverter(Attribute<AssemblyConfigurationAttribute>(a => a.Configuration));