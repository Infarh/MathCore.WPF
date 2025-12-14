using System.Windows.Input;
using System.Windows.Markup;

using MathCore.WPF.Commands;
using MathCore.WPF.Extensions;
using MathCore.WPF.Notifications;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

[MarkupExtensionReturnType(typeof(TestToastNotifyViewModel))]
internal class TestToastNotifyViewModel : ViewModel
{
    /// <summary>Заголовок уведомления</summary>
    public string? Title { get; set => Set(ref field, value); } = "Тест уведомления";

    /// <summary>Сообщение уведомления</summary>
    public string? Message { get; set => Set(ref field, value); } = "Сообщение 1";

    #region SelectedIcon : ToastNotificationIcon - Выбранный тип иконки

    /// <summary>Выбранный тип иконки</summary>
    private ToastNotificationIcon _SelectedIcon = ToastNotificationIcon.Information;

    /// <summary>Выбранный тип иконки</summary>
    public ToastNotificationIcon SelectedIcon { get => _SelectedIcon; set => Set(ref _SelectedIcon, value); }

    #endregion

    #region SelectedPosition : ToastNotificationPosition - Выбранная позиция

    /// <summary>Выбранная позиция</summary>
    private ToastNotificationPosition _SelectedPosition = ToastNotificationPosition.BottomRight;

    /// <summary>Выбранная позиция</summary>
    public ToastNotificationPosition SelectedPosition 
    { 
        get => _SelectedPosition; 
        set 
        {
            if (Set(ref _SelectedPosition, value))
                UpdateManagerSettings();
        } 
    }

    #endregion

    #region DisplayDuration : int - Длительность отображения (мс)

    /// <summary>Длительность отображения (мс)</summary>
    private int _DisplayDuration = 5000;

    /// <summary>Длительность отображения (мс)</summary>
    public int DisplayDuration 
    { 
        get => _DisplayDuration; 
        set 
        {
            if (Set(ref _DisplayDuration, value))
                UpdateManagerSettings();
        } 
    }

    #endregion

    /// <summary>Доступные типы иконок</summary>
    public IEnumerable<ToastNotificationIcon> AvailableIcons { get; } = 
        Enum.GetValues(typeof(ToastNotificationIcon)).Cast<ToastNotificationIcon>();

    /// <summary>Доступные позиции</summary>
    public IEnumerable<ToastNotificationPosition> AvailablePositions { get; } = 
        Enum.GetValues(typeof(ToastNotificationPosition)).Cast<ToastNotificationPosition>();

    private readonly ToastNotificationManager _NotificationManager;

    public TestToastNotifyViewModel()
    {
        // Создаём индивидуальный менеджер для демонстрации настройки
        _NotificationManager = new ToastNotificationManager(new ToastNotificationSettings
        {
            Position = SelectedPosition,
            DisplayDuration = DisplayDuration,
            MaxVisibleNotifications = 5
        });
    }

    private void UpdateManagerSettings()
    {
        _NotificationManager.Settings.Position = SelectedPosition;
        _NotificationManager.Settings.DisplayDuration = DisplayDuration;
    }

    #region Command ShowNotificationCommand - Показать уведомление

    /// <summary>Показать уведомление</summary>
    private ICommand? _ShowNotificationCommand;

    /// <summary>Показать уведомление</summary>
    public ICommand ShowNotificationCommand => _ShowNotificationCommand ??= Command.New(
        OnShowNotificationCommandExecuted,
        CanShowNotificationCommandExecute)
        .WithName("Показать извещение")
        .WithDescription("Отображает всплывающее сообщение на экране");

    /// <summary>Проверка возможности выполнения - Показать уведомление</summary>
    private bool CanShowNotificationCommandExecute() => Message is { Length: > 0 };

    /// <summary>Логика выполнения - Показать уведомление</summary>
    private void OnShowNotificationCommandExecuted()
    {
        _NotificationManager.Show(Title, Message!, SelectedIcon);
    }

    #endregion

    #region Command ShowInformationCommand - Показать информационное уведомление

    /// <summary>Показать информационное уведомление</summary>
    private ICommand? _ShowInformationCommand;

    /// <summary>Показать информационное уведомление</summary>
    public ICommand ShowInformationCommand => _ShowInformationCommand ??= Command.New(
        () => _NotificationManager.Show("Информация", "Это информационное уведомление", ToastNotificationIcon.Information))
        .WithDescription("Показывает информационное уведомление");

    #endregion

    #region Command ShowSuccessCommand - Показать уведомление об успехе

    /// <summary>Показать уведомление об успехе</summary>
    private ICommand? _ShowSuccessCommand;

    /// <summary>Показать уведомление об успехе</summary>
    public ICommand ShowSuccessCommand => _ShowSuccessCommand ??= Command.New(
        () => _NotificationManager.Show("Успешно", "Операция выполнена успешно!", ToastNotificationIcon.Success))
        .WithDescription("Показывает уведомление об успехе");

    #endregion

    #region Command ShowWarningCommand - Показать предупреждение

    /// <summary>Показать предупреждение</summary>
    private ICommand? _ShowWarningCommand;

    /// <summary>Показать предупреждение</summary>
    public ICommand ShowWarningCommand => _ShowWarningCommand ??= Command.New(
        () => _NotificationManager.Show("Внимание", "Это предупреждение о возможной проблеме", ToastNotificationIcon.Warning))
        .WithDescription("Показывает предупреждение");

    #endregion

    #region Command ShowErrorCommand - Показать ошибку

    /// <summary>Показать ошибку</summary>
    private ICommand? _ShowErrorCommand;

    /// <summary>Показать ошибку</summary>
    public ICommand ShowErrorCommand => _ShowErrorCommand ??= Command.New(
        () => _NotificationManager.Show("Ошибка", "Произошла ошибка при выполнении операции", ToastNotificationIcon.Error))
        .WithDescription("Показывает сообщение об ошибке");

    #endregion

    #region Command ShowCriticalCommand - Показать критическую ошибку

    /// <summary>Показать критическую ошибку</summary>
    private ICommand? _ShowCriticalCommand;

    /// <summary>Показать критическую ошибку</summary>
    public ICommand ShowCriticalCommand => _ShowCriticalCommand ??= Command.New(
        () => _NotificationManager.Show("Критическая ошибка", "Критическая ошибка системы!", ToastNotificationIcon.Critical))
        .WithDescription("Показывает критическую ошибку");

    #endregion

    #region Command ShowMultipleCommand - Показать несколько уведомлений

    /// <summary>Показать несколько уведомлений</summary>
    private ICommand? _ShowMultipleCommand;

    /// <summary>Показать несколько уведомлений</summary>
    public ICommand ShowMultipleCommand => _ShowMultipleCommand ??= Command.New(
        OnShowMultipleCommandExecuted)
        .WithDescription("Показывает несколько уведомлений подряд");

    /// <summary>Логика выполнения - Показать несколько уведомлений</summary>
    private void OnShowMultipleCommandExecuted()
    {
        _NotificationManager.Show("Уведомление 1", "Первое уведомление", ToastNotificationIcon.Information);
        _NotificationManager.Show("Уведомление 2", "Второе уведомление", ToastNotificationIcon.Success);
        _NotificationManager.Show("Уведомление 3", "Третье уведомление", ToastNotificationIcon.Warning);
        _NotificationManager.Show("Уведомление 4", "Четвёртое уведомление", ToastNotificationIcon.Error);
    }

    #endregion

    #region Command ShowWithCustomContentCommand - Показать с пользовательским содержимым

    /// <summary>Показать с пользовательским содержимым</summary>
    private ICommand? _ShowWithCustomContentCommand;

    /// <summary>Показать с пользовательским содержимым</summary>
    public ICommand ShowWithCustomContentCommand => _ShowWithCustomContentCommand ??= Command.New(
        OnShowWithCustomContentCommandExecuted)
        .WithDescription("Показывает уведомление с пользовательским содержимым");

    /// <summary>Логика выполнения - Показать с пользовательским содержимым</summary>
    private void OnShowWithCustomContentCommandExecuted()
    {
        var custom_content = new System.Windows.Controls.StackPanel
        {
            Children =
            {
                new System.Windows.Controls.TextBlock { Text = "Пользовательское содержимое:", FontWeight = System.Windows.FontWeights.Bold },
                new System.Windows.Controls.ProgressBar { Height = 20, IsIndeterminate = true, Margin = new System.Windows.Thickness(0, 5, 0, 0) }
            }
        };

        _NotificationManager.ShowCustom("Загрузка", custom_content, ToastNotificationIcon.None);
    }

    #endregion

    #region Command CloseAllCommand - Закрыть все уведомления

    /// <summary>Закрыть все уведомления</summary>
    private ICommand? _CloseAllCommand;

    /// <summary>Закрыть все уведомления</summary>
    public ICommand CloseAllCommand => _CloseAllCommand ??= Command.New(
        () => _NotificationManager.CloseAll())
        .WithDescription("Закрывает все активные уведомления");

    #endregion
}
