namespace MathCore.WPF.Converters.Reflection
{
    public class AssemblyVersionConverter : AssemblyConverter
    {
        public AssemblyVersionConverter() : base(a => a.GetName().Version) { }
    }
}