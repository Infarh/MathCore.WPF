using System.Globalization;
using System.Windows.Data;

namespace MathCore.WPF;

public class NestedBindingConverter : IMultiValueConverter
{
    public NestedBindingConverter(NestedBindingsTree tree) => Tree = tree;

    private NestedBindingsTree Tree { get; }

    public object? Convert(object[]? values, Type TargetType, object? parameter, CultureInfo culture)
    {
        var value = GetTreeValue(Tree, values, TargetType, culture);
        return value;
    }

    private static object? GetTreeValue(NestedBindingsTree tree, object[]? values, Type TargetType, CultureInfo culture)
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

    public object?[]? ConvertBack(object? value, Type[] TargetTypes, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}