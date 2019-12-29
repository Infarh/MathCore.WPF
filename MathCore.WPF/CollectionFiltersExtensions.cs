using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using MathCore.Annotations;

namespace MathCore.WPF
{
    public class CollectionFilterItem<TValue, TCriteria> : ReadOnlyObservableCollection<TValue>
    {
        [NotNull, ItemCanBeNull] private readonly ObservableCollection<TValue> _InternalCollection;
        [CanBeNull] public TCriteria Key { get; }

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

        public CollectionFilterItem([NotNull, ItemCanBeNull] ObservableCollection<TValue> list) : base(list) => _InternalCollection = list;
        public CollectionFilterItem(TCriteria key) : this(new ObservableCollection<TValue>()) => Key = key;

        public CollectionFilterItem([NotNull] TCriteria key, [NotNull, ItemCanBeNull] IEnumerable<TValue> items) : this(new ObservableCollection<TValue>(items)) => Key = key;

        internal void Add([CanBeNull] TValue value) { if (!_InternalCollection.Contains(value)) _InternalCollection.Add(value); }

        internal bool Remove([CanBeNull] TValue value) => _InternalCollection.Remove(value);
    }

    public class CollectionFilter<TValue, TCriteria> : ReadOnlyObservableCollection<CollectionFilterItem<TValue, TCriteria>>
    {
        [NotNull, ItemNotNull] private readonly ObservableCollection<CollectionFilterItem<TValue, TCriteria>> _InternalCollection;
        [NotNull, ItemCanBeNull] private readonly ObservableCollection<TValue> _Collection;
        [NotNull] private readonly Func<TValue, TCriteria> _Selector;

        private CollectionFilter([NotNull, ItemNotNull] ObservableCollection<CollectionFilterItem<TValue, TCriteria>> internal_collection) : base(internal_collection)
        {
            _InternalCollection = internal_collection;
        }
        public CollectionFilter([NotNull, ItemCanBeNull] ObservableCollection<TValue> collection, [NotNull] Func<TValue, TCriteria> selector)
            : this(new ObservableCollection<CollectionFilterItem<TValue, TCriteria>>())
        {
            _Collection = collection;
            _Selector = selector;
            collection.CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged([CanBeNull] object Sender, [NotNull] NotifyCollectionChangedEventArgs E)
        {
            switch (E.Action)
            {
                default: throw new ArgumentOutOfRangeException();
                case NotifyCollectionChangedAction.Add:
                    AddValues(E.NewItems.Cast<TValue>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveValues(E.OldItems.Cast<TValue>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemoveValues(E.OldItems.Cast<TValue>());
                    AddValues(E.NewItems.Cast<TValue>());
                    break;
                case NotifyCollectionChangedAction.Move: break;
                case NotifyCollectionChangedAction.Reset:
                    _InternalCollection.Clear();
                    if (_Collection.Count == 0) break;
                    foreach (var group in _Collection.GroupBy(_Selector))
                        _InternalCollection.Add(new CollectionFilterItem<TValue, TCriteria>(group.Key, group));
                    break;
            }
        }

        private void AddValues([NotNull, ItemCanBeNull] IEnumerable<TValue> values)
        {
            foreach (var value in values)
            {
                var key = _Selector(value);
                var filter = _InternalCollection.FirstOrDefault(f => Equals(f.Key, key));
                if (filter is null) _InternalCollection.Add(filter = new CollectionFilterItem<TValue, TCriteria>(key));
                filter.Add(value);
            }
        }

        private void RemoveValues([NotNull, ItemCanBeNull] IEnumerable<TValue> values)
        {
            foreach (var value in values)
            {
                var key = _Selector(value);
                var filter = _InternalCollection.FirstOrDefault(f => Equals(f.Key, key));
                filter?.Remove(value);
                if (filter?.Count == 0) _InternalCollection.Remove(filter);
            }
        }
    }

    public class CollectionViewFilterItem<TCriteria> : ReadOnlyObservableCollection<object>
    {
        [NotNull, ItemCanBeNull] private readonly ObservableCollection<object> _InternalCollection;
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

        public bool _ExistInView;

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

        public event EventHandler EnabledChanged;

        [CanBeNull] public TCriteria Key { get; }

        private CollectionViewFilterItem([NotNull, ItemCanBeNull] ObservableCollection<object> collection) : base(collection) => _InternalCollection = collection;
        public CollectionViewFilterItem([CanBeNull] TCriteria key) : this(new ObservableCollection<object>()) => Key = key;

        public CollectionViewFilterItem([CanBeNull] TCriteria key, [NotNull] IEnumerable items) : this(new ObservableCollection<object>(items.Cast<object>())) => Key = key;

        internal void Add([CanBeNull] object value) { if (!_InternalCollection.Contains(value)) _InternalCollection.Add(value); }

        internal bool Remove([CanBeNull] object value) => _InternalCollection.Remove(value);
    }

    public class CollectionViewFilter<TCriteria> : ReadOnlyObservableCollection<CollectionViewFilterItem<TCriteria>>
    {
        [NotNull, ItemCanBeNull] private readonly ICollectionView _View;
        [NotNull] private readonly Func<object, TCriteria> _Selector;
        [NotNull, ItemNotNull] private readonly ObservableCollection<CollectionViewFilterItem<TCriteria>> _FiltersCollection;
        [NotNull] private readonly Dictionary<TCriteria, CollectionViewFilterItem<TCriteria>> _Filters = new Dictionary<TCriteria, CollectionViewFilterItem<TCriteria>>();
        private bool _Enabled;
        private bool _AllFiltersDisabled = true;
        [CanBeNull] private string _Name;

        public bool Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled == value) return;
                _Enabled = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Enabled)));
                _View.Refresh();
            }
        }

        [CanBeNull]
        public string Name
        {
            get => _Name;
            set
            {
                if (string.Equals(_Name, value)) return;
                _Name = value;
                NameChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler NameChanged;

        private CollectionViewFilter([NotNull, ItemNotNull] ObservableCollection<CollectionViewFilterItem<TCriteria>> filters) : base(filters) => _FiltersCollection = filters;

        public CollectionViewFilter([NotNull, ItemCanBeNull] ICollectionView view, [NotNull] Func<object, TCriteria> selector, [CanBeNull] string Name = null) : this(new ObservableCollection<CollectionViewFilterItem<TCriteria>>())
        {
            _Name = Name;
            _View = view;
            _Selector = selector;
            ((INotifyCollectionChanged)view.SourceCollection).CollectionChanged += OnCollectionChanged;
            view.CollectionChanged += OnViewCollectionChanged;
        }

        private void OnViewCollectionChanged([CanBeNull] object Sender, [CanBeNull] NotifyCollectionChangedEventArgs E)
        {
            var keys = _View.Cast<object>().GroupBy(_Selector).Select(g => g.Key).ToList();
            foreach (var filter in _FiltersCollection)
                filter.ExistInView = keys.Contains(filter.Key);
        }

        private void OnCollectionChanged([CanBeNull] object Sender, [NotNull] NotifyCollectionChangedEventArgs E)
        {
            switch (E.Action)
            {
                default: throw new ArgumentOutOfRangeException();
                case NotifyCollectionChangedAction.Add:
                    AddValues(E.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveValues(E.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemoveValues(E.OldItems);
                    AddValues(E.NewItems);
                    break;
                case NotifyCollectionChangedAction.Move: break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var filter in _FiltersCollection) filter.EnabledChanged -= OnFilterEnableChanged;
                    _FiltersCollection.Clear();
                    _Filters.Clear();

                    if (_View.IsEmpty) break;
                    foreach (var group in _View.SourceCollection.Cast<object>().GroupBy(_Selector))
                    {
                        var filter = new CollectionViewFilterItem<TCriteria>(group.Key, group);
                        filter.EnabledChanged += OnFilterEnableChanged;
                        _FiltersCollection.Add(filter);
                        _Filters.Add(group.Key, filter);
                    }

                    break;
            }
        }

        private void AddValues([NotNull, ItemCanBeNull] IEnumerable values)
        {
            foreach (var value in values)
            {
                var key = _Selector(value);
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

        private void RemoveValues([NotNull, ItemCanBeNull] IEnumerable values)
        {
            foreach (var value in values)
            {
                var key = _Selector(value);
                if (!_Filters.TryGetValue(key, out var filter)) continue;
                filter.Remove(value);
                if (filter.Count != 0) continue;
                filter.EnabledChanged -= OnFilterEnableChanged;
                _FiltersCollection.Remove(filter);
                _Filters.Remove(key);
            }
        }

        private void OnFilterEnableChanged([CanBeNull] object sender, [CanBeNull] EventArgs e)
        {
            _AllFiltersDisabled = !_FiltersCollection.Any(f => f.Enabled);
            _View.Refresh();
        }

        public void Filter([CanBeNull] object Sender, [NotNull] FilterEventArgs E)
        {
            if (!_Enabled || _AllFiltersDisabled) return;
            var value = E.Item;
            var key = _Selector(value);
            if (!_Filters.TryGetValue(key, out var filter)) return;
            if (!filter.Enabled) E.Accepted = false;
        }
    }

    public class CollectionViewFilter<TCriteria, TItem> : ReadOnlyObservableCollection<CollectionViewFilterItem<TCriteria>>
    {
        [NotNull, ItemCanBeNull] private readonly ICollectionView _View;
        [NotNull] private readonly Func<TItem, TCriteria> _Selector;
        [NotNull, ItemNotNull] private readonly ObservableCollection<CollectionViewFilterItem<TCriteria>> _FiltersCollection;
        [NotNull] private readonly Dictionary<TCriteria, CollectionViewFilterItem<TCriteria>> _Filters = new Dictionary<TCriteria, CollectionViewFilterItem<TCriteria>>();
        private bool _Enabled;
        private bool _AllFiltersDisabled = true;
        [CanBeNull] private string _Name;

        public bool Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled == value) return;
                _Enabled = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Enabled)));
                _View.Refresh();
            }
        }

        [CanBeNull]
        public string Name
        {
            get => _Name;
            set
            {
                if (string.Equals(_Name, value)) return;
                _Name = value;
                NameChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler NameChanged;

        private CollectionViewFilter([NotNull, ItemNotNull] ObservableCollection<CollectionViewFilterItem<TCriteria>> filters) : base(filters) => _FiltersCollection = filters;

        public CollectionViewFilter([NotNull, ItemCanBeNull] ICollectionView view, [NotNull] Func<TItem, TCriteria> selector, [CanBeNull] string Name = null) : this(new ObservableCollection<CollectionViewFilterItem<TCriteria>>())
        {
            _Name = Name;
            _View = view;
            _Selector = selector;
            ((INotifyCollectionChanged)view.SourceCollection).CollectionChanged += OnCollectionChanged;
            view.CollectionChanged += OnViewCollectionChanged;
        }

        private void OnViewCollectionChanged([CanBeNull] object Sender, [CanBeNull] NotifyCollectionChangedEventArgs E)
        {
            var keys = _View.OfType<TItem>().GroupBy(_Selector).Select(g => g.Key).ToList();
            foreach (var filter in _FiltersCollection)
                filter.ExistInView = keys.Contains(filter.Key);
        }

        private void OnCollectionChanged([CanBeNull] object Sender, [NotNull] NotifyCollectionChangedEventArgs E)
        {
            switch (E.Action)
            {
                default: throw new ArgumentOutOfRangeException();
                case NotifyCollectionChangedAction.Add:
                    AddValues(E.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveValues(E.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemoveValues(E.OldItems);
                    AddValues(E.NewItems);
                    break;
                case NotifyCollectionChangedAction.Move: break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var filter in _FiltersCollection) filter.EnabledChanged -= OnFilterEnableChanged;
                    _FiltersCollection.Clear();
                    _Filters.Clear();

                    if (_View.IsEmpty) break;
                    foreach (var group in _View.SourceCollection.OfType<TItem>().GroupBy(_Selector))
                    {
                        var filter = new CollectionViewFilterItem<TCriteria>(group.Key, group);
                        filter.EnabledChanged += OnFilterEnableChanged;
                        _FiltersCollection.Add(filter);
                        _Filters.Add(group.Key, filter);
                    }

                    break;
            }
        }

        private void AddValues([NotNull, ItemCanBeNull] IEnumerable values)
        {
            foreach (TItem value in values)
            {
                var key = _Selector(value);
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

        private void RemoveValues([NotNull, ItemCanBeNull] IEnumerable values)
        {
            foreach (TItem value in values)
            {
                var key = _Selector(value);
                if (!_Filters.TryGetValue(key, out var filter)) continue;
                filter.Remove(value);
                if (filter.Count != 0) continue;
                filter.EnabledChanged -= OnFilterEnableChanged;
                _FiltersCollection.Remove(filter);
                _Filters.Remove(key);
            }
        }

        private void OnFilterEnableChanged([CanBeNull] object sender, [CanBeNull] EventArgs e)
        {
            _AllFiltersDisabled = !_FiltersCollection.Any(f => f.Enabled);
            _View.Refresh();
        }

        public void Filter([CanBeNull] object Sender, [NotNull] FilterEventArgs E)
        {
            if (!(E.Item is TItem item) || !_Enabled || _AllFiltersDisabled) return;
            var key = _Selector(item);
            if (!_Filters.TryGetValue(key, out var filter)) return;
            if (!filter.Enabled) E.Accepted = false;
        }
    }

    public static class CollectionFiltersExtensions
    {
        [NotNull, ItemNotNull]
        public static CollectionFilter<TValue, TCriteria> Filter<TValue, TCriteria>
        (
            [NotNull, ItemCanBeNull] this ObservableCollection<TValue> collection,
            [NotNull] Func<TValue, TCriteria> selector
        ) => new CollectionFilter<TValue, TCriteria>(collection, selector);

        [NotNull, ItemNotNull]
        public static CollectionViewFilter<TCriteria> FilterView<TCriteria>
        (
            [NotNull, ItemCanBeNull] this ICollectionView view,
            [NotNull] Func<object, TCriteria> selector,
            [CanBeNull] string Name = null
        ) => new CollectionViewFilter<TCriteria>(view, selector, Name);

        [NotNull, ItemNotNull]
        public static CollectionViewFilter<TCriteria, TItem> FilterView<TItem, TCriteria>
        (
            [NotNull, ItemCanBeNull] this ICollectionView view,
            [NotNull] Func<TItem, TCriteria> selector,
            [CanBeNull] string Name = null
        ) => new CollectionViewFilter<TCriteria, TItem>(view, selector, Name);

        [NotNull, ItemNotNull]
        public static CollectionViewFilter<TCriteria> FilterView<TCriteria>
        (
            [NotNull] this CollectionViewSource source,
            [NotNull] Func<object, TCriteria> selector,
            [CanBeNull] string Name = null)
        {
            var filter = source.View.FilterView(selector, Name);
            source.Filter += filter.Filter;
            return filter;
        }

        [NotNull, ItemNotNull]
        public static CollectionViewFilter<TCriteria, TItem> FilterView<TItem, TCriteria>
        (
            [NotNull] this CollectionViewSource source,
            [NotNull] Func<TItem, TCriteria> selector,
            [CanBeNull] string Name = null)
        {
            var filter = source.View.FilterView(selector, Name);
            source.Filter += filter.Filter;
            return filter;
        }
    }
}
