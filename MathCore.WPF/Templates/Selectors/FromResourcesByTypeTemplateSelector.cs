using System.Windows;
using System.Windows.Controls;

namespace MathCore.WPF.Templates.Selectors
{
    public class FromResourcesByTypeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            if(element == null || item == null) return null;
            var type = item.GetType();
            return element.FindResource(type.Name) as DataTemplate;
        }
    }
}