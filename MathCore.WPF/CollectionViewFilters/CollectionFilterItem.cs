using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace MathCore.WPF;

public class CollectionFilterItem<TValue, TCriteria> : ReadOnlyObservableCollection<TValue>
{
    private readonly ObservableCollection<TValue?> _InternalCollection;

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

    public CollectionFilterItem(ObservableCollection<TValue?> list) : base(list) => _InternalCollection = list;

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
        if (!_InternalCollection.Contains(value))
            _InternalCollection.Add(value);
    }

    internal bool Remove(TValue? value) => _InternalCollection.Remove(value);
}