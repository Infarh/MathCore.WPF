using System.Reflection;
using System.Windows.Markup;

namespace MathCore.WPF.Converters.Reflection;

/// <summary>Возвращает торговую марку сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyTrademark))]
public class AssemblyTrademark : AssemblyConverter
{
    /// <summary>Возвращает торговую марку сборки</summary>
    public AssemblyTrademark() : base(Attribute<AssemblyTrademarkAttribute>(a => a.Trademark)) { }
}