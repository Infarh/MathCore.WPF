using System.Windows;
using System.Windows.Input;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF
{
    public static class UI
    {
        public static readonly DependencyProperty InputBindingProperty =
            DependencyProperty.RegisterAttached(
                "InputBinding",
                typeof(InputBinding),
                typeof(UI),
                new PropertyMetadata(default(InputBinding), OnInputBindingChanged));

        private static void OnInputBindingChanged(DependencyObject D, DependencyPropertyChangedEventArgs E) => ((UIElement)D).InputBindings.Add((InputBinding)E.NewValue);

        public static void SetInputBinding([NotNull] DependencyObject element, InputBinding value) => element.SetValue(InputBindingProperty, value);

        public static InputBinding GetInputBinding([NotNull] DependencyObject element) => (InputBinding)element.GetValue(InputBindingProperty);
    }
}