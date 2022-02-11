using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF;

[Copyright("adeptuss", url = "https://habr.com/ru/post/277157/")]
[ContentProperty(nameof(Bindings))]
public class NestedBinding : MarkupExtension
{
    public NestedBinding() => Bindings = new Collection<BindingBase>();

    public Collection<BindingBase> Bindings { get; }

    public IMultiValueConverter Converter { get; set; }

    public object ConverterParameter { get; set; }

    public CultureInfo ConverterCulture { get; set; }

    public override object ProvideValue(IServiceProvider Services)
    {
        if (Bindings.Count == 0)
            throw new ArgumentNullException(nameof(Bindings));
        if (Converter is null)
            throw new ArgumentNullException(nameof(Converter));

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
            var binding = binding_base as Binding;
            if (binding?.Source is NestedBinding child_nested_binding && binding.Converter == null)
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

public class NestedBindingNode
{
    public NestedBindingNode(int index) => Index = index;

    public int Index { get; }

    public override string ToString() => Index.ToString();
}

public class NestedBindingsTree : NestedBindingNode
{
    public NestedBindingsTree() : base(-1) => Nodes = new List<NestedBindingNode>();

    public IMultiValueConverter Converter { get; set; }

    public object ConverterParameter { get; set; }

    public CultureInfo ConverterCulture { get; set; }

    public List<NestedBindingNode> Nodes { get; }
}

public class NestedBindingConverter : IMultiValueConverter
{
    public NestedBindingConverter(NestedBindingsTree tree) => Tree = tree;

    private NestedBindingsTree Tree { get; }

    public object Convert(object[] values, Type TargetType, object parameter, CultureInfo culture)
    {
        var value = GetTreeValue(Tree, values, TargetType, culture);
        return value;
    }

    private static object GetTreeValue(NestedBindingsTree tree, object[] values, Type TargetType, CultureInfo culture)
    {
        //var objects = tree.Nodes
        //   .Select(x => x is NestedBindingsTree bindings_tree
        //        ? GetTreeValue(bindings_tree, values, TargetType, culture)
        //        : values[x.Index])
        //   .ToArray();

        var objects = new object[tree.Nodes.Count];
        for (var i = 0; i < objects.Length; i++)
        {
            var element = tree.Nodes[i];
            objects[i] = element is NestedBindingsTree bindings_tree
                ? GetTreeValue(bindings_tree, values, TargetType, culture)
                : values[element.Index];
        }

        var value = tree.Converter.Convert(objects, TargetType, tree.ConverterParameter, tree.ConverterCulture ?? culture);
        return value;
    }

    public object[] ConvertBack(object value, Type[] TargetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class JoinStringConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type TargetType, object parameter, CultureInfo culture)
    {
        var separator = parameter as string ?? " ";
        return string.Join(separator, values);
    }

    public object[] ConvertBack(object value, Type[] TargetTypes, object parameter, CultureInfo culture)
    {
        var separator = parameter as string ?? " ";
        return (value as string)?.Split(new[] { separator }, StringSplitOptions.None).Cast<object>().ToArray();
    }
}