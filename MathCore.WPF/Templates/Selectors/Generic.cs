using System.Collections;
using System.Windows;
using System.Windows.Markup;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Templates.Selectors;

[ContentProperty("Templates")]
[MarkupExtensionReturnType(typeof(GenericDataTemplateSelector))]
public class Generic : MarkupExtension
{
    private readonly List<DataTemplate> _Items = new();

    public IList Templates => _Items;

    public override object ProvideValue(IServiceProvider sp) => new GenericDataTemplateSelector(_Items);
}