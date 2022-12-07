using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MathCore.WPF;

public class ThreadSaveObservableCollectionWrapper<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    private ObservableCollection<T> _BaseCollection;

    public ObservableCollection<T> BaseCollection => _BaseCollection;

    public ThreadSaveObservableCollectionWrapper(ObservableCollection<T> collection)
    {
        _BaseCollection                  =  collection;
        collection.CollectionChanged += OnBaseCollectionChanged;
        ((INotifyPropertyChanged)collection).PropertyChanged += OnBaseCollectionPropertyChanged;
    }

    protected virtual void OnBaseCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E) => 
        CollectionChanged?.ThreadSafeInvoke(this, E);

    protected virtual void OnBaseCollectionPropertyChanged(object? Sender, PropertyChangedEventArgs E) => 
        PropertyChanged?.ThreadSafeInvoke(this, E.PropertyName);

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => _BaseCollection.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_BaseCollection).GetEnumerator();

    /// <inheritdoc />
    public void Add(T item) => _BaseCollection.Add(item);

    /// <inheritdoc />
    public void Clear() => _BaseCollection.Clear();

    /// <inheritdoc />
    public bool Contains(T item) => _BaseCollection.Contains(item);

    /// <inheritdoc />
    public void CopyTo(T[] array, int ArrayIndex) => _BaseCollection.CopyTo(array, ArrayIndex);

    /// <inheritdoc />
    public bool Remove(T item) => _BaseCollection.Remove(item);

    /// <inheritdoc />
    public int Count => _BaseCollection.Count;

    /// <inheritdoc />
    public bool IsReadOnly => ((ICollection<T>)_BaseCollection).IsReadOnly;

    /// <inheritdoc />
    public int IndexOf(T item) => _BaseCollection.IndexOf(item);

    /// <inheritdoc />
    public void Insert(int index, T item) => _BaseCollection.Insert(index, item);

    /// <inheritdoc />
    public void RemoveAt(int index) => _BaseCollection.RemoveAt(index);

    /// <inheritdoc />
    public T this[int index] { get => _BaseCollection[index]; set => _BaseCollection[index] = value; }

    public void Reset(IEnumerable<T> items)
    {
        if (_BaseCollection is { } old_collection)
        {
            old_collection.CollectionChanged                         -= OnBaseCollectionChanged;
            ((INotifyPropertyChanged)old_collection).PropertyChanged -= OnBaseCollectionPropertyChanged;
        }

        var collection = new ObservableCollection<T>(items);

        collection.CollectionChanged                         += OnBaseCollectionChanged;
        ((INotifyPropertyChanged)collection).PropertyChanged += OnBaseCollectionPropertyChanged;

        _BaseCollection = collection;
        OnBaseCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public static implicit operator ThreadSaveObservableCollectionWrapper<T>(ObservableCollection<T> collection) => new(collection);

    public static implicit operator ObservableCollection<T>(ThreadSaveObservableCollectionWrapper<T> collection) => collection._BaseCollection;
}

public static class ThreadSaveObservableCollectionExtensions
{
    public static ThreadSaveObservableCollectionWrapper<T?> AsThreadSave<T>(this ObservableCollection<T?> collection)
        => new(collection);
}