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

    private static bool SelectTreeViewItem(ItemsControl ParentContainer, object target)
    {
        if (ParentContainer is null) throw new ArgumentNullException(nameof(ParentContainer));

        foreach (var item in ParentContainer.Items)
        {
            var view = (TreeViewItem)ParentContainer.ItemContainerGenerator.ContainerFromItem(item);
            if (view is null) continue;

            if (Equals(item, target))
            {
                view.IsSelected = true;
                view.BringIntoView();
                return true;
            }

            if (view.Items.Count == 0) continue;
            var is_expanded = view.IsExpanded;
            view.IsExpanded = true;
            view.UpdateLayout();
            if (SelectTreeViewItem(view, target)) return true;
            view.IsExpanded = is_expanded;
        }

        //foreach (var item in ParentContainer.Items)
        //{
        //    var view = (TreeViewItem)ParentContainer.ItemContainerGenerator.ContainerFromItem(item);
        //    if (view is null || view.Items.Count == 0) continue;
        //    view.IsExpanded = true;
        //    view.UpdateLayout();
        //    if (SelectTreeViewItem(view, target)) return true;
        //    view.IsExpanded = false;
        //}
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
        var style = tree_view.ItemContainerStyle ?? (_CustomItemContainerStyle = tree_view.ItemContainerStyle = new Style(typeof(TreeViewItem)));
        style.Setters.Add(_TreeViewItemStyleLoadedEventSetter = new EventSetter(FrameworkElement.LoadedEvent, new RoutedEventHandler(OnTreeViewItem_Loaded)));
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