using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

public class ListBoxScrollOnNewItem : Behavior<ListBox>
{
    #region Enabled : bool - Включено

    /// <summary>Включено</summary>
    public static readonly DependencyProperty EnabledProperty =
        DependencyProperty.Register(
            nameof(Enabled),
            typeof(bool),
            typeof(ListBoxScrollOnNewItem),
            new(true));

    /// <summary>Включено</summary>
    //[Category("")]
    [Description("Включено")]
    public bool Enabled { get => (bool)GetValue(EnabledProperty); set => SetValue(EnabledProperty, value); }

    #endregion

    protected override void OnAttached()
    {
        var list_box = AssociatedObject;

        var items_source_property_descriptor = TypeDescriptor.GetProperties(list_box)["ItemsSource"];
        items_source_property_descriptor.AddValueChanged(list_box, ListBox_ItemsSourceChanged);

        SetCollection(list_box.ItemsSource as INotifyCollectionChanged);
    }

    private INotifyCollectionChanged? _ListCollection;

    private void SetCollection(INotifyCollectionChanged? collection)
    {
        if (_ListCollection is { } old_collection)
            old_collection.CollectionChanged -= OnCollectionChanged;

        _ListCollection = collection;

        if (collection is not null)
            collection.CollectionChanged += OnCollectionChanged;
    }

    
    private void ListBox_ItemsSourceChanged(object? Sender, EventArgs E) => SetCollection(AssociatedObject?.ItemsSource as INotifyCollectionChanged);

    protected override void OnDetaching()
    {
        var list_box = AssociatedObject;

        var items_source_property_descriptor = TypeDescriptor.GetProperties(list_box)["ItemsSource"];
        items_source_property_descriptor.RemoveValueChanged(list_box, ListBox_ItemsSourceChanged);

        SetCollection(null);
    }

    private void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
    {
        if (!Enabled || E.Action != NotifyCollectionChangedAction.Add || E.NewItems is not [ var new_item, .. ]) return;

        var list = AssociatedObject;
        list.ScrollIntoView(new_item);
        list.SelectedItem = new_item;
    }
}