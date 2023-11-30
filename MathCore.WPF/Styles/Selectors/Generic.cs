using System.Collections;
using System.Windows;
using System.Windows.Markup;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Styles.Selectors;

[ContentProperty("Styles")]
[MarkupExtensionReturnType(typeof(GenericStyleSelector))]
public class Generic : MarkupExtension
{
    private readonly List<Style> _Items = [];

    public IList Styles => _Items;

    public override object ProvideValue(IServiceProvider sp) => new GenericStyleSelector(_Items);
}