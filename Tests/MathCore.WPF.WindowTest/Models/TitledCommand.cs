using System.Diagnostics;
using System.Windows.Input;

namespace MathCore.WPF.WindowTest.Models;

class TitledCommand : ICommand
{
    private readonly Action _Execute;

    public string? Title { get; set; }

    public bool CanExecute { get; set; } = true;

    public TitledCommand(Action Execute) => _Execute = Execute;
        
    bool ICommand.CanExecute(object? parameter)
    {
        Debug.WriteLine($"{Title}-{CanExecute}");
        return CanExecute;
    }

    void ICommand.Execute(object? parameter) => _Execute();

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}