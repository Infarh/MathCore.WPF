using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MathCore.WPF;

public static class TextBoxEx
{
    #region Attached property UpdateBindingSourceOnEnter : bool - Обновить привязку при нажатии Enter

    /// <summary>Обновить привязку при нажатии Enter</summary>
    public static readonly DependencyProperty UpdateBindingSourceOnEnterProperty =
        DependencyProperty.RegisterAttached(
            "UpdateBindingSourceOnEnter",
            typeof(bool),
            typeof(TextBoxEx),
            new(default(bool), OnUpdateBindingOnEnterChanged));

    private static void OnUpdateBindingOnEnterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if(sender is not TextBox text_box || e.NewValue is not bool value) return;

        if (value)
            text_box.KeyDown += OnTextBoxKeyDown;
        else
            text_box.KeyDown -= OnTextBoxKeyDown;
    }

    private static void OnTextBoxKeyDown(object Sender, KeyEventArgs E)
    {
        if(E.Key != Key.Enter) return;

        var text_box = (TextBox)Sender;

        if (text_box.AcceptsReturn && !E.KeyboardDevice.IsKeyDown(Key.LeftCtrl)) 
            return;

        if (BindingOperations.GetBindingExpression(text_box, TextBox.TextProperty) is not { } binding) 
            return;

        if(binding.ParentBinding.UpdateSourceTrigger != UpdateSourceTrigger.Explicit && !E.KeyboardDevice.IsKeyDown(Key.LeftCtrl))
            return;

        binding.UpdateSource();
        E.Handled = true;
    }

    /// <summary>Обновить привязку при нажатии Enter</summary>
    [AttachedPropertyBrowsableForType(typeof(TextBox))]
    public static void SetUpdateBindingSourceOnEnter(DependencyObject d, bool value) => d.SetValue(UpdateBindingSourceOnEnterProperty, value);

    /// <summary>Обновить привязку при нажатии Enter</summary>
    public static bool GetUpdateBindingSourceOnEnter(DependencyObject d) => (bool)d.GetValue(UpdateBindingSourceOnEnterProperty);

    #endregion
}
