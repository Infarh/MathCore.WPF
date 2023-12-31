using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MathCore.WPF;

public class CollectionFilter<TValue, TCriteria> : ReadOnlyObservableCollection<CollectionFilterItem<TValue, TCriteria>>
{
    private readonly ObservableCollection<CollectionFilterItem<TValue, TCriteria>> _InternalCollection;

    private readonly ObservableCollection<TValue?>? _Collection;

    private readonly Func<TValue, TCriteria>? _Selector;

    private CollectionFilter(ObservableCollection<CollectionFilterItem<TValue, TCriteria>> internal_collection)
        : base(internal_collection) =>
        _InternalCollection = internal_collection;

    public CollectionFilter(ObservableCollection<TValue?> collection, Func<TValue, TCriteria> selector)
        : this([])
    {
        _Collection = collection;
        _Selector = selector;
        collection.CollectionChanged += OnCollectionChanged;
    }

    private void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
    {
        switch (E.Action)
        {
            default: throw new InvalidOperationException();

            case NotifyCollectionChangedAction.Add:
                if (E.NewItems is IEnumerable<TValue> added) AddValues(added);
                break;
            case NotifyCollectionChangedAction.Remove:
                if (E.OldItems is IEnumerable<TValue> removed) RemoveValues(removed);
                break;

            case NotifyCollectionChangedAction.Replace:
                if (E.OldItems is IEnumerable<TValue> old_items) RemoveValues(old_items);
                if (E.NewItems is IEnumerable<TValue> new_items) AddValues(new_items);
                break;

            case NotifyCollectionChangedAction.Move: break;
            case NotifyCollectionChangedAction.Reset:
                ResetCollection();
                break;
        }
    }

    private void ResetCollection()
    {
        _InternalCollection.Clear();
        if (_Collection.Count == 0 || _Selector is not { } selector) return;

        foreach (var group in _Collection!.GroupBy(selector))
            _InternalCollection.Add(new(group.Key!, group));
    }

    private void AddValues(IEnumerable<TValue> values)
    {
        if(_Selector is not { } selector) return;

        foreach (var value in values)
        {
            var key = selector(value);
            var filter = _InternalCollection.FirstOrDefault(f => Equals(f.Key, key));
            if (filter is null) _InternalCollection.Add(filter = new(key));
            filter.Add(value);
        }
    }

    private void RemoveValues(IEnumerable<TValue> values)
    {
        if(_Selector is not { } selector) return;

        foreach (var value in values)
        {
            var key = selector(value);
            var filter = _InternalCollection.FirstOrDefault(f => Equals(f.Key, key));
            filter?.Remove(value);
            if (filter is { Count: > 0 }) _InternalCollection.Remove(filter);
        }
    }
}