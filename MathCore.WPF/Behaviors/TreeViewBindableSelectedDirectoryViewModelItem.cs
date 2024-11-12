using System.Windows;
using System.Windows.Controls;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Behaviors;

/// <summary>Представляет элемент модели представления для дерева, который привязывается к выбранному каталогу.</summary>
public class TreeViewBindableSelectedDirectoryViewModelItem : TreeViewBindableSelectedItem
{
    /// <summary>Вызывается при изменении свойства выбранного элемента.</summary>
    /// <param name="item">Новый выбранный элемент.</param>
    protected override void OnSelectedItemPropertyChanged(object? item)
    {
        // Проверяем, является ли новый элемент DirectoryViewModel
        if (item is not DirectoryViewModel model) return;

        // Получаем связанное дерево представления
        var tree_view = AssociatedObject;

        // Если дерево представления равно null или выбранный элемент уже тот же, возвращаемся
        if (tree_view is null || ReferenceEquals(tree_view.SelectedItem, item)) return;

        // Выбираем элемент дерева представления, соответствующий новому выбранному каталогу
        SelectTreeViewItem(tree_view, model.Directory.FullName);
    }

    /// <summary>Рекурсивно выбирает элемент дерева представления, соответствующий указанному пути.</summary>
    /// <param name="container">Элемент управления, содержащий элементы дерева представления.</param>
    /// <param name="path">Путь для выбора.</param>
    /// <returns>True, если путь был найден и выбран, false в противном случае.</returns>
    private static bool SelectTreeViewItem(ItemsControl container, string path)
    {
        // Перебираем модели каталогов в контейнере
        foreach (var model in container.Items.OfType<DirectoryViewModel>())
        {
            // Получаем элемент дерева представления, соответствующий текущей модели каталога
            if (container.ItemContainerGenerator.ContainerFromItem(model) is not TreeViewItem view) continue;

            // Если текущая модель каталога совпадает с путем, выбираем ее
            if (model.Equals(path))
            {
                view.IsSelected = true;
                view.BringIntoView();
                return true;
            }

            // Если путь не начинается с полного имени текущего каталога, пропускаем его
            if (!path.StartsWith(model.Directory.FullName, StringComparison.InvariantCultureIgnoreCase)) continue;

            // Разворачиваем текущий элемент дерева представления и рекурсивно ищем его дочерние элементы
            var is_expanded = view.IsExpanded;
            view.IsExpanded = true;
            view.UpdateLayout();
            if (view.Items.Count == 0) return false;
            if (SelectTreeViewItem(view, path)) return true;
            view.IsExpanded = is_expanded;
            view.UpdateLayout();
            return false;
        }

        // Если путь не был найден, возвращаем false
        return false;
    }

    /// <summary>Вызывается при загрузке элемента дерева представления.</summary>
    /// <param name="sender">Элемент дерева представления, который был загружен.</param>
    /// <param name="_">Аргументы события.</param>
    protected override void OnTreeViewItem_Loaded(object? sender, RoutedEventArgs? _)
    {
        // Проверяем, является ли выбранный элемент DirectoryViewModel
        if (SelectedItem is not DirectoryViewModel selected_item)
            return;

        // Получаем элемент дерева представления, который был загружен
        var tree_view_item = (TreeViewItem)sender!;

        // Получаем модель каталога, соответствующую загруженному элементу дерева представления
        var current_item = (DirectoryViewModel)tree_view_item.DataContext;

        // Получаем полное имя выбранного каталога и текущего каталога
        var selected_path = selected_item.Directory.FullName;
        var current_path = current_item.Directory.FullName;

        // Если выбранный каталог совпадает с текущим каталогом, выбираем элемент дерева представления
        if (string.Equals(selected_path, current_path, StringComparison.InvariantCultureIgnoreCase))
        {
            tree_view_item.IsSelected = true;
            tree_view_item.BringIntoView();
        }
        // Если выбранный каталог является подкаталогом текущего каталога, разворачиваем элемент дерева представления
        else if (selected_path.StartsWith(current_path, StringComparison.InvariantCultureIgnoreCase))
        {
            tree_view_item.IsExpanded = true;
            tree_view_item.UpdateLayout();
        }
    }
}