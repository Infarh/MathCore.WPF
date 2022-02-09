using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;

namespace MathCore.WPF;

// ReSharper disable once RedundantExtendsListEntry
public class CollectionViewFiltersCollection : FreezableCollection<CollectionViewFilterItem>, IList
{
    private CollectionViewSource _Source;

    public CollectionViewFiltersCollection(CollectionViewSource item)
    {
        _Source = item;
        ((INotifyCollectionChanged)this).CollectionChanged += OnCollectionChanged;
    }

    public CollectionViewFiltersCollection SetCollectionView(CollectionViewSource item)
    {
        _Source = item;
        return this;
    }

    protected void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems is IEnumerable<CollectionViewFilterItem> added)
                    foreach (var item in added) item.SetSource(_Source);
                break;

            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems is IEnumerable<CollectionViewFilterItem> removed)
                    foreach (var item in removed) item.SetSource(null);
                break;

            case NotifyCollectionChangedAction.Replace:
                if (e.NewItems is IEnumerable<CollectionViewFilterItem> @new)
                    foreach (var item in @new) item.SetSource(_Source);
                if (e.OldItems is IEnumerable<CollectionViewFilterItem> old)
                    foreach (var item in old) item.SetSource(null);
                break;
        }
    }

    /// <inheritdoc />
    protected override Freezable CreateInstanceCore()
    {
        var collection = new CollectionViewFiltersCollection(_Source);
        collection.AddItems(this.Select(f => (CollectionViewFilterItem)f.Clone()));
        return collection;
    }
}