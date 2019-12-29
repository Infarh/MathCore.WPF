using System.IO;

namespace MathCore.WPF.Converters.Reflection
{
    public class AssemblyTimeConverter : AssemblyConverter
    {
        public AssemblyTimeConverter() : base(a => new FileInfo(a.Location).CreationTime) { }
    }
}