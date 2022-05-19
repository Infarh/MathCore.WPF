using System.Windows.Input;

using MathCore.WPF.Commands;

namespace MathCore.WPF.Extensions;

public static class CommandExtensions
{
    public static NamedCommand WithName(this ICommand Command, string Name, string Description = null)
    {
        if (Command is NamedCommand named_command)
        {
            named_command.Name = Name;
            named_command.Description = Description;
            return named_command;
        }

        return new(Command, Name, Description);
    }
}
