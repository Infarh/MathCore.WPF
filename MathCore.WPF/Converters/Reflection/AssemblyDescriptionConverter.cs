using System.Reflection;

namespace MathCore.WPF.Converters.Reflection
{
    public class AssemblyDescriptionConverter : AssemblyConverter
    {
        public AssemblyDescriptionConverter() : base(GetAttributeValue<AssemblyDescriptionAttribute>(a => a.Description)) { }
    }
}