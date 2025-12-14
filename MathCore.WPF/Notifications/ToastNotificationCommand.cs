using System.Windows.Input;
using MathCore.WPF.Commands;

namespace MathCore.WPF.Notifications;

/// <summary>Команда для показа всплывающего уведомления</summary>
public class ToastNotificationCommand : Command
{
    private readonly ToastNotificationManager _Manager;

    /// <summary>Инициализация команды с менеджером по умолчанию</summary>
    public ToastNotificationCommand() : this(ToastNotificationManager.Default) { }

    /// <summary>Инициализация команды с указанным менеджером</summary>
    /// <param name="Manager">Менеджер уведомлений</param>
    public ToastNotificationCommand(ToastNotificationManager Manager) => 
        _Manager = Manager ?? throw new ArgumentNullException(nameof(Manager));

    /// <summary>Заголовок уведомления</summary>
    public string? Title { get; set; }

    /// <summary>Текст сообщения</summary>
    public string? Message { get; set; }

    /// <summary>Тип иконки</summary>
    public ToastNotificationIcon Icon { get; set; } = ToastNotificationIcon.Information;

    public override bool CanExecute(object? Parameter) => true;

    public override void Execute(object? Parameter)
    {
        var message = Parameter as string ?? Message;

        if (string.IsNullOrWhiteSpace(message))
            return;

        _Manager.Show(Title, message, Icon);
    }
}

/// <summary>Команда для показа всплывающего информационного уведомления</summary>
public class ShowInformationToastCommand : ToastNotificationCommand
{
    public ShowInformationToastCommand()
    {
        Icon = ToastNotificationIcon.Information;
        Title = "Информация";
    }
}

/// <summary>Команда для показа всплывающего уведомления об успехе</summary>
public class ShowSuccessToastCommand : ToastNotificationCommand
{
    public ShowSuccessToastCommand()
    {
        Icon = ToastNotificationIcon.Success;
        Title = "Успех";
    }
}

/// <summary>Команда для показа всплывающего уведомления с предупреждением</summary>
public class ShowWarningToastCommand : ToastNotificationCommand
{
    public ShowWarningToastCommand()
    {
        Icon = ToastNotificationIcon.Warning;
        Title = "Предупреждение";
    }
}

/// <summary>Команда для показа всплывающего уведомления об ошибке</summary>
public class ShowErrorToastCommand : ToastNotificationCommand
{
    public ShowErrorToastCommand()
    {
        Icon = ToastNotificationIcon.Error;
        Title = "Ошибка";
    }
}
