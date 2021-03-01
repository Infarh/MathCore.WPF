using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MathCore.Annotations;
using MathCore.Extensions.Expressions;
using MathCore.WPF;
// ReSharper disable UnusedType.Global
// ReSharper disable ReturnTypeCanBeEnumerable.Local

// ReSharper disable once CheckNamespace
namespace System.Collections.ObjectModel
{
    public static class CollectionExtensions
    {
        [NotNull]
        public static ObservableCollectionSyncWrapper<T> GetThreadSafeWrapper<T>(
            this ObservableCollection<T> collection) =>
                new(collection);

        private abstract class CollectionConnectorBase
        {
            private static readonly List<CollectionConnectorBase> __ConnectorsPool
                = new();

            [NotNull]
            public static ReadOnlyCollection<CollectionConnectorBase> Pool =>
                new(__ConnectorsPool);

            protected CollectionConnectorBase() => __ConnectorsPool.Add(this);

            public virtual void Close() => __ConnectorsPool.Remove(this);
        }

        private sealed class CollectionConnector<TSourceItem, TDestItem, TSource, TDest> : CollectionConnectorBase
            where TSource : class, ICollection<TSourceItem>, INotifyCollectionChanged
            where TDest : class, ICollection<TDestItem>, INotifyCollectionChanged
        {
            private readonly WeakReference _SourceRef;
            private readonly WeakReference _DestinationRef;
            private readonly Func<TSourceItem, TDestItem> _Converter;
            private readonly Dictionary<object, TDestItem> _Items = new();

            public TSource Source => (TSource)_SourceRef.Target!;

            public TDest Destination => (TDest)_DestinationRef.Target!;

            public CollectionConnector(
                [NotNull] TSource Source,
                [NotNull] TDest Destination,
                [CanBeNull] Func<TSourceItem, TDestItem>? Converter = null)
            {
                _SourceRef = new WeakReference(Source);
                _DestinationRef = new WeakReference(Destination);
                _Converter = Converter ?? (s => (TDestItem)(object)s!);
                AddItems();
                Source.CollectionChanged += OnCollectionChanged;
            }

            private void OnCollectionChanged(object Sender, [NotNull]NotifyCollectionChangedEventArgs E)
            {
                switch (E.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        AddItems(E.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        RemoveItems(E.OldItems);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        RemoveItems(E.OldItems);
                        AddItems(E.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Reset();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private void Reset()
            {
                Clear();
                AddItems();
            }

            private void AddItems()
            {
                var source = Source;
                if (source != null)
                    AddItems(source.ToList());
            }

            private void AddItems([CanBeNull] ICollection? NewItems)
            {
                if (NewItems is null || NewItems.Count == 0) return;
                var dest = Destination;
                if (dest is null)
                {
                    Close();
                    return;
                }

                foreach (var obj in NewItems)
                {
                    if(obj is not TSourceItem item) continue;
                    var value = _Converter(item);
                    dest.Add(value);
                    _Items.Add(item, value);
                }
            }

            private void RemoveItems([CanBeNull] ICollection? NewItems)
            {
                if (NewItems is null || NewItems.Count == 0) return;
                var dest = Destination;
                if (dest is null)
                {
                    Close();
                    return;
                }

                foreach (var obj in NewItems)
                {
                    if(obj is not TSourceItem item || !_Items.TryGetValue(item, out var value)) continue;
                    dest.Remove(value);
                    _Items.Remove(item);
                }
            }

            private void Clear() => RemoveItems(_Items.Keys.ToArray());

            public override void Close()
            {
                var source = Source;
                if (source != null)
                    source.CollectionChanged -= OnCollectionChanged;
                base.Close();
            }
        }

        public static void SetItemSource<TDestItem, TSourceItem, TSource, TDest>(
            [NotNull] TDest DestinationCollection,
            [NotNull] TSource SourceCollection,
            Func<TSourceItem, TDestItem>? Converter = null)
            where TDest : class, ICollection<TDestItem>, INotifyCollectionChanged
            where TSource : class, ICollection<TSourceItem>, INotifyCollectionChanged
        {
            if (CollectionConnectorBase.Pool
                .OfType<CollectionConnector<TSourceItem, TDestItem, TSource, TDest>>()
                .Any(c => ReferenceEquals(c.Source, SourceCollection) && ReferenceEquals(c.Destination, DestinationCollection)))
                return;
            _ = new CollectionConnector<TSourceItem, TDestItem, TSource, TDest>(SourceCollection, DestinationCollection, Converter);
        }

        public static void ConnectTo<TDestItem, TSourceItem, TSource, TDest>(
            [NotNull] TSource SourceCollection,
            [NotNull] TDest DestinationCollection,
            Func<TSourceItem, TDestItem>? Converter = null)
            where TDest : class, ICollection<TDestItem>, INotifyCollectionChanged
            where TSource : class, ICollection<TSourceItem>, INotifyCollectionChanged
        {
            if (CollectionConnectorBase.Pool
                .OfType<CollectionConnector<TSourceItem, TDestItem, TSource, TDest>>()
                .Any(c => ReferenceEquals(c.Source, SourceCollection) && ReferenceEquals(c.Destination, DestinationCollection)))
                return;
            _ = new CollectionConnector<TSourceItem, TDestItem, TSource, TDest>(SourceCollection, DestinationCollection, Converter);
        }

        public static void DisconnectSource<TDestItem, TSourceItem, TSource, TDest>(
            [NotNull] this TDest DestinationCollection,
            [NotNull] TSource SourceCollection)
                where TDest : class, ICollection<TDestItem>, INotifyCollectionChanged
                where TSource : class, ICollection<TSourceItem>, INotifyCollectionChanged
            => CollectionConnectorBase.Pool.OfType<CollectionConnector<TSourceItem, TDestItem, TSource, TDest>>()
                    .FirstOrDefault(
                        c =>
                            ReferenceEquals(c.Source, SourceCollection) &&
                            ReferenceEquals(c.Destination, DestinationCollection))?.Close();

        public static void ResetBinding<TDestItem, TSourceItem, TSource, TDest>(
            [NotNull] this TSource SourceCollection,
            [NotNull] TDest DestinationCollection)
                where TDest : class, ICollection<TDestItem>, INotifyCollectionChanged
                where TSource : class, ICollection<TSourceItem>, INotifyCollectionChanged
            => CollectionConnectorBase.Pool.OfType<CollectionConnector<TSourceItem, TDestItem, TSource, TDest>>()
                    .FirstOrDefault(
                        c =>
                            ReferenceEquals(c.Source, SourceCollection) &&
                            ReferenceEquals(c.Destination, DestinationCollection))?.Close();

        private static readonly Dictionary<Type, (Delegate GetItems, Action<object, PropertyChangedEventArgs> OnPropertyChanged, Action<object, NotifyCollectionChangedEventArgs> OnCollectionChanged)> __ItemsDictionary = 
            new();

        public static void AddItemsRange<TCollection, TItem>(
            [NotNull]this TCollection collection,
            [NotNull] IEnumerable<TItem> items)
            where TCollection : ICollection<TItem>
        {
            if (!(collection is ObservableCollection<TItem>))
                foreach (var item in items)
                    collection.Add(item);

            else
            {
                (Delegate GetItems, Action<object, PropertyChangedEventArgs> OnPropertyChanged, Action<object, NotifyCollectionChangedEventArgs> OnCollectionChanged) p;
                lock (__ItemsDictionary)
                    p = __ItemsDictionary.GetValueOrAddNew(
                        typeof(TCollection), t =>
                        {
                            var mi_pc = t.GetMethod("OnPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic,
                                null, new[] { typeof(PropertyChangedEventArgs) }, new[] { new ParameterModifier(1) })
                                ?? throw new InvalidOperationException("Метод OnPropertyChanged не найден");
                            var mi_cc = t.GetMethod("OnCollectionChanged",
                                BindingFlags.Instance | BindingFlags.NonPublic, null,
                                new[] { typeof(NotifyCollectionChangedEventArgs) }, new[] { new ParameterModifier(1) })
                                ?? throw new InvalidOperationException("Метод OnCollectionChanged не найден");
                            var p_obj = "obj".ParameterOf(typeof(object));
                            var p_pc = "arg".ParameterOf(typeof(PropertyChangedEventArgs));
                            var p_cc = "arg".ParameterOf(typeof(NotifyCollectionChangedEventArgs));
                            return (
                                Property<IList<TItem>>.GetExtractor<TCollection>("Items", false),
                                p_obj.ConvertTo(typeof(TCollection))
                                    .GetCall(mi_pc, p_pc)
                                    .CreateLambda<Action<object, PropertyChangedEventArgs>>(p_obj, p_pc)
                                    .Compile(),
                                p_obj.ConvertTo(typeof(TCollection))
                                    .GetCall(mi_cc, p_cc)
                                    .CreateLambda<Action<object, NotifyCollectionChangedEventArgs>>(p_obj, p_cc)
                                    .Compile()
                                );
                        });

                var items_collection = ((Func<TCollection, IList<TItem>>)p.GetItems)(collection);
                object? I = null;
                var count = 0;
                items_collection.AddItemsRange(items.ForeachLazy(i => { if (count++ == 0) I = i; else I = null; }));
                if (count == 0) return;
                p.OnPropertyChanged(collection, new PropertyChangedEventArgs("Count"));
                p.OnPropertyChanged(collection, new PropertyChangedEventArgs("Item[]"));
                var e = count == 1
                    ? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, I)
                    : new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                p.OnCollectionChanged(collection, e);
            }
        }

        public static void RemoveItemsRange<TCollection, TItem>(
            [NotNull]this TCollection collection,
            [NotNull] IEnumerable<TItem> items)
            where TCollection : ICollection<TItem>
        {
            if (!(collection is ObservableCollection<TItem>))
                foreach (var item in items)
                    collection.Remove(item);
            else
            {
                (Delegate GetItems, Action<object, PropertyChangedEventArgs> OnPropertyChanged, Action<object, NotifyCollectionChangedEventArgs> OnCollectionChanged) p;
                lock (__ItemsDictionary)
                    p = __ItemsDictionary.GetValueOrAddNew(
                        typeof(TCollection), t =>
                        {
                            var mi_pc = t.GetMethod("OnPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(PropertyChangedEventArgs) }, new[] { new ParameterModifier(1) })
                                ?? throw new InvalidOperationException("Метод OnPropertyChanged не найден");
                            var mi_cc = t.GetMethod("OnCollectionChanged", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(NotifyCollectionChangedEventArgs) }, new[] { new ParameterModifier(1) })
                                ?? throw new InvalidOperationException("Метод OnCollectionChanged не найден");
                            var p_obj = "obj".ParameterOf(typeof(object));
                            var p_pc = "arg".ParameterOf(typeof(PropertyChangedEventArgs));
                            var p_cc = "arg".ParameterOf(typeof(NotifyCollectionChangedEventArgs));
                            return (
                                Property<IList<TItem>>.GetExtractor<TCollection>("Items", false),
                                p_obj.ConvertTo(typeof(TCollection)).GetCall(mi_pc, p_pc).CreateLambda<Action<object, PropertyChangedEventArgs>>(p_obj, p_pc).Compile(),
                                p_obj.ConvertTo(typeof(TCollection)).GetCall(mi_cc, p_cc).CreateLambda<Action<object, NotifyCollectionChangedEventArgs>>(p_obj, p_cc).Compile()
                                );
                        });

                var items_collection = ((Func<TCollection, IList<TItem>>)p.GetItems)(collection);
                object? I = null;
                var count = 0;
                items_collection.RemoveItemsRange(items.ForeachLazy(i => { if (count++ == 0) I = i; else I = null; }));
                if (count == 0) return;
                p.OnPropertyChanged(collection, new PropertyChangedEventArgs("Count"));
                p.OnPropertyChanged(collection, new PropertyChangedEventArgs("Item[]"));
                var e = count == 1
                    ? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, I)
                    : new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                p.OnCollectionChanged(collection, e);
            }
        }

        [NotNull] public static bool[] RemoveItemsRangeResult<T>([NotNull]this ICollection<T> collection, [NotNull] IEnumerable<T> items) => items.Select(collection.Remove).ToArray();
    }
}