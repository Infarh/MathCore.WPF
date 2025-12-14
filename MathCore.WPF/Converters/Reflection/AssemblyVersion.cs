using System.Windows.Markup;

namespace MathCore.WPF.Converters.Reflection;

/// <summary>Возвращает версию сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyVersion))]
public class AssemblyVersion : AssemblyConverter
{
    /// <summary>Возвращает версию сборки</summary>
    public AssemblyVersion() : base(a => a.GetName().Version) { }
}