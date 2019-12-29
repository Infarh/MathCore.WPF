using System.Reflection;

namespace MathCore.WPF.Converters.Reflection
{
    public class AssemblyProductConverter : AssemblyConverter
    {
        public AssemblyProductConverter() : base(GetAttributeValue<AssemblyProductAttribute>(a => a.Product)) { }
    }
}