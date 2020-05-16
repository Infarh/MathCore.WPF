using System;
using System.Windows.Markup;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Templates.Selectors
{
    [MarkupExtensionReturnType(typeof(FromResourcesByTypeTemplateSelector))]
    public class FromResourcesByType : MarkupExtension
    {
        [NotNull] public override object ProvideValue(IServiceProvider sp) => new FromResourcesByTypeTemplateSelector();
    }
}