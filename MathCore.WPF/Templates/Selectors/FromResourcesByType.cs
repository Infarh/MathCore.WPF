using System.Windows.Markup;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Templates.Selectors;

[MarkupExtensionReturnType(typeof(FromResourcesByTypeTemplateSelector))]
public class FromResourcesByType : MarkupExtension
{
    public override object ProvideValue(IServiceProvider sp) => new FromResourcesByTypeTemplateSelector();
}