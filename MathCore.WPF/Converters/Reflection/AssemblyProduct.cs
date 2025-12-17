using System.Reflection;
using System.Windows.Markup;

namespace MathCore.WPF.Converters.Reflection;

/// <summary>Возвращает продукт сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyProduct))]
public class AssemblyProduct : AssemblyConverter
{
    /// <summary>Возвращает продукт сборки</summary>
    public AssemblyProduct() : base(Attribute<AssemblyProductAttribute>(a => a.Product)) { }
}