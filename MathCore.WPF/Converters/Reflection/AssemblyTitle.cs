using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection
{
    [MarkupExtensionReturnType(typeof(AssemblyTitle))]
    public class AssemblyTitle : AssemblyConverter
    {
        public AssemblyTitle() : base(Attribute<AssemblyTitleAttribute>(a => a.Title)) { }
    }
}