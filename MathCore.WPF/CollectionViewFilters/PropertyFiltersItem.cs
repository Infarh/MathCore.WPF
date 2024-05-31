using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

using MathCore.WPF.Commands;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF;

public class PropertyFiltersItem : CollectionViewFilterItem, ICollection<PropertyFilterItem>, INotifyCollectionChanged, IList
{
    #region CollectionItemType : Type

    /// <summary></summary>
    public static readonly DependencyProperty CollectionItemTypeProperty =
        DependencyProperty.Register(
            nameof(CollectionItemType),
            typeof(Type),
            typeof(PropertyFiltersItem),
            new(default(Type), OnCollectionItemTypeChanged));

    private static void OnCollectionItemTypeChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        if (D is not PropertyFiltersItem filters_item) return;
        foreach (var filter in filters_item)
            filter.ItemType = (Type)E.NewValue;
    }

    /// <summary></summary>
    public Type CollectionItemType
    {
        get => (Type)GetValue(CollectionItemTypeProperty); 
        set => SetValue(CollectionItemTypeProperty, value);
    }

    #endregion

    private readonly ObservableCollection<PropertyFilterItem> _Filters = [];

    public ICommand AddNewFilterCommand { get; }

    public ICommand RemoveCommand { get; }

    public PropertyFiltersItem()
    {
        _Filters.CollectionChanged += OnFiltersCollection_Changed;

        AddNewFilterCommand = new LambdaCommand(OnAddNewCommandExecuted);
        RemoveCommand = new LambdaCommand(OnRemoveCommandExecuted);
        _Filters.Add(new());
    }

    private void OnAddNewCommandExecuted(object? Obj) => _Filters.Add(Obj as PropertyFilterItem ?? new());

    private void OnRemoveCommandExecuted(object? Obj)
    {
        if (Obj is PropertyFilterItem filter)
            Remove(filter); 
        else 
            Clear();
    }

    private void OnFiltersCollection_Changed(object? Sender, NotifyCollectionChangedEventArgs E)
    {
        switch (E.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (E.NewItems is { } added)
                    foreach (PropertyFilterItem? item in added)
                    {
                        item!.ItemType = CollectionItemType;
                        item.PropertyChanged += OnPropertyFilterItemChanged;
                    }
                break;

            case NotifyCollectionChangedAction.Remove:
                if (E.OldItems is { } removed)
                    foreach (PropertyFilterItem? item in removed)
                        item!.PropertyChanged -= OnPropertyFilterItemChanged;
                break;

            case NotifyCollectionChangedAction.Replace:
                if (E.OldItems is { } old_items)
                    foreach (PropertyFilterItem? item in old_items)
                        item!.PropertyChanged -= OnPropertyFilterItemChanged;
                if (E.NewItems is { } new_items)
                    foreach (PropertyFilterItem? item in new_items)
                    {
                        item!.ItemType = CollectionItemType;
                        item.PropertyChanged += OnPropertyFilterItemChanged;
                    }
                break;
        }
    }

    private void OnPropertyFilterItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PropertyFilterItem.Enabled):
            case nameof(PropertyFilterItem.Property):
            case nameof(PropertyFilterItem.Value):
            case nameof(PropertyFilterItem.CanBeNull):
            case nameof(PropertyFilterItem.Comparison):
            case nameof(PropertyFilterItem.ItemType):
                RefreshSource();
                break;
        }
    }

    protected override Freezable CreateInstanceCore() => throw new NotSupportedException();

    protected override void OnFilter(object Sender, FilterEventArgs E)
    {
        if (!E.Accepted) return;
        foreach (var filter in _Filters)
            if (!filter.Filter(E.Item))
            {
                E.Accepted = false;
                break;
            }


        //if (_Filters.Any(f => !f.Filter(E.Item)))
        //    E.Accepted = false;

    }

    #region ICollection<PropertyFilterItem>

    /// <inheritdoc />
    IEnumerator<PropertyFilterItem> IEnumerable<PropertyFilterItem>.GetEnumerator() => _Filters.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Filters).GetEnumerator();

    /// <inheritdoc />
    public void Add(PropertyFilterItem item) => _Filters.Add(item);

    public int Add(object? value) => ((IList)_Filters).Add(value);

    public bool Contains(object? value) => ((IList)_Filters).Contains(value);

    public void Clear()
    {
        foreach (var filter in _Filters)
            filter.PropertyChanged -= OnPropertyFilterItemChanged;

        _Filters.Clear();
    }

    public int IndexOf(object? value) => ((IList)_Filters).IndexOf(value);

    public void Insert(int index, object? value) => ((IList)_Filters).Insert(index, value);

    public void Remove(object? value) => ((IList)_Filters).Remove(value);

    public void RemoveAt(int index) => _Filters.RemoveAt(index);

    public object? this[int index]
    {
        get => ((IList)_Filters)[index];
        set => ((IList)_Filters)[index] = value;
    }

    public bool IsReadOnly => ((IList)_Filters).IsReadOnly;

    public bool IsFixedSize => ((IList)_Filters).IsFixedSize;

    /// <inheritdoc />
    bool ICollection<PropertyFilterItem>.Contains(PropertyFilterItem? item) => item is not null && _Filters.Contains(item);

    /// <inheritdoc />
    void ICollection<PropertyFilterItem>.CopyTo(PropertyFilterItem[] array, int index) => _Filters.CopyTo(array, index);

    /// <inheritdoc />
    public bool Remove(PropertyFilterItem item) => _Filters.Remove(item);

    public void CopyTo(Array array, int index) => ((ICollection)_Filters).CopyTo(array, index);

    public int Count => _Filters.Count;

    public object SyncRoot => ((ICollection)_Filters).SyncRoot;

    public bool IsSynchronized => ((ICollection)_Filters).IsSynchronized;

    /// <inheritdoc />
    int ICollection<PropertyFilterItem>.Count => _Filters.Count;

    /// <inheritdoc />
    bool ICollection<PropertyFilterItem>.IsReadOnly => false;

    #endregion

    #region INotifyCollectionChanged

    /// <inheritdoc />
    event NotifyCollectionChangedEventHandler? INotifyCollectionChanged.CollectionChanged
    {
        add => _Filters.CollectionChanged += value;
        remove => _Filters.CollectionChanged -= value;
    }

    #endregion
}