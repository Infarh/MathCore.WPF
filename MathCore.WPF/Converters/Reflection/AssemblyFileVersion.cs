using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;


/// <summary>Конвертер получения версии файла из атрибутов сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyFileVersion))]
public class AssemblyFileVersion() : AssemblyConverter(Attribute<AssemblyFileVersionAttribute>(a => a.Version));