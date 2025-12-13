using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;


/// <summary>Конвертер получения торговой марки из атрибутов сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyTrademark))]
public class AssemblyTrademark() : AssemblyConverter(Attribute<AssemblyTrademarkAttribute>(a => a.Trademark));