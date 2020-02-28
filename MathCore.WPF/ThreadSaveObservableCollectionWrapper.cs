using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using MathCore.Annotations;

namespace MathCore.WPF
{
    public class ThreadSaveObservableCollectionWrapper<T> : IList<T>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        [NotNull] private readonly ObservableCollection<T> _Collection;

        public ThreadSaveObservableCollectionWrapper([NotNull] ObservableCollection<T> collection)
        {
            _Collection = collection;
            collection.CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object Sender, NotifyCollectionChangedEventArgs E) => CollectionChanged?.ThreadSafeInvoke(_Collection, E);


        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => _Collection.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Collection).GetEnumerator();

        /// <inheritdoc />
        public void Add(T item) => _Collection.Add(item);

        /// <inheritdoc />
        public void Clear() => _Collection.Clear();

        /// <inheritdoc />
        public bool Contains(T item) => _Collection.Contains(item);

        /// <inheritdoc />
        public void CopyTo(T[] array, int ArrayIndex) => _Collection.CopyTo(array, ArrayIndex);

        /// <inheritdoc />
        public bool Remove(T item) => _Collection.Remove(item);

        /// <inheritdoc />
        public int Count => _Collection.Count;

        /// <inheritdoc />
        public bool IsReadOnly => ((ICollection<T>)_Collection).IsReadOnly;

        /// <inheritdoc />
        public int IndexOf(T item) => _Collection.IndexOf(item);

        /// <inheritdoc />
        public void Insert(int index, T item) => _Collection.Insert(index, item);

        /// <inheritdoc />
        public void RemoveAt(int index) => _Collection.RemoveAt(index);

        /// <inheritdoc />
        public T this[int index] { get => _Collection[index]; set => _Collection[index] = value; }
    }

    public static class ThreadSaveObservableCollectionExtentions
    {
        [NotNull, ItemCanBeNull]
        public static ThreadSaveObservableCollectionWrapper<T> AsThreadSave<T>([NotNull, ItemCanBeNull] this ObservableCollection<T> collection)
            => new ThreadSaveObservableCollectionWrapper<T>(collection);
    }
}