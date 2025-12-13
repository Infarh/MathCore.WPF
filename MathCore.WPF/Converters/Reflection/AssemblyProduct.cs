using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;


/// <summary>Конвертер получения названия продукта из атрибутов сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyProduct))]
public class AssemblyProduct() : AssemblyConverter(Attribute<AssemblyProductAttribute>(a => a.Product));