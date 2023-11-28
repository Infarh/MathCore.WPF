using System.Windows;
using System.Windows.Markup;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace MathCore.WPF.Templates.Selectors;

[ContentProperty("Templates")]
[MarkupExtensionReturnType(typeof(CustomDataTemplateSelector))]
public class Custom : MarkupExtension
{
    private readonly Dictionary<string, DataTemplate> _Items = [];

    public IDictionary<string, DataTemplate> Templates => _Items;

    public Func<object, string> KeySelector { get; set; } = o => o.GetType().Name;

    public override object ProvideValue(IServiceProvider sp) => new CustomDataTemplateSelector(_Items, KeySelector);
}