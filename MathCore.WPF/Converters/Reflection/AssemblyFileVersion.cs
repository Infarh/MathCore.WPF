using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection
{
    [MarkupExtensionReturnType(typeof(AssemblyFileVersion))]
    public class AssemblyFileVersion : AssemblyConverter
    {
        public AssemblyFileVersion() : base(Attribute<AssemblyFileVersionAttribute>(a => a.Version)) { }
    }
}