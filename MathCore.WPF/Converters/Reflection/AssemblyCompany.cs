using System.Reflection;
using System.Windows.Markup;

namespace MathCore.WPF.Converters.Reflection;

/// <summary>Возвращает компанию авторов сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyCompany))]
public class AssemblyCompany : AssemblyConverter
{
    /// <summary>Инициализирует конвертер, извлекающий значение AssemblyCompanyAttribute.Company</summary>
    public AssemblyCompany() : base(Attribute<AssemblyCompanyAttribute>(a => a.Company)) { }
}