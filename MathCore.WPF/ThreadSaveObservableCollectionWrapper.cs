using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MathCore.WPF;

/// <summary>Потокобезопасная обертка над ObservableCollection для безопасной подписки на события коллекции</summary>
public class ThreadSaveObservableCollectionWrapper<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    /// <summary>Событие изменения коллекции</summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>Событие изменения свойств</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    private ObservableCollection<T> _BaseCollection;

    /// <summary>Базовая коллекция, обернутая данным классом</summary>
    public ObservableCollection<T> BaseCollection => _BaseCollection;

    /// <summary>Инициализирует новый экземпляр обертки над указанной коллекцией</summary>
    /// <param name="collection">Коллекция, для которой создается обертка</param>
    public ThreadSaveObservableCollectionWrapper(ObservableCollection<T> collection)
    {
        _BaseCollection               =  collection;
        collection.CollectionChanged += OnBaseCollectionChanged;
        ((INotifyPropertyChanged)collection).PropertyChanged += OnBaseCollectionPropertyChanged;
    }

    /// <summary>Обработчик события изменения базовой коллекции</summary>
    /// <param name="Sender">Источник события</param>
    /// <param name="E">Аргументы события изменения коллекции</param>
    protected virtual void OnBaseCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E) => 
        CollectionChanged?.ThreadSafeInvoke(this, E);

    /// <summary>Обработчик события изменения свойств базовой коллекции</summary>
    /// <param name="Sender">Источник события</param>
    /// <param name="E">Аргументы события изменения свойства</param>
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

    /// <summary>Полностью переинициализирует содержимое коллекции новыми элементами без смены базовой коллекции</summary>
    /// <param name="items">Новый набор элементов для коллекции</param>
    public void Reset(IEnumerable<T> items)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));

        _BaseCollection.CollectionChanged                         -= OnBaseCollectionChanged;
        ((INotifyPropertyChanged)_BaseCollection).PropertyChanged -= OnBaseCollectionPropertyChanged;

        _BaseCollection.Clear();
        foreach (var item in items)
            _BaseCollection.Add(item);

        _BaseCollection.CollectionChanged                         += OnBaseCollectionChanged;
        ((INotifyPropertyChanged)_BaseCollection).PropertyChanged += OnBaseCollectionPropertyChanged;

        OnBaseCollectionChanged(this, new(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>Неявное преобразование ObservableCollection в потокобезопасную обертку</summary>
    /// <param name="collection">Исходная коллекция</param>
    /// <returns>Созданная обертка над коллекцией</returns>
    public static implicit operator ThreadSaveObservableCollectionWrapper<T>(ObservableCollection<T> collection) => new(collection);

    /// <summary>Неявное преобразование обертки к базовой ObservableCollection</summary>
    /// <param name="collection">Обертка над коллекцией</param>
    /// <returns>Базовая коллекция</returns>
    public static implicit operator ObservableCollection<T>(ThreadSaveObservableCollectionWrapper<T> collection) => collection._BaseCollection;
}

/// <summary>Набор методов расширения для создания потокобезопасных оберток над ObservableCollection</summary>
public static class ThreadSaveObservableCollectionExtensions
{
    /// <summary>Создает потокобезопасную обертку над указанной коллекцией</summary>
    /// <param name="collection">Коллекция, для которой создается обертка</param>
    /// <typeparam name="T">Тип элементов коллекции</typeparam>
    /// <returns>Потокобезопасная обертка над переданной коллекцией</returns>
    public static ThreadSaveObservableCollectionWrapper<T?> AsThreadSave<T>(this ObservableCollection<T?> collection)
        => new(collection);
}