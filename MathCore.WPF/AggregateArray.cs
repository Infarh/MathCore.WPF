using System.Collections;
using System.Windows.Markup;
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF;

[ContentProperty("Elements")]
public class AggregateArray : MarkupExtension
{
    private readonly ArrayList _Elements = new();

    public IList Elements => _Elements;

    public override object ProvideValue(IServiceProvider sp) => _Elements.Cast<Array>().SelectMany(GetItems).ToArray();

    private static IEnumerable<object> GetItems(object? Item)
    {
        switch (Item)
        {
            case null: yield break;
            case IEnumerable enumerable:
                foreach (var item in enumerable)
                    yield return item;
                break;
            default: yield return Item;
                break;
        }
    }
}