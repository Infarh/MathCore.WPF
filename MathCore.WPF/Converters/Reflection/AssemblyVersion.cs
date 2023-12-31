using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;

[MarkupExtensionReturnType(typeof(AssemblyVersion))]
public class AssemblyVersion() : AssemblyConverter(a => a.GetName().Version);