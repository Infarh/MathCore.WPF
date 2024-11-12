using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MathCore.WPF;

/// <summary>Декоратор для отслеживания изменений пароля в PasswordBox.</summary>
public class PasswordBoxWatcher : Decorator
{
    #region Password dependency property (Other : Пароль) : string

    /// <summary>DependencyProperty для пароля.</summary>
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register(
            nameof(Password),
            typeof(string),
            typeof(PasswordBoxWatcher),
            new FrameworkPropertyMetadata(default(string),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnPasswordChanged));

    /// <summary>Пароль.</summary>
    [Category("Other")]
    [Description("Пароль")]
    public string Password { get => (string)GetValue(PasswordProperty); set => SetValue(PasswordProperty, value); }

    /// <summary>Обработчик изменения пароля.</summary>
    /// <param name="D">Источник события.</param>
    /// <param name="E">Аргумент события.</param>
    private static void OnPasswordChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        // Получаем экземпляр PasswordBoxWatcher и PasswordBox
        var password_watcher = (PasswordBoxWatcher)D;
        var password_box = (PasswordBox)password_watcher.Child;

        // Если callback уже запущен, выходим
        if (password_watcher._IsPreventCallback) return;

        // Удаляем обработчик PasswordChanged, чтобы избежать рекурсии
        password_box.PasswordChanged -= password_watcher._OnHandlePasswordChangedDelegate;

        // Устанавливаем новое значение пароля
        password_box.Password = E.NewValue?.ToString() ?? "";

        // Добавляем обратно обработчик PasswordChanged
        password_box.PasswordChanged += password_watcher._OnHandlePasswordChangedDelegate;
    }

    #endregion

    // Флаг, указывающий, что callback уже запущен
    private bool _IsPreventCallback;

    // Делегат для обработчика PasswordChanged
    private readonly RoutedEventHandler _OnHandlePasswordChangedDelegate;

    /// <summary>Конструктор PasswordBoxWatcher.</summary>
    public PasswordBoxWatcher()
    {
        // Инициализируем делегат для обработчика PasswordChanged
        _OnHandlePasswordChangedDelegate = OnHandlePasswordChanged;

        // Создаём PasswordBox и добавляем обработчик PasswordChanged
        var password_box = new PasswordBox();
        password_box.PasswordChanged += _OnHandlePasswordChangedDelegate;
        Child = password_box;
    }

    /// <summary>Обработчик события изменения пароля.</summary>
    /// <param name="sender">Источник события - должен быть полем ввода пароля.</param>
    /// <param name="e">Аргумент события.</param>
    private void OnHandlePasswordChanged(object sender, RoutedEventArgs e)
    {
        // Устанавливаем флаг, чтобы избежать рекурсии
        _IsPreventCallback = true;

        // Обновляем DependencyProperty пароля
        Password = ((PasswordBox)sender).Password;

        // Сбрасываем флаг
        _IsPreventCallback = false;
    }
}