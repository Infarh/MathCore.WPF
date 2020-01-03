using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection
{
    [MarkupExtensionReturnType(typeof(AssemblyCompany))]
    public class AssemblyCopyright : AssemblyConverter
    {
        public AssemblyCopyright() : base(Attribute<AssemblyCopyrightAttribute>(a => a.Copyright)) { }
    }
}