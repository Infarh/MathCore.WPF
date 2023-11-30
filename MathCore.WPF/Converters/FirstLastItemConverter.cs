using System.Collections;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(FirstItemConverter))]
public class FirstItemConverter : ValueConverter
{
    private static object? GetFirstValue(IEnumerable items)
    {
        var enumerator = items.GetEnumerator();
        try
        {
            return enumerator.MoveNext() ? enumerator.Current : null;
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }
    }

    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) =>
        v switch
        {
            Array { Length: 0 } => null,
            IList { Count: 0 } => null,
            Array array => array.GetValue(0),
            IList and [var first, ..] => first,
            IEnumerable enumerable => GetFirstValue(enumerable),
            _ => Binding.DoNothing
        };
}

[MarkupExtensionReturnType(typeof(LastItemConverter))]
public class LastItemConverter : ValueConverter
{
    private static object? GetLastValue(IEnumerable items)
    {
        var enumerator = items.GetEnumerator();
        try
        {
            if (!enumerator.MoveNext()) return null;

            object? item;
            do
            {
                item = enumerator.Current;
            }
            while (enumerator.MoveNext());

            return item;
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }
    }

    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) =>
        v switch
        {
            Array { Length: 0 } => null,
            IList { Count: 0 } => null,
            Array array => array.GetValue(array.Length - 1),
            IList and [.., var last]  => last,
            IEnumerable enumerable => GetLastValue(enumerable),
            _ => Binding.DoNothing
        };
}