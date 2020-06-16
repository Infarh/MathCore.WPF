using System.Windows;
using System.Windows.Controls;

namespace MathCore.WPF.Styles.Selectors
{
    public class FirstLastItemsSelector : StyleSelector
    {
        public Style? FirstItemStyle { get; set; }

        public Style? LastItemStyle { get; set; }

        public Style? OtherElementsStyle { get; set; }

        public override Style? SelectStyle(object _, DependencyObject item)
        {
            if ((item is ContentControl || item is ContentPresenter) && item.FindVisualParent<ItemsControl>() is {} list)
            {
                var index = list.ItemContainerGenerator.IndexFromContainer(item);
                if (index == 0) return FirstItemStyle;
                if (index == list.Items.Count - 1) return LastItemStyle;
            }

            return OtherElementsStyle;
        }
    }
}
