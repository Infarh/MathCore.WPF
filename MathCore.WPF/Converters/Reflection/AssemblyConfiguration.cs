using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;


/// <summary>Конвертер получения конфигурации из атрибутов сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyCompany))]
public class AssemblyConfiguration() : AssemblyConverter(Attribute<AssemblyConfigurationAttribute>(a => a.Configuration));