using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection
{
    [MarkupExtensionReturnType(typeof(AssemblyCompany))]
    public class AssemblyCompany : AssemblyConverter
    {
        public AssemblyCompany() : base(Attribute<AssemblyCompanyAttribute>(a => a.Company)) { }
    }
}