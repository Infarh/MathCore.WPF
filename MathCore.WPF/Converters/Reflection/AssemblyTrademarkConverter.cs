using System.Reflection;

namespace MathCore.WPF.Converters.Reflection
{
    public class AssemblyTrademarkConverter : AssemblyConverter
    {
        public AssemblyTrademarkConverter() : base(GetAttributeValue<AssemblyTrademarkAttribute>(a => a.Trademark)) { }
    }
}