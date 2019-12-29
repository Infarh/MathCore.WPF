using System.Reflection;

namespace MathCore.WPF.Converters.Reflection
{
    public class AssemblyConfigurationConverter : AssemblyConverter
    {
        public AssemblyConfigurationConverter() : base(GetAttributeValue<AssemblyConfigurationAttribute>(a => a.Configuration)) { }
    }
}