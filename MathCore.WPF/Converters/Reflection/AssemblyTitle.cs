using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;


/// <summary>Конвертер получения заголовка из атрибутов сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyTitle))]
public class AssemblyTitle() : AssemblyConverter(Attribute<AssemblyTitleAttribute>(a => a.Title));