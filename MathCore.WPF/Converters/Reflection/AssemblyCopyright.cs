using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;


/// <summary>Конвертер получения информации об авторских правах из атрибутов сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyCopyright))]
public class AssemblyCopyright() : AssemblyConverter(Attribute<AssemblyCopyrightAttribute>(a => a.Copyright));