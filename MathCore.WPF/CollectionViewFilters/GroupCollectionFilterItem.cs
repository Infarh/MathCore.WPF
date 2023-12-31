using System.Collections.ObjectModel;
using System.Windows;

namespace MathCore.WPF;

public class GroupCollectionFilterItem(object key) : DependencyObject
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

    public ObservableCollection<object> Items { get; } = [];

    public object Key => key;

    public override string ToString() => $"Group[{key}]({Items.Count}) - enabled:{_Enabled}";
}