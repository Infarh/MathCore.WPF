using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
// ReSharper disable EventNeverSubscribedTo.Global

namespace MathCore.WPF;

public class EventsTrigger(Func<object, bool> Checker) : INotifyPropertyChanged, IObservable<bool>
{
    public abstract class TriggersPool : INotifyCollectionChanged
    {
        private readonly Dictionary<string, EventsTrigger> _Triggers = [];

        public EventsTrigger? this[string Name]
        {
            get { lock (_Triggers) return _Triggers.GetValue(Name); }
            set
            {
                lock (_Triggers)
                    if (!_Triggers.ContainsKey(Name))
                    {
                        _Triggers.Add(Name, value);
                        CollectionChanged.ThreadSafeInvoke(this,
                            new(NotifyCollectionChangedAction.Add, new { value }));
                    }
                    else
                    {
                        var old_value = _Triggers[Name];
                        _Triggers[Name] = value;
                        CollectionChanged.ThreadSafeInvoke(this,
                            new(NotifyCollectionChangedAction.Replace, value, old_value));
                    }
            }
        }

        internal TriggersPool() { }

        public bool IsTriggerExist(string Name) { lock (_Triggers) return _Triggers.ContainsKey(Name); }

        public bool Remove(string Name)
        {
            lock (_Triggers)
            {
                if (!_Triggers.TryGetValue(Name, out var trigger)) return false;
                var is_removed = _Triggers.Remove(Name);
                CollectionChanged.ThreadSafeInvoke(this, new(NotifyCollectionChangedAction.Remove, new { trigger }));
                return is_removed;
            }
        }

        #region Implementation of INotifyCollectionChanged

        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }

    private class EventTriggersPool : TriggersPool;

    static EventsTrigger() => __Pool = new EventTriggersPool();

    private static readonly TriggersPool __Pool;

    private static TriggersPool Pool => __Pool;

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged( /*[CallerMemberName]*/ string PropertyName /* = null*/)
        => PropertyChanged.ThreadSafeInvoke(this, PropertyName);

    public event EventHandler? Checked;
    public event EventHandler? Raise;
    public event EventHandler? Falloff;

    private readonly object _SyncRoot = new();
    private bool _LastState;

    private readonly List<IObserver<bool>> _StateObservers = [];

    public bool LastState => _LastState;

    public EventsTrigger(Func<bool> Checker) : this(_ => Checker()) { }

    public bool Check() => Check(null);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool Check(object? obj)
    {
        bool current;
        lock (_SyncRoot)
        {
            var last             = _LastState;
            current = _LastState = Checker(obj);
            Checked?.Invoke(this, EventArgs.Empty);
            if (current == last) return current;
            (current ? Raise : Falloff)?.Invoke(this, EventArgs.Empty);
        }
        OnPropertyChanged(nameof(LastState));
        _StateObservers.ForEach(o => o.OnNext(current));
        return current;
    }

    #region Implementation of IObservable<out bool>

    /// <inheritdoc />
    public IDisposable Subscribe(IObserver<bool> observer)
    {
        _StateObservers.Add(observer);
        return LambdaDisposable.OnDisposed(() => _StateObservers.Remove(observer));
    }

    #endregion
}