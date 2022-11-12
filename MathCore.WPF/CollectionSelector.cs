using System.Collections.ObjectModel;

using MathCore.WPF.ViewModels;
// ReSharper disable UnusedType.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF;

/// <summary>Селектор элементов в коллекции</summary>
/// <typeparam name="T">Тип элементов коллекции</typeparam>
public class CollectionSelector<T> : ViewModel
{
    #region Items : IEnumerable<T> - Коллекция

    /// <summary>Коллекция</summary>
    private IEnumerable<T>? _Items;

    /// <summary>Коллекция</summary>
    public IEnumerable<T>? Items
    {
        get => _Items;
        set
        {
            if (Set(ref _Items, value))
                SelectedItem = value switch
                {
                    T[] { Length                   : > 0 } array       => array[0],
                    List<T> { Count                : > 0 } list        => list[0],
                    LinkedList<T> { First.Value    : { } first_value } => first_value, 
                    ObservableCollection<T> { Count: > 0 } collection  => collection[0],
                    IList<T> { Count               : > 0 } list        => list[0],
                    { } items                                          => items.FirstOrDefault(),
                    _                                                  => default
                };
        }
    }

    #endregion

    #region SelectedItem : T - Выбранный элемент

    /// <summary>Выбранный элемент</summary>
    private T? _SelectedItem;

    /// <summary>Выбранный элемент</summary>
    public T? SelectedItem { get => _SelectedItem; set => Set(ref _SelectedItem, value); }

    #endregion

    #region SelectFirstItem : bool - Выбирать первый элемент для нового значения коллекции

    /// <summary>Выбирать первый элемент для нового значения коллекции</summary>
    private bool _SelectFirstItem;

    /// <summary>Выбирать первый элемент для нового значения коллекции</summary>
    public bool SelectFirstItem { get => _SelectFirstItem; set => Set(ref _SelectFirstItem, value); }

    #endregion

    public CollectionSelector(bool SelectFirstItem = true) => _SelectFirstItem = SelectFirstItem;

    public CollectionSelector(IEnumerable<T> Items, bool SelectFirstItem = true) : this(SelectFirstItem) => this.Items = Items;

    public static implicit operator CollectionSelector<T>(T[] Collection) => new(Collection);
    public static implicit operator CollectionSelector<T>(List<T> Collection) => new(Collection);
    public static implicit operator CollectionSelector<T>(ObservableCollection<T> Collection) => new(Collection);
}