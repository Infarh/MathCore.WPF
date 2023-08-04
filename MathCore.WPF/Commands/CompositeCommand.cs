using System.Windows.Input;

namespace MathCore.WPF.Commands;

public class CompositeCommand(params ICommand[] Commands) : Command
{
    private readonly ICommand[] _Commands = Commands.SelectMany(GetCommands).ToArray();

    private static IEnumerable<ICommand> GetCommands(ICommand command) => command switch
    {
        CompositeCommand composite => composite._Commands.SelectMany(GetCommands),
        _ => Enumerable.Repeat(command, 1)
    };

    public override bool CanExecute(object? parameter)
    {
        foreach (var command in _Commands)
            if (!command.CanExecute(parameter))
                return false;
        return true;
    }

    public override void Execute(object? parameter)
    {
        foreach (var command in _Commands)
            command.Execute(parameter);
    }
}
