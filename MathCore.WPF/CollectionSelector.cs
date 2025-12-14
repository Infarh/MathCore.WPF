using System.Collections.ObjectModel;

using MathCore.WPF.ViewModels;
// ReSharper disable UnusedType.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF;

/// <summary>Селектор элементов в коллекции</summary>
/// <typeparam name="T">Тип элементов коллекции</typeparam>
public class CollectionSelector<T>(bool SelectFirstItem = true) : ViewModel
{
    #region Items : IEnumerable<T> - Коллекция

    /// <summary>Коллекция</summary>
    public IEnumerable<T>? Items
    {
        get;
        set
        {
            if (Set(ref field, value))
                SelectedItem = value switch
                {
                    T[] { Length: > 0 } array => array[0],
                    List<T> { Count: > 0 } list => list[0],
                    LinkedList<T> { First.Value: { } first_value } => first_value,
                    ObservableCollection<T> { Count: > 0 } collection => collection[0],
                    IList<T> { Count: > 0 } list => list[0],
                    { } items => items.FirstOrDefault(),
                    _ => default
                };
        }
    }

    #endregion

    /// <summary>Выбранный элемент</summary>
    public T? SelectedItem { get; set => Set(ref field, value); }

    /// <summary>Выбирать первый элемент для нового значения коллекции</summary>
    public bool SelectFirstItem { get; set => Set(ref field, value); } = SelectFirstItem;

    public CollectionSelector(IEnumerable<T> Items, bool SelectFirstItem = true) : this(SelectFirstItem) => this.Items = Items;

    public static implicit operator CollectionSelector<T>(T[] Collection) => new(Collection);
    public static implicit operator CollectionSelector<T>(List<T> Collection) => new(Collection);
    public static implicit operator CollectionSelector<T>(ObservableCollection<T> Collection) => new(Collection);
}