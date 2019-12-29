using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Behaviors
{
    public class TextBoxEndCaretAtGotFocus : Behavior<TextBox>
    {
        protected override void OnAttached() => AssociatedObject.GotFocus += OnTextBoxGotFocus;

        protected override void OnDetaching() => AssociatedObject.GotFocus -= OnTextBoxGotFocus;

        private static void OnTextBoxGotFocus(object Sender, RoutedEventArgs E)
        {
            if (!(Sender is TextBox edit)) return;
            edit.CaretIndex = edit.Text.Length;
        }
    }
}
