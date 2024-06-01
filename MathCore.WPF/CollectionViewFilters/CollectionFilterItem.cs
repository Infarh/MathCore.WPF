using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace MathCore.WPF;

public class CollectionFilterItem<TValue, TCriteria>(ObservableCollection<TValue?> collection) : ReadOnlyObservableCollection<TValue>(collection)
{
    public TCriteria? Key { get; }

    private bool _Enabled;

    private readonly ObservableCollection<TValue?> _Collection = collection;

    public bool Enabled
    {
        get => _Enabled;
        set
        {
            if (_Enabled == value) return;
            _Enabled = value;
            OnPropertyChanged(new(nameof(Enabled)));
        }
    }

    public CollectionFilterItem(TCriteria key) : this([]) => Key = key;

    public CollectionFilterItem(
#if NETCOREAPP
        [DisallowNull]
#else
        [Annotations.NotNull]
#endif
        TCriteria key, IEnumerable<TValue?> items) : this(new ObservableCollection<TValue>(items)) => Key = key;

    internal void Add(TValue? value)
    {
        if (!_Collection.Contains(value))
            _Collection.Add(value);
    }

    internal bool Remove(TValue? value) => _Collection.Remove(value);
}