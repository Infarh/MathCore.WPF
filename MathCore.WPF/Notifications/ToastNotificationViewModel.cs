using System.Windows.Input;
using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF.Notifications;

/// <summary>Модель-представление уведомления</summary>
public class ToastNotificationViewModel : ViewModel
{
    /// <summary>Событие запроса закрытия уведомления</summary>
    public event EventHandler? CloseRequested;

    /// <summary>Событие клика левой кнопкой мыши по уведомлению</summary>
    public event EventHandler? LeftClick;

    /// <summary>Событие клика правой кнопкой мыши по уведомлению</summary>
    public event EventHandler? RightClick;

    #region Title : string - Заголовок уведомления

    /// <summary>Заголовок уведомления</summary>
    private string? _Title;

    /// <summary>Заголовок уведомления</summary>
    public string? Title { get => _Title; set => Set(ref _Title, value); }

    #endregion

    #region Message : string - Текст сообщения

    /// <summary>Текст сообщения</summary>
    private string? _Message;

    /// <summary>Текст сообщения</summary>
    public string? Message { get => _Message; set => Set(ref _Message, value); }

    #endregion

    #region Icon : ToastNotificationIcon - Тип иконки

    /// <summary>Тип иконки</summary>
    private ToastNotificationIcon _Icon = ToastNotificationIcon.Information;

    /// <summary>Тип иконки</summary>
    public ToastNotificationIcon Icon { get => _Icon; set => Set(ref _Icon, value); }

    #endregion

    #region CustomContent : object - Пользовательское содержимое

    /// <summary>Пользовательское содержимое</summary>
    private object? _CustomContent;

    /// <summary>Пользовательское содержимое</summary>
    public object? CustomContent { get => _CustomContent; set => Set(ref _CustomContent, value); }

    #endregion

    #region Command CloseCommand - Команда закрытия уведомления

    /// <summary>Команда закрытия уведомления</summary>
    private ICommand? _CloseCommand;

    /// <summary>Команда закрытия уведомления</summary>
    public ICommand CloseCommand => _CloseCommand ??= Command.New(OnCloseCommandExecuted);

    /// <summary>Логика выполнения команды закрытия уведомления</summary>
    private void OnCloseCommandExecuted() => CloseRequested?.Invoke(this, EventArgs.Empty);

    #endregion

    /// <summary>Вызывает событие клика левой кнопкой</summary>
    public void OnLeftClick() => LeftClick?.Invoke(this, EventArgs.Empty);

    /// <summary>Вызывает событие клика правой кнопкой</summary>
    public void OnRightClick() => RightClick?.Invoke(this, EventArgs.Empty);
}
