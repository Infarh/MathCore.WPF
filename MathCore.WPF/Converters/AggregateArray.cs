using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;
using MathCore.Annotations;
using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(AggregateArray))]
public class AggregateArray : MultiValueValueConverter
{
    /// <inheritdoc />
    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => vv?.SelectMany(GetItems);

    private static IEnumerable<object?> GetItems(object? Item)
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