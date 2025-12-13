using System.Collections;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер извлечения элемента коллекции по индексу</summary>
/// <remarks>
/// Извлекает элемент из массива, списка или перечисления по указанному индексу.
/// Индекс может быть задан через свойство Index или через параметр конвертера.
/// Поддерживает массивы, IList и IEnumerable.
/// <example>
/// <code>
/// &lt;TextBlock Text="{Binding Items, Converter={converters:ArrayElement Index=0}}" /&gt;
/// &lt;Image Source="{Binding Images, Converter={converters:ArrayElement}, ConverterParameter=2}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(ArrayElement))]
[ValueConversion(typeof(IEnumerable), typeof(object))]
public class ArrayElement(int Index) : ValueConverter
{
    /// <summary>Инициализирует конвертер с индексом 0</summary>
    public ArrayElement() : this(0) { }

    /// <summary>Индекс извлекаемого элемента</summary>
    public int Index { get; set; } = Index;

    /// <summary>Извлечение элемента из коллекции по индексу</summary>
    /// <param name="v">Коллекция (массив, IList, IEnumerable)</param>
    /// <param name="t">Целевой тип (не используется)</param>
    /// <param name="p">Параметр конвертера (может содержать индекс)</param>
    /// <param name="c">Информация о культуре (не используется)</param>
    /// <returns>Элемент коллекции по указанному индексу или default если индекс вне диапазона</returns>
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