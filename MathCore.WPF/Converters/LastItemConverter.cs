using System.Collections;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Конвертер извлечения последнего элемента коллекции</summary>
/// <remarks>
/// Извлекает последний элемент из массива, списка или перечисления.
/// Возвращает null для пустой коллекции.
/// <example>
/// <code>
/// &lt;TextBlock Text="{Binding RecentItems, Converter={converters:LastItemConverter}}" /&gt;
/// &lt;Image Source="{Binding PhotoList, Converter={converters:LastItemConverter}}" /&gt;
/// </code>
/// </example>
/// </remarks>
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

    /// <summary>Извлечение последнего элемента из коллекции</summary>
    /// <param name="v">Коллекция (массив, IList, IEnumerable)</param>
    /// <param name="t">Целевой тип</param>
    /// <param name="p">Параметр конвертера (не используется)</param>
    /// <param name="c">Информация о культуре</param>
    /// <returns>Последний элемент коллекции или null если коллекция пуста</returns>
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