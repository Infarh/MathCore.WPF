using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(AggregateArray))]
    public class AggregateArray : MultiValueValueConverter
    {
        /// <inheritdoc />
        [CanBeNull]
        protected override object Convert([CanBeNull] object[]? vv, Type? t, object? p, CultureInfo? c) => vv?.SelectMany(GetItems)!;

        private static IEnumerable<object?> GetItems([CanBeNull] object? Item)
        {
            if(Item is null) yield break;
            if(!(Item is IEnumerable)) yield return Item;
            else foreach(var item in (IEnumerable)Item) yield return item;
        }
    }
}