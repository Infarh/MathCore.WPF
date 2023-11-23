using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace MathCore.WPF;

public class CollectionFilterItem<TValue, TCriteria>(ObservableCollection<TValue?> list) : ReadOnlyObservableCollection<TValue>(list)
{
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

    public CollectionFilterItem(TCriteria key) : this(new ObservableCollection<TValue>()) => Key = key;

    public CollectionFilterItem(
#if NETCOREAPP
        [DisallowNull]
#else
        [MathCore.Annotations.NotNull]
#endif
        TCriteria key, IEnumerable<TValue?> items) : this(new ObservableCollection<TValue>(items)) => Key = key;

    internal void Add(TValue? value)
    {
        if (!list.Contains(value))
            list.Add(value);
    }

    internal bool Remove(TValue? value) => list.Remove(value);
}