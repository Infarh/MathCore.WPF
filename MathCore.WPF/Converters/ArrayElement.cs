using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(ArrayElement))]
[ValueConversion(typeof(IEnumerable), typeof(object))]
public class ArrayElement(int Index) : ValueConverter
{
    public ArrayElement() : this(0) { }

    public int Index { get; set; } = Index;

    /// <inheritdoc />
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => (v, p) switch
    {
        (Array array, int index) => index < array.Length ? array.GetValue(index) : default,
        (Array array, _) => Index < array.Length ? array.GetValue(Index) : default,
        (IList list, int index) => index < list.Count ? list[index] : default,
        (IList list, _) => Index < list.Count ? list[Index] : default,
        (IEnumerable items, int index) => items.Cast<object>().ElementAtOrDefault(index),
        (IEnumerable items, _) => items.Cast<object>().ElementAtOrDefault(Index),
        _ => Binding.DoNothing
    };
}