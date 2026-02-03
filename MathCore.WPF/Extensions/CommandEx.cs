using System.Windows.Input;

using MathCore.WPF.Commands;

namespace MathCore.WPF.Extensions;

/// <summary>Статический класс-расширение для работы с командами WPF</summary>
public static class CommandEx
{
    /// <summary>Устанавливает имя и описание для команды</summary>
    /// <typeparam name="TCommand">Тип команды</typeparam>
    /// <param name="Command">Команда</param>
    /// <param name="Name">Имя команды</param>
    /// <param name="Description">Описание команды</param>
    /// <returns>Команда с установленными именем и описанием</returns>
    public static TCommand WithName<TCommand>(this TCommand Command, string Name, string? Description = null)
        where TCommand : Command
    {
        Command.Name = Name;
        if (Description is not null)
            Command.Description = Description;
        return Command;
    }

    /// <summary>Устанавливает описание для команды</summary>
    /// <typeparam name="TCommand">Тип команды</typeparam>
    /// <param name="Command">Команда</param>
    /// <param name="Description">Описание команды</param>
    /// <returns>Команда с установленными описанием</returns>
    public static TCommand WithDescription<TCommand>(this TCommand Command, string? Description)
        where TCommand : Command
    {
        Command.Description = Description;
        return Command;
    }

    /// <summary>Пытается выполнить команду с указанным параметром</summary>
    /// <param name="Command">Команда</param>
    /// <param name="Parameter">Параметр команды</param>
    /// <returns>Истина, если команда была успешно выполнена</returns>
    public static bool TryExecute(this ICommand? Command, object? Parameter = null)
    {
        if (Command is null || !Command.CanExecute(Parameter)) return false;

        Command.Execute(Parameter);

        return true;
    }
}
