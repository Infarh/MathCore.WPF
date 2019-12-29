using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors
{
    public abstract class WindowBehavior : Behavior<UIElement>
    {
        private Window? _Window;

        public Window? AssociatedWindow => _Window;

        protected override void OnAttached() => _Window = AssociatedObject as Window ?? AssociatedObject.FindVisualParent<Window>();
    }
}