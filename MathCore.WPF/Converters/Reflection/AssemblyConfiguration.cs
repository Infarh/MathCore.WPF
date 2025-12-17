using System.Reflection;
using System.Windows.Markup;

namespace MathCore.WPF.Converters.Reflection;

/// <summary>Возвращает конфигурацию сборки</summary>
[MarkupExtensionReturnType(typeof(AssemblyConfiguration))]
public class AssemblyConfiguration : AssemblyConverter
{
    /// <summary>Возвращает конфигурацию сборки</summary>
    public AssemblyConfiguration() : base(Attribute<AssemblyConfigurationAttribute>(a => a.Configuration)) { }
}