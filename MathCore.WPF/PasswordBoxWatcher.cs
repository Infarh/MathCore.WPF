using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MathCore.WPF;

public class PasswordBoxWatcher : Decorator
{
    #region Password dependency property (Other : Пароль) : string

    /// <summary>Пароль</summary>
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register(
            nameof(Password),
            typeof(string),
            typeof(PasswordBoxWatcher),
            new FrameworkPropertyMetadata(default(string),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnPasswordChanged));

    /// <summary>Пароль</summary>
    [Category("Other")]
    [Description("Пароль")]
    public string Password { get => (string)GetValue(PasswordProperty); set => SetValue(PasswordProperty, value); }

    private static void OnPasswordChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        var password_watcher = (PasswordBoxWatcher)D;
        var password_box = (PasswordBox)password_watcher.Child;

        if (password_watcher._IsPreventCallback) return;

        password_box.PasswordChanged -= password_watcher._OnHandlePasswordChangedDelegate;
        password_box.Password = E.NewValue?.ToString() ?? "";
        password_box.PasswordChanged += password_watcher._OnHandlePasswordChangedDelegate;
    }

    #endregion

    private bool _IsPreventCallback;
    private readonly RoutedEventHandler _OnHandlePasswordChangedDelegate;

    public PasswordBoxWatcher()
    {
        _OnHandlePasswordChangedDelegate = OnHandlePasswordChanged;

        var password_box = new PasswordBox();
        password_box.PasswordChanged += _OnHandlePasswordChangedDelegate;
        Child = password_box;
    }

    /// <summary>Обработчик события изменения пароля</summary>
    /// <param name="sender">Источник события - должен быть полем ввода пароля</param>
    /// <param name="e">Аргумент события</param>
    private void OnHandlePasswordChanged(object sender, RoutedEventArgs e)
    {
        _IsPreventCallback = true;
        Password = ((PasswordBox)sender).Password;
        _IsPreventCallback = false;
    }
}