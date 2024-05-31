using System.ComponentModel;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

public class PasswordBoxBinder : Behavior<PasswordBox>
{
    #region Password : SecureString - Пароль

    /// <summary>Пароль</summary>
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register(
            nameof(Password),
            typeof(SecureString),
            typeof(PasswordBoxBinder),
            new(default(SecureString)));

    /// <summary>Пароль</summary>
    //[Category("")]
    [Description("Пароль")]
    public SecureString Password { get => (SecureString)GetValue(PasswordProperty); set => SetValue(PasswordProperty, value); }

    #endregion

    protected override void OnAttached() => AssociatedObject.PasswordChanged += OnPasswordChanged;

    protected override void OnDetaching() => AssociatedObject.PasswordChanged -= OnPasswordChanged;

    private void OnPasswordChanged(object Sender, RoutedEventArgs E) => Password = ((PasswordBox)Sender).SecurePassword;
}