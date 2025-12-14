using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Разворачивает вложенные перечисления в одну последовательность</summary>
[MarkupExtensionReturnType(typeof(AggregateArray))]
public class AggregateArray : MultiValueValueConverter
{
    /// <summary>Преобразует массив значений в одну плоскую последовательность, разворачивая вложенныеenumerations</summary>
    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => vv?.SelectMany(GetItems);

    /// <summary>Возвращает элементы, если вход — перечисление, иначе возвращает сам элемент; пропускает null</summary>
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