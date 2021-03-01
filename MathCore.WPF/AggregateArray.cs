using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace MathCore.WPF
{
    [ContentProperty("Elements")]
    public class AggregateArray : MarkupExtension
    {
        private readonly ArrayList _Elements = new();

        public IList Elements => _Elements;

        public override object ProvideValue(IServiceProvider sp) => _Elements.Cast<Array>().SelectMany(GetItems).ToArray();

        private static IEnumerable<object> GetItems(object Item)
        {
            if (Item is null) yield break;
            if (!(Item is IEnumerable)) yield return Item;
            else foreach (var item in (IEnumerable)Item) yield return item;
        }
    }
}
