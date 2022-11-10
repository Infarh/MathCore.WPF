using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

public class TextBoxSelectAllAtGotFocus : Behavior<TextBox>
{
    protected override void OnAttached()
    {
        AssociatedObject.GotFocus    += OnTextBoxGotFocus;
        AssociatedObject.TextChanged += OnTextChanged;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.GotFocus    -= OnTextBoxGotFocus;
        AssociatedObject.TextChanged -= OnTextChanged;
    }

    private static void OnTextBoxGotFocus(object Sender, RoutedEventArgs E)
    {
        if (Sender is not TextBox { Text.Length: > 0 } text_box) return;
        text_box.SelectAll();
    }

    private static void OnTextChanged(object? Sender, EventArgs E)
    {
        if (Sender is not TextBox { Text.Length: > 0 } text_box) return;
        text_box.TextChanged -= OnTextChanged;
        text_box.SelectAll();
    }
}