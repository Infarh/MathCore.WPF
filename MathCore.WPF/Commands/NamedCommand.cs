using System.Windows.Input;

namespace MathCore.WPF.Commands;

public class NamedCommand : Command
{
    private readonly ICommand _BaseCommand;
    private string _Name;
    private string? _Description;

    public ICommand BaseCommand => _BaseCommand;

    public string Name { get => _Name; set => Set(ref _Name, value); }
    public string? Description { get => _Description; set => Set(ref _Description, value); }

    public override bool IsCanExecute
    {
        get
        {
            if (_BaseCommand is Command command)
                return command.IsCanExecute;
            return base.IsCanExecute;
        }
        set
        {
            if (_BaseCommand is Command command)
                command.IsCanExecute = value;
            base.IsCanExecute = value;
        }
    }

    public NamedCommand(ICommand BaseCommand) => _BaseCommand = BaseCommand;

    public NamedCommand(ICommand BaseCommand, string Name, string? Description) : this(BaseCommand)
    {
        _Name = Name;
        _Description = Description;
    }

    public override void Execute(object? parameter) => _BaseCommand.Execute(parameter);

    public override bool CanExecute(object? parameter) => base.CanExecute(parameter) && _BaseCommand.CanExecute(parameter);

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        (_BaseCommand as IDisposable)?.Dispose();
    }
}
