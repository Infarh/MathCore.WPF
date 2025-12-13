using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;


/// <summary>Конвертер получения версии из атрибутов сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyVersion))]
public class AssemblyVersion() : AssemblyConverter(a => a.GetName().Version);