using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection
{
    [MarkupExtensionReturnType(typeof(AssemblyVersion))]
    public class AssemblyVersion : AssemblyConverter
    {
        public AssemblyVersion() : base(a => a.GetName().Version) { }
    }
}