using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace MathCore.WPF.WindowTest;

public static class ListBoxEx
{
    #region Attached property DraggableItems : bool - Перетаскиваемые элементы

    /// <summary>Перетаскиваемые элементы</summary>
    public static readonly DependencyProperty DraggableItemsProperty =
        DependencyProperty.RegisterAttached(
            "DraggableItems",
            typeof(bool),
            typeof(ListBoxEx),
            new(default(bool), OnDraggableItemsChanged));

    /// <summary>Перетаскиваемые элементы</summary>
    [AttachedPropertyBrowsableForType(typeof(ListBox))]
    public static void SetDraggableItems(DependencyObject d, bool value) => d.SetValue(DraggableItemsProperty, value);

    /// <summary>Перетаскиваемые элементы</summary>
    public static bool GetDraggableItems(DependencyObject d) => (bool)d.GetValue(DraggableItemsProperty);

    private static void OnDraggableItemsChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        if (D is not ListBox list) return;

        list.PreviewMouseLeftButtonDown += OnListMouseDown;

        var item_style = list.ItemContainerStyle ??= new(typeof(ListBoxItem));
        if ((bool)E.NewValue)
        {
            list.AllowDrop = true;
            list.PreviewMouseLeftButtonDown += OnListMouseDown;
            list.Drop += OnListDrop;
        }
        else
        {
            list.AllowDrop = false;
            list.PreviewMouseLeftButtonDown -= OnListMouseDown;
            list.Drop -= OnListDrop;
        }
    }

    private static void OnListDrop(object Sender, DragEventArgs E)
    {
        if ((E.OriginalSource as DependencyObject).FindVisualParent<ListBoxItem>() is not { DataContext: { } context } item)
            return;

        var drop_position = E.GetPosition(item);

        var percent = drop_position.Y / item.ActualHeight;
    }

    private static void OnListMouseDown(object Sender, MouseButtonEventArgs E)
    {
        if((E.OriginalSource as DependencyObject).FindVisualParent<ListBoxItem>() is not { DataContext: { } context } item) 
            return;

        item.IsSelected = true;
        DragDrop.DoDragDrop(item, context, DragDropEffects.Move);

    }

    #endregion
}
