using System.Windows;
using System.Windows.Controls;

namespace MathCore.WPF.Templates.Selectors
{
    public class FromResourcesByTypeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if(!(container is FrameworkElement element) || item is null) return null;
            var type = item.GetType();
            return element.FindResource(type.Name) as DataTemplate;
        }
    }
}