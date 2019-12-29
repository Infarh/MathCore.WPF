using System.Reflection;

namespace MathCore.WPF.Converters.Reflection
{
    public class AssemblyCopyrightConverter : AssemblyConverter
    {
        public AssemblyCopyrightConverter() : base(GetAttributeValue<AssemblyCopyrightAttribute>(a => a.Copyright)) { }
    }
}