#nullable enable
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

using MathCore.WPF.Commands;

namespace MathCore.WPF;

public class ItemsCollection<T>(
    Func<Task<T>> CreatorAsync,
    PropertyChangedEventHandler? ItemPropertyChanged = null,
    Action<T>? Editor = null)
    : SelectableCollection<T>
{
    [field: MaybeNull, AllowNull]
    public ICommand AddCommand => field ??= Command.New(OnCreateCommandExecuted);
    private async Task OnCreateCommandExecuted()
    {
        var item = await CreatorAsync();
        Add(item);
        SelectedItem = item;
    }

    [field: MaybeNull]
    public ICommand RemoveCommand => field ??= Command.New<T>(t => Remove(t), Contains);

    [field: MaybeNull]
    public ICommand EditCommand => field ??= Command.New<T>(t => Editor?.Invoke(t!), Contains);

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (ItemPropertyChanged is not null)
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add when e.NewItems?.OfType<INotifyPropertyChanged>() is { } new_items:
                    foreach (var item in new_items)
                        item.PropertyChanged += ItemPropertyChanged;
                    break;

                case NotifyCollectionChangedAction.Remove when e.OldItems?.OfType<INotifyPropertyChanged>() is { } old_items:
                    foreach (var item in old_items)
                        item.PropertyChanged -= ItemPropertyChanged;
                    break;
            }

        base.OnCollectionChanged(e);
    }
}
