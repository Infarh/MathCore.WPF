using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для привязки выбранного элемента TreeView</summary>
public class TreeViewBindableSelectedItem : Behavior<TreeView>
{
    /// <summary>DependencyProperty для свойства SelectedItem</summary>
    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(
            nameof(SelectedItem),
            typeof(object),
            typeof(TreeViewBindableSelectedItem),
            new FrameworkPropertyMetadata(default, OnSelectedItemPropertyChanged) { BindsTwoWayByDefault = true });

    /// <summary>Обработчик изменения свойства SelectedItem</summary>
    /// <param name="D">Объект зависимости</param>
    /// <param name="E">Аргументы изменения свойства</param>
    private static void OnSelectedItemPropertyChanged(DependencyObject D, DependencyPropertyChangedEventArgs E) => (D as TreeViewBindableSelectedItem)?.OnSelectedItemPropertyChanged(E.NewValue);

    /// <summary>Вызывается при изменении выбранного элемента</summary>
    /// <param name="item">Новый выбранный элемент</param>
    protected virtual void OnSelectedItemPropertyChanged(object? item)
    {
        if (item is null) return;
        var tree_view = AssociatedObject;
        if (tree_view is null || ReferenceEquals(tree_view.SelectedItem, item)) return;
        SelectTreeViewItem(tree_view, item);
    }

    /// <summary>Выбирает элемент в дереве</summary>
    /// <param name="ParentContainer">Родительский контейнер элементов</param>
    /// <param name="item">Элемент для выбора</param>
    /// <returns>True, если элемент найден и выбран</returns>
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

    /// <summary>Выбранный элемент дерева</summary>
    public object? SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

    /// <summary>Пользовательский стиль контейнера элементов</summary>
    private Style? _CustomItemContainerStyle;
    /// <summary>Установщик события загрузки элемента дерева</summary>
    private EventSetter? _TreeViewItemStyleLoadedEventSetter;

    /// <summary>Вызывается при присоединении поведения к дереву</summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        var tree_view = AssociatedObject;
        tree_view.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        var style = tree_view.ItemContainerStyle ?? (_CustomItemContainerStyle = tree_view.ItemContainerStyle = new(typeof(TreeViewItem)));
        style.Setters.Add(_TreeViewItemStyleLoadedEventSetter = new(FrameworkElement.LoadedEvent, new RoutedEventHandler(OnTreeViewItem_Loaded)));
    }

    /// <summary>Вызывается при отсоединении поведения от дерева</summary>
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

    /// <summary>Обработчик изменения выбранного элемента дерева</summary>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    private void OnTreeViewSelectedItemChanged(object? sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (!ReferenceEquals(SelectedItem, e.NewValue))
            SelectedItem = e.NewValue;
    }

    /// <summary>Обработчик загрузки элемента дерева</summary>
    /// <param name="Sender">Источник события</param>
    /// <param name="_">Аргументы события</param>
    protected virtual void OnTreeViewItem_Loaded(object? Sender, RoutedEventArgs? _)
    {
        //var item = (TreeViewItem) Sender;
    }
}