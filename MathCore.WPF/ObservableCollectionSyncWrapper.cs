using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Threading;

namespace MathCore.WPF;

/// <summary>Потокобезопасная обёртка для <see cref="T:System.Collection.ObjectModel.ObservableCollection[T]"/></summary>
/// <typeparam name="T">Тип элемента коллекции</typeparam>
public class ObservableCollectionSyncWrapper<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    public event NotifyCollectionChangedEventHandler CollectionChanged;
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs E) => _AsyncOp.Post(_CollectionChanged, E);

    public event PropertyChangedEventHandler PropertyChanged;

    //[NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged(string PropertyName) => _AsyncOp.Post(_PropertyChanged, new PropertyChangedEventArgs(PropertyName));

    private readonly ObservableCollection<T> _Collection;

    private readonly AsyncOperation _AsyncOp = AsyncOperationManager.CreateOperation(null);
    private readonly SendOrPostCallback _CollectionChanged;
    private readonly SendOrPostCallback _PropertyChanged;

    public object? SyncRoot { get; set; }
    public ObservableCollectionSyncWrapper() : this(new ObservableCollection<T>()) { }

    public ObservableCollectionSyncWrapper(ObservableCollection<T> Collection)
    {
        _CollectionChanged = o => CollectionChanged?.Invoke(this, (NotifyCollectionChangedEventArgs)o);
        _PropertyChanged   = o => PropertyChanged?.Invoke(this, (PropertyChangedEventArgs)o);

        _Collection                                           =  Collection;
        _Collection.CollectionChanged                         += (_, e) => OnCollectionChanged(e);
        ((INotifyPropertyChanged)_Collection).PropertyChanged += (_, e) => OnPropertyChanged(e.PropertyName);
    }

    public IEnumerator<T> GetEnumerator() => _Collection.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Collection).GetEnumerator();

    public void Add(T item)
    {
        if (SyncRoot != null)
            lock (SyncRoot)
                _Collection.Add(item);
        else
            _Collection.Add(item);
    }

    public void Clear()
    {
        if (SyncRoot != null)
            lock (SyncRoot)
                _Collection.Clear();
        else
            _Collection.Clear();
    }

    public bool Contains(T item)
    {
        bool result;
        if (SyncRoot != null)
            lock (SyncRoot)
                result = _Collection.Contains(item);
        else
            result = _Collection.Contains(item);
        return result;
    }

    public void CopyTo(T[] array, int Index)
    {
        if (SyncRoot != null)
            lock (SyncRoot)
                _Collection.CopyTo(array, Index);
        else
            _Collection.CopyTo(array, Index);
    }

    public bool Remove(T item)
    {
        bool result;
        if (SyncRoot != null)
            lock (SyncRoot)
                result = _Collection.Remove(item);
        else
            result = _Collection.Remove(item);
        return result;
    }

    public int Count => _Collection.Count;
    public bool IsReadOnly => ((ICollection<T>)_Collection).IsReadOnly;

    public static implicit operator ObservableCollectionSyncWrapper<T>(ObservableCollection<T> collection) => new(collection);
    public static explicit operator ObservableCollection<T>(ObservableCollectionSyncWrapper<T> wrapper) => wrapper._Collection;
}

public class RangeObservableCollection<T> : ObservableCollection<T>
{
    private bool _SuppressNotification;

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (!_SuppressNotification)
            base.OnCollectionChanged(e);
    }

    public void AddRange(IEnumerable<T> list)
    {
        if (list is null)
            throw new ArgumentNullException(nameof(list));

        _SuppressNotification = true;

        foreach (var item in list) Add(item);
        _SuppressNotification = false;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}

public class DeferEventObservableCollection<T> : ObservableCollection<T>
{
    private readonly List<NotifyCollectionChangedEventArgs> _DeferredEvents = new();
    private bool _HasQueuedDispatcherUpdate;
    private readonly int _Threshold;
    private readonly object _SyncRoot = new();

    public DeferEventObservableCollection(int threshold = 10) => _Threshold = threshold;

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        lock (_SyncRoot)
        {
            _DeferredEvents.Add(e);
            if (!_HasQueuedDispatcherUpdate)
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    lock (_SyncRoot)
                    {
                        if (_DeferredEvents.Count > _Threshold)
                            base.OnCollectionChanged(
                                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        else
                            foreach (var ev in _DeferredEvents) base.OnCollectionChanged(ev);
                        _DeferredEvents.Clear();
                        _HasQueuedDispatcherUpdate = false;
                    }
                }));
            _HasQueuedDispatcherUpdate = true;
        }
    }
}

public class DeferredRefreshObservableCollection<T> : ObservableCollection<T>
{
    private int _RefreshDeferred;
    private bool _Modified;

    /// <summary>Raises the <see cref="E:CollectionChanged" /> event.</summary>
    /// <param name="e">The instance containing the event data.</param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (_RefreshDeferred > 0)
        {
            _Modified = true;
            return;
        }

        base.OnCollectionChanged(e);
    }

    /// <summary>Отложить посылку уведомлений об изменении состава коллекции</summary><returns>Дескриптор</returns>
    public DeferRefreshHelper DeferRefresh() => new(this);

    /// <summary>Дескриптор отложенных изменений</summary>
    public struct DeferRefreshHelper : IDisposable
    {
        private DeferredRefreshObservableCollection<T> _Owner;

        internal DeferRefreshHelper(DeferredRefreshObservableCollection<T> owner)
        {
            _Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _Owner._RefreshDeferred++;
        }

        /// <summary>Уменьшить счетчик отложенной посылки обновлений</summary>
        public void Dispose()
        {
            if (null == _Owner) return;

            var temp = _Owner;
            _Owner = null;

            if (0 == --temp._RefreshDeferred && temp._Modified)
                temp.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}

public class SynchronizationObservableCollection<T> : ObservableCollection<T>
{
    private readonly SynchronizationContext _Context = SynchronizationContext.Current;

    public SynchronizationObservableCollection() { }

    public SynchronizationObservableCollection(IEnumerable<T> list) : base(list) { }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (SynchronizationContext.Current == _Context)
            RaiseCollectionChanged(e); // Execute the CollectionChanged event on the current thread
        else
            _Context.Send(RaiseCollectionChanged, e); // Raises the CollectionChanged event on the creator thread
    }

    private void RaiseCollectionChanged(object param) => base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (SynchronizationContext.Current == _Context)
            RaisePropertyChanged(e); // Execute the PropertyChanged event on the current thread
        else
            _Context.Send(RaisePropertyChanged, e); // Raises the PropertyChanged event on the creator thread
    }

    private void RaisePropertyChanged(object param) => base.OnPropertyChanged((PropertyChangedEventArgs)param);
}

public class ObservableRangeCollection<T> : ObservableCollection<T>
{
    private const string __CountString = "Count";
    private const string __IndexerName = "Item[]";

    public ObservableRangeCollection() { }

    public ObservableRangeCollection(IEnumerable<T> collection) : base(collection) { }

    public ObservableRangeCollection(List<T> list) : base(list) { }

    public void AddRange(IEnumerable<T> items)
    {
        var new_items = new List<T>();
        foreach (var item in items)
        {
            Items.Add(item);
            new_items.Add(item);
        }
        if (new_items.Count == 0) return;
        OnPropertyChanged(new PropertyChangedEventArgs(__CountString));
        OnPropertyChanged(new PropertyChangedEventArgs(__IndexerName));
        var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new_items);
        OnCollectionChanged(e);
    }

    public void RemoveRange(IEnumerable<T> items)
    {
        var old_items = new List<T>();
        foreach (var item in items)
        {
            Items.Remove(item);
            old_items.Add(item);
        }
        if (old_items.Count == 0) return;
        OnPropertyChanged(new PropertyChangedEventArgs(__CountString));
        OnPropertyChanged(new PropertyChangedEventArgs(__IndexerName));
        var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, old_items);
        OnCollectionChanged(e);
    }
}