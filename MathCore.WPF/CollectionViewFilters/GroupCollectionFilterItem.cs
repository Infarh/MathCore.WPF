using System.Collections.ObjectModel;
using System.Windows;

namespace MathCore.WPF;

public class GroupCollectionFilterItem : DependencyObject
{
    private bool _Enabled = true;

    public bool Enabled
    {
        get => _Enabled;
        set
        {
            if (_Enabled == value) return;
            _Enabled = value;
            EnabledChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? EnabledChanged;

    public ObservableCollection<object> Items { get; } = new();

    public object Key { get; }

    public GroupCollectionFilterItem(object key) => Key = key;

    public override string ToString() => $"Group[{Key}]({Items.Count}) - enabled:{_Enabled}";
}