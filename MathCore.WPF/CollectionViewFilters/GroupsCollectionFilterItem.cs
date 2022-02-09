using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;

namespace MathCore.WPF;

public class GroupsCollectionFilterItem : CollectionViewFilterItem
{
    public ObservableCollection<GroupCollectionFilterItem> Groups { get; } = new();

    private IEnumerable? _ViewSource;

    private void CheckView(IEnumerable? ViewSource)
    {
        if (ReferenceEquals(_ViewSource, ViewSource))
            return;

        if (_ViewSource is INotifyCollectionChanged old_collection)
            old_collection.CollectionChanged -= OnItemsChanged;

        _ViewSource = ViewSource;
        if (_ViewSource is null)
            return;

        UpdateGroups();

        if (_ViewSource is INotifyCollectionChanged new_collection)
            new_collection.CollectionChanged += OnItemsChanged;
    }

    private async void OnItemsChanged(object? Sender, NotifyCollectionChangedEventArgs E)
    {
        switch (E.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (E.NewItems is { } added) 
                    foreach (var item in added) 
                        await AddItemAsync(item).ConfigureAwait(false);
                break;

            case NotifyCollectionChangedAction.Remove:
                if (E.OldItems is { } old) 
                    foreach (var item in old) 
                        RemoveItem(item);
                break;

            case NotifyCollectionChangedAction.Replace:
                if (E.OldItems is { } removed_items) 
                    foreach (var item in removed_items)
                        RemoveItem(item);
                if (E.NewItems is { } new_items)
                    foreach (var item in new_items)
                        await AddItemAsync(item).ConfigureAwait(false);
                break;

            case NotifyCollectionChangedAction.Reset:
                UpdateGroups();
                break;
        }
    }

    private void ClearGroups() => Groups.Clear();

    public void UpdateGroups()
    {
        ClearGroups();
        if (_ViewSource is not { } view) return;
        foreach (var item in view)
            AddItem(item);
    }

    private void AddItem(object item)
    {
        //var value = ;
        if (GetItemValue(item) is not { } value) 
            return;

        if (Groups.FirstOrDefault(g => Equals(g.Key, value)) is not { } group) 
            CreateGroup(out group, value);

        group.Items.Add(item);
    }

    private async Task AddItemAsync(object? item)
    {
        if (await GetItemValueAsync(item).ConfigureAwait(false) is not { } value)
            return;

        if (Groups.FirstOrDefault(g => Equals(g.Key, value)) is not { } group) 
            CreateGroup(out group, value);

        group.Items.Add(item);
    }

    private void CreateGroup(out GroupCollectionFilterItem Group, object Value)
    {
        Group = new GroupCollectionFilterItem(Value);
        if (Value is IComparable comparable && Groups.Count > 0)
        {
            int i;
            for (i = 0; i < Groups.Count; i++)
            {
                if (comparable.CompareTo(Groups[i].Key) >= 0) continue;
                Groups.Insert(i, Group);
                i = -1;
                break;
            }
            if (i > 0) 
                Groups.Add(Group);
        }
        else
            Groups.Add(Group);

        Group.EnabledChanged += GroupEnableChanged;
    }

    private void GroupEnableChanged(object? Sender, EventArgs E) => RefreshSource(this, default);

    private void RemoveItem(object item)
    {
        var value = GetItemValue(item);
        if (value is null) return;
        var group = Groups.FirstOrDefault(g => Equals(g.Key, value));
        if (group?.Items.Remove(item) == false || group?.Items.Count != 0) return;
        group.EnabledChanged -= GroupEnableChanged;
        Groups.Remove(group);
    }

    private async Task RemoveItemAsync(object? item)
    {
        if (await GetItemValueAsync(item).ConfigureAwait(false) is not { } value) 
            return;

        if (Groups.FirstOrDefault(g => Equals(g.Key, value)) is { } group 
            && group.Items.Remove(item) 
            && group.Items.Count == 0)
        {
            group.EnabledChanged -= GroupEnableChanged;
            Groups.Remove(group);
        }
    }

    /// <inheritdoc />
    protected override Freezable CreateInstanceCore()
    {
        var filter = new GroupsCollectionFilterItem();
        filter.SetSource(_Source);
        return filter;
    }

    /// <inheritdoc />
    protected override void OnFilter(object Sender, FilterEventArgs E)
    {
        CheckView(((CollectionViewSource)Sender).Source as IEnumerable);
        if (!Enabled || !E.Accepted || !Groups.Any(g => g.Enabled)) 
            return;

        var value = GetItemValue(E.Item);
        foreach (var group in Groups)
            if (Equals(group.Key, value))
            {
                E.Accepted = group.Enabled;
                break;
            }
    }
}