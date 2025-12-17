using System.Reflection;
using System.Windows.Markup;

namespace MathCore.WPF.Converters.Reflection;

/// <summary>Возвращает версию файла сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyFileVersion))]
public class AssemblyFileVersion : AssemblyConverter
{
    /// <summary>Возвращает версию файла сборки</summary>
    public AssemblyFileVersion() : base(Attribute<AssemblyFileVersionAttribute>(a => a.Version)) { }
}