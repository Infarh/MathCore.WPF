using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MathCore.WPF;

/// <summary>Наблюдаемая коллекция элементов, построенная на другой наблюдаемой коллекции элементов с указаинем метода преобразвоания элементов</summary>
/// <typeparam name="TSourceItem">Тим элементов исходной коллекции</typeparam>
/// <typeparam name="TCollectionItem">Тип требуемых элементов</typeparam>
public class ReadOnlyObservableLambdaCollection<TSourceItem, TCollectionItem> : IList<TCollectionItem>, IList, INotifyCollectionChanged, INotifyPropertyChanged
{
    #region Статические методы

    /// <summary>Генерация исключения при вызове метода интерфейса, не поддерживаемого данной коллекцией</summary>
    /// <param name="Method">Имя вызываемого метода</param>
    /// <returns>Исключение <see cref="NotSupportedException"/></returns>
    private static NotSupportedException NotSupported([CallerMemberName] string Method = null!) => new($"{Method} не поддерживается коллекцией только для чтения");

    #endregion

    #region INotifyCollectionChanged

    /// <summary>Событие возникает когда наблюдаемая коллекция меняется</summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>Генерация события изменения коллекции</summary>
    /// <param name="args">Аргументы события</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args) => CollectionChanged?.Invoke(this, args);

    /// <summary>Обработчик событий изменения в наблюдаемой коллекции</summary>
    /// <param name="sender">Источник события - наблюдаемая коллекция</param>
    /// <param name="e">Аргумент события, определяющий тип изменения наблюдаемой коллекции</param>
    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        NotifyCollectionChangedEventArgs new_e;
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                new_e = new NotifyCollectionChangedEventArgs(e.Action, e.NewItems.OfType<TSourceItem>().Select(_Converter).ToArray());
                break;
            case NotifyCollectionChangedAction.Reset: OnCollectionChanged(e); return;
            default:
                throw new NotImplementedException();
        }

        OnCollectionChanged(new_e);
    }

    #endregion

    #region INotifyPropertyChanged

    /// <summary>Событие возникает когда изменяется одно из свойств наблюдаемой коллекции</summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>Генерация события изменения свойства коллекции</summary>
    /// <param name="args">Аргумент события изменения свойства, хранящий имя изменившегося свойства</param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

    /// <summary>Обработчик события изменеий свойств наблюдаемой коллекции</summary>
    /// <param name="sender">Источник события - наблюдаемая коллекция</param>
    /// <param name="e">Аргумент события - хранящий имя изменившегося свойства</param>
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) => OnPropertyChanged(e);

    #endregion

    #region Поля

    /// <summary>Наблюдаемая коллекция</summary>
    private readonly ObservableCollection<TSourceItem> _Collection;

    /// <summary>Метод преобразования элементов наблюдаемой коллекции в элементы конечной коллекции</summary>
    private readonly Func<TSourceItem, TCollectionItem> _Converter;

    #endregion

    #region Свойства

    /// <summary>Число элементов коллекции</summary>
    public int Count => _Collection.Count;

    /// <summary>Перечисление преобразованных элементов исходной коллекции указанным методом преобразвоания</summary>
    private IEnumerable<TCollectionItem> OutElementCollection => _Collection.Select(_Converter);

    /// <summary>Элемент коллекции с указанным индексом</summary>
    /// <param name="index">Индекс элемента коллекции</param>
    /// <returns>Элемент коллекции с указанным индексом</returns>
    public TCollectionItem this[int index] => _Converter(_Collection[index]);

    #endregion

    #region Конструкторы

    /// <summary>Инициализация новой наблюдаемой коллекции элементов требуемого типа</summary>
    /// <param name="collection">Исходная наблюдаемая коллекция</param>
    /// <param name="converter">Метод преобразования элементов</param>
    public ReadOnlyObservableLambdaCollection(ObservableCollection<TSourceItem> collection, Func<TSourceItem, TCollectionItem> converter)
    {
        _Collection                                           =  collection;
        _Converter                                            =  converter;
        _Collection.CollectionChanged                         += OnCollectionChanged;
        ((INotifyPropertyChanged)_Collection).PropertyChanged += OnPropertyChanged;
    }

    #endregion

    #region Интерфейсы

    #region ICollection<TOutElement>

    /// <inheritdoc />
    bool ICollection<TCollectionItem>.IsReadOnly => true;

    /// <inheritdoc />
    void ICollection<TCollectionItem>.Add(TCollectionItem item) => throw NotSupported();

    /// <inheritdoc />
    bool ICollection<TCollectionItem>.Remove(TCollectionItem item) => throw NotSupported();

    /// <inheritdoc />
    bool ICollection<TCollectionItem>.Contains(TCollectionItem item) => OutElementCollection.Contains(item);

    /// <inheritdoc />
    void ICollection<TCollectionItem>.CopyTo(TCollectionItem[] array, int index)
    {
        foreach (var element in _Collection)
            array[index++] = _Converter(element);
    }

    /// <inheritdoc />
    void ICollection<TCollectionItem>.Clear() => throw NotSupported();

    #endregion

    #region ICollection

    /// <inheritdoc />
    object ICollection.SyncRoot => ((ICollection)_Collection).SyncRoot;

    /// <inheritdoc />
    bool ICollection.IsSynchronized => ((ICollection)_Collection).IsSynchronized;

    /// <inheritdoc />
    void ICollection.CopyTo(Array array, int index)
    {
        foreach (var item in _Collection)
            array.SetValue(_Converter(item), index++);
    }

    #endregion

    #region IList<TOutElement>

    /// <inheritdoc />
    TCollectionItem IList<TCollectionItem>.this[int index] { get => _Converter(_Collection[index]); set => throw NotSupported(); }

    /// <inheritdoc />
    void IList<TCollectionItem>.Insert(int index, TCollectionItem item) => throw NotSupported();

    /// <inheritdoc />
    int IList<TCollectionItem>.IndexOf(TCollectionItem item) => _Collection.Select(_Converter).FirstIndexOf(item);

    #endregion

    #region IList

    /// <inheritdoc />
    bool IList.IsReadOnly => true;

    /// <inheritdoc />
    bool IList.IsFixedSize => true;

    /// <inheritdoc />
    object? IList.this[int index] { get => this[index]; set => throw NotSupported(); }

    /// <inheritdoc />
    int IList.Add(object? value) => throw NotSupported();

    /// <inheritdoc />
    void IList<TCollectionItem>.RemoveAt(int index) => throw NotSupported();

    /// <inheritdoc />
    bool IList.Contains(object? value) => OutElementCollection.Contains((TCollectionItem)value);

    /// <inheritdoc />
    int IList.IndexOf(object? value) => OutElementCollection.FirstIndexOf((TCollectionItem)value);

    /// <inheritdoc />
    void IList.Insert(int index, object? value) => throw NotSupported();

    /// <inheritdoc />
    void IList.Remove(object? value) => throw NotSupported();

    /// <inheritdoc />
    void IList.RemoveAt(int index) => throw NotSupported();

    /// <inheritdoc />
    void IList.Clear() => throw NotSupported();

    #endregion

    #region IEnumerable

    /// <inheritdoc />
    IEnumerator<TCollectionItem> IEnumerable<TCollectionItem>.GetEnumerator() => OutElementCollection.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => OutElementCollection.GetEnumerator();

    #endregion

    #endregion
}