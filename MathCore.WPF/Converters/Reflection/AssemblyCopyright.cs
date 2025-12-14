using System.Reflection;
using System.Windows.Markup;

namespace MathCore.WPF.Converters.Reflection;

/// <summary>Возвращает данные об авторских правах сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyCopyright))]
public class AssemblyCopyright : AssemblyConverter
{
    /// <summary>Возвращает данные об авторских правах сборки</summary>
    public AssemblyCopyright() : base(Attribute<AssemblyCopyrightAttribute>(a => a.Copyright)) { }
}