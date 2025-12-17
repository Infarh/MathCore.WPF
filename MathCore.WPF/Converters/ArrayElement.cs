using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Возвращает элемент коллекции по указанному индексу</summary>
[MarkupExtensionReturnType(typeof(ArrayElement))]
[ValueConversion(typeof(IEnumerable), typeof(object))]
public class ArrayElement(int Index) : ValueConverter
{
    public ArrayElement() : this(0) { }

    /// <summary>Индекс элемента для возврата</summary>
    public int Index { get; set; } = Index;

    /// <summary>Пытается разрешить индекс из параметра: поддерживает int, строку и числовые типы</summary>
    private static bool TryResolveIndex(object? p, int defaultIndex, out int index)
    {
        index = defaultIndex;
        if (p is null) return true;
        if (p is int i) { index = i; return true; }
        if (p is string s && int.TryParse(s, out i)) { index = i; return true; }
        try
        {
            index = System.Convert.ToInt32(p);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>Возвращает элемент коллекции по индексу (поддерживает Array, IList и IEnumerable)</summary>
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c)
    {
        if (!TryResolveIndex(p, Index, out var idx)) return Binding.DoNothing;
        if (idx < 0) return Binding.DoNothing;

        return v switch
        {
            Array array when idx < array.Length => array.GetValue(idx),
            IList list when idx < list.Count => list[idx],
            IEnumerable items => items.Cast<object?>().ElementAtOrDefault(idx),
            _ => Binding.DoNothing
        };
    }
}