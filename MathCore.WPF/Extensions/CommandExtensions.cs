using System.Windows.Input;

using MathCore.WPF.Commands;

namespace MathCore.WPF.Extensions;

public static class CommandExtensions
{
    public static TCommand WithName<TCommand>(this TCommand Command, string Name, string? Description = null)
        where TCommand : Command
    {
        Command.Name = Name;
        if (Description is not null)
            Command.Description = Description;
        return Command;
    }

    public static bool TryExecute(this ICommand? Command, object Parameter)
    {
        if (Command is null || !Command.CanExecute(Parameter)) return false;

        Command.Execute(Parameter);

        return true;
    }
}
