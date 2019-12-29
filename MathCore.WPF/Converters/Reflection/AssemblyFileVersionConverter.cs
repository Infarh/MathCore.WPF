using System.Reflection;

namespace MathCore.WPF.Converters.Reflection
{
    public class AssemblyFileVersionConverter : AssemblyConverter
    {
        public AssemblyFileVersionConverter() : base(GetAttributeValue<AssemblyFileVersionAttribute>(a => a.Version)) { }
    }
}