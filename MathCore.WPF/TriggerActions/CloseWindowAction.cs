using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.TriggerActions
{
    public class CloseWindowAction : TriggerAction<DependencyObject>
    {
        protected override void Invoke(object parameter) => (AssociatedObject?.FindVisualRoot() as Window)?.Close();
    }
}
