using System.Reflection;

namespace MathCore.WPF.Converters.Reflection
{
    public class AssemblyTitleConverter : AssemblyConverter
    {
        public AssemblyTitleConverter() : base(GetAttributeValue<AssemblyTitleAttribute>(a => a.Title)) { }
    }
}