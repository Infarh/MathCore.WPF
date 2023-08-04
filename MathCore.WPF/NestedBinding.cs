using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF;

[Copyright("adeptuss", url = "https://habr.com/ru/post/277157/")]
[ContentProperty(nameof(Bindings))]
[MarkupExtensionReturnType(typeof(object))]
public class NestedBinding : MarkupExtension
{
    public Collection<BindingBase> Bindings { get; } = new();

    public IMultiValueConverter Converter { get; set; }

    public object ConverterParameter { get; set; }

    public CultureInfo ConverterCulture { get; set; }

    public override object ProvideValue(IServiceProvider Services)
    {
        if (Bindings.NotNull().Count == 0)
            throw new ArgumentException(nameof(Bindings));
        if (Converter is null)
            throw new InvalidOperationException("Не заданы Converter");

        var target = (IProvideValueTarget)Services.GetService(typeof(IProvideValueTarget));
        if (target.TargetObject is Collection<BindingBase>)
        {
            var binding = new Binding { Source = this };
            return binding;
        }

        var multi_binding = new MultiBinding { Mode = BindingMode.OneWay };
        var tree = GetNestedBindingsTree(this, multi_binding);
        var converter = new NestedBindingConverter(tree);
        multi_binding.Converter = converter;

        return multi_binding.ProvideValue(Services);
    }

    private static NestedBindingsTree GetNestedBindingsTree(NestedBinding NestedBinding, MultiBinding MultiBinding)
    {
        var tree = new NestedBindingsTree
        {
            Converter = NestedBinding.Converter,
            ConverterParameter = NestedBinding.ConverterParameter,
            ConverterCulture = NestedBinding.ConverterCulture
        };

        foreach (var binding_base in NestedBinding.Bindings)
        {
            if (binding_base is Binding { Source: NestedBinding child_nested_binding, Converter: null })
            {
                tree.Nodes.Add(GetNestedBindingsTree(child_nested_binding, MultiBinding));
                continue;
            }

            tree.Nodes.Add(new NestedBindingNode(MultiBinding.Bindings.Count));
            MultiBinding.Bindings.Add(binding_base);
        }

        return tree;
    }
}