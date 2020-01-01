using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Behaviors
{
    public class CloseButtonBehavior : Behavior<Button>
    {
        protected override void OnAttached() => AssociatedObject.Click += OnClick;

        protected override void OnDetaching() => AssociatedObject.Click -= OnClick;

        private static void OnClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    }
}