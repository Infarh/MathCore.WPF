using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Behaviors;

public class TreeViewBindableSelectedItem : Behavior<TreeView>
{
    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(
            nameof(SelectedItem),
            typeof(object),
            typeof(TreeViewBindableSelectedItem),
            new FrameworkPropertyMetadata(default, OnSelectedItemPropertyChanged) { BindsTwoWayByDefault = true });

    private static void OnSelectedItemPropertyChanged(DependencyObject D, DependencyPropertyChangedEventArgs E) => (D as TreeViewBindableSelectedItem)?.OnSelectedItemPropertyChanged(E.NewValue);

    protected virtual void OnSelectedItemPropertyChanged(object? item)
    {
        if (item is null) return;
        var tree_view = AssociatedObject;
        if (tree_view is null || ReferenceEquals(tree_view.SelectedItem, item)) return;
        SelectTreeViewItem(tree_view, item);
    }

    private static bool SelectTreeViewItem(ItemsControl ParentContainer, object item)
    {
        if (ParentContainer is null) throw new ArgumentNullException(nameof(ParentContainer));

        foreach (var tree_item in ParentContainer.Items)
        {
            if (ParentContainer.ItemContainerGenerator.ContainerFromItem(tree_item) is not TreeViewItem view_item) continue;

            if (Equals(tree_item, item))
            {
                view_item.IsSelected = true;
                view_item.BringIntoView();
                return true;
            }

            if (view_item.Items.Count == 0) continue;

            var is_expanded = view_item.IsExpanded;
            view_item.IsExpanded = true;
            view_item.UpdateLayout();

            if (SelectTreeViewItem(view_item, item)) return true;

            view_item.IsExpanded = is_expanded;
        }

        return false;
    }

    public object? SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

    private Style? _CustomItemContainerStyle;
    private EventSetter? _TreeViewItemStyleLoadedEventSetter;

    protected override void OnAttached()
    {
        base.OnAttached();
        var tree_view = AssociatedObject;
        tree_view.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        var style = tree_view.ItemContainerStyle ?? (_CustomItemContainerStyle = tree_view.ItemContainerStyle = new(typeof(TreeViewItem)));
        style.Setters.Add(_TreeViewItemStyleLoadedEventSetter = new(FrameworkElement.LoadedEvent, new RoutedEventHandler(OnTreeViewItem_Loaded)));
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        var tree_view = AssociatedObject;
        if (tree_view is null) return;
        tree_view.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
        if (ReferenceEquals(tree_view.ItemContainerStyle, _CustomItemContainerStyle))
            tree_view.ItemContainerStyle = null;
        else
            tree_view.ItemContainerStyle?.Setters.Remove(_TreeViewItemStyleLoadedEventSetter);
    }

    private void OnTreeViewSelectedItemChanged(object? sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (!ReferenceEquals(SelectedItem, e.NewValue))
            SelectedItem = e.NewValue;
    }

    protected virtual void OnTreeViewItem_Loaded(object? Sender, RoutedEventArgs? _)
    {
        //var item = (TreeViewItem) Sender;
    }
}