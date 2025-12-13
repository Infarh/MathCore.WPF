using System.Reflection;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.Reflection;


/// <summary>Конвертер получения названия компании из атрибутов сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyCompany))]
public class AssemblyCompany() : AssemblyConverter(Attribute<AssemblyCompanyAttribute>(a => a.Company));