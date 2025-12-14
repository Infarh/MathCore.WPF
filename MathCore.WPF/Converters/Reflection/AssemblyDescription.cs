using System.Reflection;
using System.Windows.Markup;

namespace MathCore.WPF.Converters.Reflection;

/// <summary>Возвращает описание сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyDescription))]
public class AssemblyDescription : AssemblyConverter
{
    /// <summary>Возвращает описание сборки</summary>
    public AssemblyDescription() : base(Attribute<AssemblyDescriptionAttribute>(a => a.Description)) { }
}