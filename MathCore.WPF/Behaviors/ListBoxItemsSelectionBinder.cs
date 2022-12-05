using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

public class ListBoxItemsSelectionBinder : Behavior<ListBox>
{
    #region SelectedItems : IList - Выбранные элементы

    /// <summary>Выбранные элементы</summary>
    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.Register(
            nameof(SelectedItems),
            typeof(IList),
            typeof(ListBoxItemsSelectionBinder),
            new FrameworkPropertyMetadata(
                default(IList),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedItemsChanged));

    private static void OnSelectedItemsChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        var behavior            = (ListBoxItemsSelectionBinder)D;
        var list_selected_items = behavior.AssociatedObject.SelectedItems;
        var new_selected_items  = (IList)E.NewValue;
        if (ReferenceEquals(list_selected_items, new_selected_items)) return;

        var to_remove = new List<object>();
        foreach (var list_item in list_selected_items)
            if (!new_selected_items.Contains(list_item) && list_item is not null)
                to_remove.Add(list_item);

        foreach (var item in to_remove)
            list_selected_items.Remove(item);

        foreach (var new_selected_item in new_selected_items)
            if (!list_selected_items.Contains(new_selected_item))
                list_selected_items.Add(new_selected_item);
    }

    /// <summary>Выбранные элементы</summary>
    //[Category("")]
    [Description("Выбранные элементы")]
    public IList SelectedItems
    {
        get => (IList)GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    #endregion

    protected override void OnAttached() => AssociatedObject.SelectionChanged += OnSelectionChanged;

    protected override void OnDetaching() => AssociatedObject.SelectionChanged -= OnSelectionChanged;

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
        SelectedItems = AssociatedObject.SelectedItems;
}