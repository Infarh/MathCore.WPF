using System.ComponentModel;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для привязки пароля из PasswordBox к SecureString.</summary>
public class PasswordBoxBinder : Behavior<PasswordBox>
{
    #region Password : SecureString - Пароль

    /// <summary>DependencyProperty для свойства Password.</summary>
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register(
            nameof(Password),
            typeof(SecureString),
            typeof(PasswordBoxBinder),
            new(default(SecureString)));

    /// <summary>Пароль, введенный пользователем.</summary>
    /// <remarks>Привязывается к свойству SecurePassword элемента PasswordBox.</remarks>
    [Description("Пароль")]
    public SecureString Password
    {
        get => (SecureString)GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    #endregion

    /// <summary>Вызывается при присоединении поведения к элементу PasswordBox.</summary>
    protected override void OnAttached()
    {
        // Подписываемся на событие изменения пароля в PasswordBox.
        AssociatedObject.PasswordChanged += OnPasswordChanged;
    }

    /// <summary>Вызывается при отсоединении поведения от элемента PasswordBox.</summary>
    protected override void OnDetaching()
    {
        // Отменяем подписку на событие изменения пароля в PasswordBox.
        AssociatedObject.PasswordChanged -= OnPasswordChanged;
    }

    /// <summary>Обработчик события изменения пароля в PasswordBox.</summary>
    /// <param name="Sender">Источник события.</param>
    /// <param name="E">Аргументы события.</param>
    private void OnPasswordChanged(object Sender, RoutedEventArgs E)
    {
        // Обновляем свойство Password при изменении пароля в PasswordBox.
        Password = ((PasswordBox)Sender).SecurePassword;
    }
}