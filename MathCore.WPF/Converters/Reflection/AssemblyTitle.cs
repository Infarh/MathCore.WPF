using System.Reflection;
using System.Windows.Markup;

namespace MathCore.WPF.Converters.Reflection;

/// <summary>Возвращает заголовок сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyTitle))]
public class AssemblyTitle : AssemblyConverter
{
    /// <summary>Возвращает заголовок сборки</summary>
    public AssemblyTitle() : base(Attribute<AssemblyTitleAttribute>(a => a.Title)) { }
}