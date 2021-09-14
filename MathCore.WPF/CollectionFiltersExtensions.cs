using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF
{
    public class CollectionFilterItem<TValue, TCriteria> : ReadOnlyObservableCollection<TValue>
    {
        private readonly ObservableCollection<TValue?> _InternalCollection;
        public TCriteria? Key { get; }

        private bool _Enabled;
        public bool Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled == value) return;
                _Enabled = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Enabled)));
            }
        }

        public CollectionFilterItem(ObservableCollection<TValue?> list) : base(list) => _InternalCollection = list;
        public CollectionFilterItem(TCriteria key) : this(new ObservableCollection<TValue>()) => Key = key;

        public CollectionFilterItem([NotNull] TCriteria key, IEnumerable<TValue?> items) : this(new ObservableCollection<TValue>(items)) => Key = key;

        internal void Add(TValue? value) { if (!_InternalCollection.Contains(value)) _InternalCollection.Add(value); }

        internal bool Remove(TValue? value) => _InternalCollection.Remove(value);
    }

    public class CollectionFilter<TValue, TCriteria> : ReadOnlyObservableCollection<CollectionFilterItem<TValue, TCriteria>>
    {
        private readonly ObservableCollection<CollectionFilterItem<TValue, TCriteria>> _InternalCollection;
        private readonly ObservableCollection<TValue?>? _Collection;
        private readonly Func<TValue, TCriteria>? _Selector;

        private CollectionFilter(ObservableCollection<CollectionFilterItem<TValue, TCriteria>> internal_collection)
            : base(internal_collection) =>
            _InternalCollection = internal_collection;

        public CollectionFilter(ObservableCollection<TValue?> collection, Func<TValue, TCriteria> selector)
            : this(new ObservableCollection<CollectionFilterItem<TValue, TCriteria>>())
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
                    if (E.OldItems is IEnumerable<TValue> old) RemoveValues(old);
                    if (E.NewItems is IEnumerable<TValue> @new) AddValues(@new);
                    break;
                case NotifyCollectionChangedAction.Move: break;
                case NotifyCollectionChangedAction.Reset:
                    _InternalCollection.Clear();
                    if (_Collection is not { Count: > 0 } collection) break;
                    foreach (var group in collection!.GroupBy(_Selector!))
                        _InternalCollection.Add(new CollectionFilterItem<TValue, TCriteria>(group.Key, group));
                    break;
            }
        }

        private void AddValues(IEnumerable<TValue> values)
        {
            if(_Selector is not { } selector) return;
            foreach (var value in values)
            {
                var key = selector(value);
                var filter = _InternalCollection.FirstOrDefault(f => Equals(f.Key, key));
                if (filter is null) _InternalCollection.Add(filter = new CollectionFilterItem<TValue, TCriteria>(key));
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

    public class CollectionViewFilterItem<TCriteria> : ReadOnlyObservableCollection<object?>
    {
        private readonly ObservableCollection<object?> _InternalCollection;

        private bool _Enabled;

        public bool Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled == value) return;
                _Enabled = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Enabled)));
                EnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _ExistInView;

        public bool ExistInView
        {
            get => _ExistInView;
            internal set
            {
                if (_ExistInView == value) return;
                _ExistInView = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(ExistInView)));
            }
        }

        public event EventHandler? EnabledChanged;

        public TCriteria? Key { get; }

        private CollectionViewFilterItem(ObservableCollection<object?> collection) : base(collection) => _InternalCollection = collection;
        public CollectionViewFilterItem(TCriteria? key) : this(new ObservableCollection<object?>()) => Key = key;

        public CollectionViewFilterItem(TCriteria? key, IEnumerable items) : this(new ObservableCollection<object?>(items.Cast<object?>())) => Key = key;

        internal void Add(object? value) { if (!_InternalCollection.Contains(value)) _InternalCollection.Add(value); }

        internal bool Remove(object? value) => _InternalCollection.Remove(value);
    }

    public class CollectionViewFilter<TCriteria> : ReadOnlyObservableCollection<CollectionViewFilterItem<TCriteria>>
    {
        private readonly ICollectionView? _View;
        private readonly Func<object, TCriteria>? _Selector;
        private readonly ObservableCollection<CollectionViewFilterItem<TCriteria>> _FiltersCollection;
        private readonly Dictionary<TCriteria, CollectionViewFilterItem<TCriteria>> _Filters = new();
        private bool _Enabled;
        private bool _AllFiltersDisabled = true;
        private string? _Name;

        public bool Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled == value) return;
                _Enabled = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Enabled)));
                _View?.Refresh();
            }
        }

        public string? Name
        {
            get => _Name;
            set
            {
                if (string.Equals(_Name, value)) return;
                _Name = value;
                NameChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler? NameChanged;

        private CollectionViewFilter(ObservableCollection<CollectionViewFilterItem<TCriteria>> filters) : base(filters) => _FiltersCollection = filters;

        public CollectionViewFilter(ICollectionView view, Func<object, TCriteria> selector, string? Name = null) : this(new ObservableCollection<CollectionViewFilterItem<TCriteria>>())
        {
            _Name = Name;
            _View = view;
            _Selector = selector;
            ((INotifyCollectionChanged)view.SourceCollection).CollectionChanged += OnCollectionChanged;
            view.CollectionChanged += OnViewCollectionChanged;
        }

        private void OnViewCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs? E)
        {
            if (_View is not { } view || _Selector is not { } selector) return;
            var keys = view.OfType<object>().GroupBy(selector).Select(g => g.Key).ToList();
            foreach (var filter in _FiltersCollection)
                if (filter.Key is { } key)
                    filter.ExistInView = keys.Contains(key);
        }

        private void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
        {
            switch (E.Action)
            {
                default: throw new InvalidOperationException();
                case NotifyCollectionChangedAction.Add:
                    if (E.NewItems is IEnumerable<TCriteria> added) AddValues(added);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (E.OldItems is IEnumerable<TCriteria> removed) RemoveValues(removed);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (E.OldItems is IEnumerable<TCriteria> old) RemoveValues(old);
                    if (E.NewItems is IEnumerable<TCriteria> @new) AddValues(@new);
                    break;
                case NotifyCollectionChangedAction.Move: break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var filter in _FiltersCollection) filter.EnabledChanged -= OnFilterEnableChanged;
                    _FiltersCollection.Clear();
                    _Filters.Clear();

                    if (_View is not { IsEmpty: false } || _Selector is not { } selector) break;
                    foreach (var group in _View.SourceCollection.Cast<object>().GroupBy(selector))
                    {
                        var filter = new CollectionViewFilterItem<TCriteria>(group.Key, group);
                        filter.EnabledChanged += OnFilterEnableChanged;
                        _FiltersCollection.Add(filter);
                        _Filters.Add(group.Key, filter);
                    }

                    break;
            }
        }

        private void AddValues([ItemCanBeNull] IEnumerable values)
        {
            if (_Selector is not { } selector) return;
            foreach (var value in values)
            {
                var key = selector(value);
                if (!_Filters.TryGetValue(key, out var filter))
                {
                    filter = new CollectionViewFilterItem<TCriteria>(key);
                    filter.EnabledChanged += OnFilterEnableChanged;
                    _Filters.Add(key, filter);
                    _FiltersCollection.Add(filter);
                }
                filter.Add(value);
            }
        }

        private void RemoveValues([ItemCanBeNull] IEnumerable values)
        {
            if (_Selector is not { } selector) return;
            foreach (var value in values)
            {
                var key = selector(value);
                if (!_Filters.TryGetValue(key, out var filter)) continue;
                filter.Remove(value);
                if (filter.Count != 0) continue;
                filter.EnabledChanged -= OnFilterEnableChanged;
                _FiltersCollection.Remove(filter);
                _Filters.Remove(key);
            }
        }

        private void OnFilterEnableChanged(object? sender, EventArgs? e)
        {
            _AllFiltersDisabled = !_FiltersCollection.Any(f => f.Enabled);
            _View?.Refresh();
        }

        public void Filter(object? Sender, FilterEventArgs E)
        {
            if (_Selector is not { } selector) return;
            if (!_Enabled || _AllFiltersDisabled) return;
            var value = E.Item;
            var key = selector(value);
            if (!_Filters.TryGetValue(key, out var filter)) return;
            if (!filter.Enabled) E.Accepted = false;
        }
    }

    public class CollectionViewFilter<TCriteria, TItem> : ReadOnlyObservableCollection<CollectionViewFilterItem<TCriteria>>
    {
        private readonly ICollectionView? _View;
        private readonly Func<TItem, TCriteria>? _Selector;
        private readonly ObservableCollection<CollectionViewFilterItem<TCriteria>> _FiltersCollection;
        private readonly Dictionary<TCriteria, CollectionViewFilterItem<TCriteria>> _Filters = new();
        private bool _Enabled;
        private bool _AllFiltersDisabled = true;
        private string? _Name;

        public bool Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled == value) return;
                _Enabled = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Enabled)));
                _View?.Refresh();
            }
        }

        public string? Name
        {
            get => _Name;
            set
            {
                if (string.Equals(_Name, value)) return;
                _Name = value;
                NameChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler? NameChanged;

        private CollectionViewFilter(ObservableCollection<CollectionViewFilterItem<TCriteria>> filters) : base(filters) => _FiltersCollection = filters;

        public CollectionViewFilter(ICollectionView view, Func<TItem, TCriteria> selector, string? Name = null) : this(new ObservableCollection<CollectionViewFilterItem<TCriteria>>())
        {
            _Name = Name;
            _View = view;
            _Selector = selector;
            ((INotifyCollectionChanged)view.SourceCollection).CollectionChanged += OnCollectionChanged;
            view.CollectionChanged += OnViewCollectionChanged;
        }

        private void OnViewCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs? E)
        {
            if (_View is not { } view || _Selector is not { } selector) return;
            var keys = view.OfType<TItem>().GroupBy(selector).Select(g => g.Key).ToList();
            foreach (var filter in _FiltersCollection)
                if (filter.Key is { } key)
                    filter.ExistInView = keys.Contains(key);
        }

        private void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
        {
            switch (E.Action)
            {
                default: throw new ArgumentOutOfRangeException();
                case NotifyCollectionChangedAction.Add:
                    if (E.NewItems is IEnumerable<TCriteria> added) AddValues(added);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (E.OldItems is IEnumerable<TCriteria> removed) RemoveValues(removed);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (E.OldItems is IEnumerable<TCriteria> old) RemoveValues(old);
                    if (E.NewItems is IEnumerable<TCriteria> @new) AddValues(@new);
                    break;
                case NotifyCollectionChangedAction.Move: break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var filter in _FiltersCollection) filter.EnabledChanged -= OnFilterEnableChanged;
                    _FiltersCollection.Clear();
                    _Filters.Clear();

                    if (_View is not { IsEmpty: false } || _Selector is not { } selector) break;
                    foreach (var group in _View.SourceCollection.OfType<TItem>().GroupBy(selector))
                    {
                        var filter = new CollectionViewFilterItem<TCriteria>(group.Key, group);
                        filter.EnabledChanged += OnFilterEnableChanged;
                        _FiltersCollection.Add(filter);
                        _Filters.Add(group.Key, filter);
                    }

                    break;
            }
        }

        private void AddValues(IEnumerable values)
        {
            if(_Selector is not { } selector) return;
            foreach (TItem value in values)
            {
                var key = selector(value);
                if (!_Filters.TryGetValue(key, out var filter))
                {
                    filter = new CollectionViewFilterItem<TCriteria>(key);
                    filter.EnabledChanged += OnFilterEnableChanged;
                    _Filters.Add(key, filter);
                    _FiltersCollection.Add(filter);
                }
                filter.Add(value);
            }
        }

        private void RemoveValues([ItemCanBeNull] IEnumerable values)
        {
            if(_Selector is not { } selector) return;
            foreach (TItem value in values)
            {
                var key = selector(value);
                if (!_Filters.TryGetValue(key, out var filter)) continue;
                filter.Remove(value);
                if (filter.Count != 0) continue;
                filter.EnabledChanged -= OnFilterEnableChanged;
                _FiltersCollection.Remove(filter);
                _Filters.Remove(key);
            }
        }

        private void OnFilterEnableChanged(object? sender, EventArgs? e)
        {
            _AllFiltersDisabled = !_FiltersCollection.Any(f => f.Enabled);
            _View?.Refresh();
        }

        public void Filter(object? Sender, FilterEventArgs E)
        {
            if (E.Item is not TItem item || !_Enabled || _AllFiltersDisabled || _Selector is not { } selector) return;
            var key = selector(item);
            if (!_Filters.TryGetValue(key, out var filter)) return;
            if (!filter.Enabled) E.Accepted = false;
        }
    }

    public static class CollectionFiltersExtensions
    {
        public static CollectionFilter<TValue, TCriteria> Filter<TValue, TCriteria>
        (
            this ObservableCollection<TValue?> collection,
            Func<TValue, TCriteria> selector
        ) => new(collection, selector);

        public static CollectionViewFilter<TCriteria> FilterView<TCriteria>
        (
            this ICollectionView view,
            Func<object, TCriteria> selector,
            string? Name = null
        ) => new(view, selector, Name);

        public static CollectionViewFilter<TCriteria, TItem> FilterView<TItem, TCriteria>
        (
            this ICollectionView view,
            Func<TItem, TCriteria> selector,
            string? Name = null
        ) => new(view, selector, Name);

        public static CollectionViewFilter<TCriteria> FilterView<TCriteria>
        (
            this CollectionViewSource source,
            Func<object, TCriteria> selector,
            string? Name = null)
        {
            var filter = source.View.FilterView(selector, Name);
            source.Filter += filter.Filter;
            return filter;
        }

        public static CollectionViewFilter<TCriteria, TItem> FilterView<TItem, TCriteria>
        (
            this CollectionViewSource source,
            Func<TItem, TCriteria> selector,
            string? Name = null)
        {
            var filter = source.View.FilterView(selector, Name);
            source.Filter += filter.Filter;
            return filter;
        }
    }
}
