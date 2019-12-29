using System;
using System.Windows;
using System.Windows.Controls;
using MathCore.Annotations;

namespace MathCore.WPF.Behaviors
{
    public class TreeViewBindableSelectedDirectoryViewModelItem : TreeViewBindableSelectedItem
    {
        protected override void OnSelectedItemPropertyChanged(object item)
        {
            if (!(item is DirectoryViewModel model)) return;
            var tree_view = AssociatedObject;
            if (tree_view is null || ReferenceEquals(tree_view.SelectedItem, item)) return;
            SelectTreeViewItem(tree_view, model.Directory.FullName);
        }

        private static bool SelectTreeViewItem([NotNull] ItemsControl Container, [NotNull] string path)
        {
            foreach (DirectoryViewModel model in Container.Items)
            {
                var view = (TreeViewItem)Container.ItemContainerGenerator.ContainerFromItem(model);
                if (view is null) continue;
                if (model.Equals(path))
                {
                    view.IsSelected = true;
                    view.BringIntoView();
                    return true;
                }

                if (!path.StartsWith(model.Directory.FullName, StringComparison.InvariantCultureIgnoreCase)) continue;
                var is_expanded = view.IsExpanded;
                view.IsExpanded = true;
                view.UpdateLayout();
                if (view.Items.Count == 0) return false;
                if (SelectTreeViewItem(view, path)) return true;
                view.IsExpanded = is_expanded;
                view.UpdateLayout();
                return false;
            }

            return false;
        }

        protected override void OnTreeViewItem_Loaded(object Sender, RoutedEventArgs _)
        {
            var selected_item = (DirectoryViewModel)SelectedItem;
            if (selected_item is null) return;
            var tree_view_item = (TreeViewItem)Sender ?? throw new InvalidOperationException();
            var current_item = (DirectoryViewModel)tree_view_item.DataContext;
            var selected_path = selected_item.Directory.FullName;
            var current_path = current_item.Directory.FullName;
            if (string.Equals(selected_path, current_path, StringComparison.InvariantCultureIgnoreCase))
            {
                tree_view_item.IsSelected = true;
                tree_view_item.BringIntoView();
            }
            else if (selected_path.StartsWith(current_path, StringComparison.InvariantCultureIgnoreCase))
            {
                tree_view_item.IsExpanded = true;
                tree_view_item.UpdateLayout();
            }
        }
    }
}