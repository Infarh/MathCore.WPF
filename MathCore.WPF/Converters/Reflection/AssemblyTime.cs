using System.IO;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;

[MarkupExtensionReturnType(typeof(AssemblyTime))]
public class AssemblyTime() : AssemblyConverter(a => new FileInfo(a.Location).CreationTime);