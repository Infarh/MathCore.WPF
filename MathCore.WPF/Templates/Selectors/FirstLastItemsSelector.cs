using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MathCore.Trees;
using MathCore.WPF.Extensions;

namespace MathCore.WPF.Templates.Selectors
{
    public class FirstLastItemsSelector : DataTemplateSelector
    {
        public DataTemplate? FirstItemTemplate { get; set; }
        public DataTemplate? LastItemTemplate { get; set; }
        public DataTemplate? OtherItemTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object _, DependencyObject item)
        {
            var s = item.AsTreeNodeVisual().EnumerateParents().OfType<ItemsControl>().First();
            if ((item is ContentControl || item is ContentPresenter) && item.FindVisualParent<ItemsControl>() is { } list)
            {
                var index = list.ItemContainerGenerator.IndexFromContainer(item);
                if (index == 0) return FirstItemTemplate;
                if (index == list.Items.Count - 1) return LastItemTemplate;
            }

            return OtherItemTemplate;
        }
    }
}
