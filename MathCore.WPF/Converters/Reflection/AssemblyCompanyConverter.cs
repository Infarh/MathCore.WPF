using System.Reflection;

namespace MathCore.WPF.Converters.Reflection
{
    public class AssemblyCompanyConverter : AssemblyConverter
    {
        public AssemblyCompanyConverter() : base(GetAttributeValue<AssemblyCompanyAttribute>(a => a.Company)) { }
    }
}