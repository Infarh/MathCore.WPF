using System;
using System.Windows.Markup;

namespace MathCore.WPF.Templates.Selectors
{
    [MarkupExtensionReturnType(typeof(FromResourcesByTypeTemplateSelector))]
    public class FromResourcesByType : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => new FromResourcesByTypeTemplateSelector();
    }
}