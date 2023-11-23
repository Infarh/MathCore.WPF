using System.Diagnostics;
using System.Windows.Markup;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Commands;

/// <summary>Команда выполнения процесса в ОС, заданного командной строкой</summary>
public class ExecuteCommand(string Command) : LambdaCommand
{
    public ExecuteCommand() : this(null) { }

    public ExecuteCommand(string Command, bool ShellExecute) : this(Command) => this.ShellExecute = ShellExecute;

    /// <summary>Выполняемая командная строка</summary>
    [ConstructorArgument(nameof(Command))]
    public string? Command { get; set; } = Command;

    /// <summary>Использовать интерпретацию ОС</summary>
    [ConstructorArgument(nameof(ShellExecute))]
    public bool ShellExecute { get; set; }

    public override bool CanExecute(object? p) => !string.IsNullOrWhiteSpace(p as string) || !string.IsNullOrWhiteSpace(Command);

    public override void Execute(object? p)
    {
        ProcessStartInfo info;
        if(string.IsNullOrWhiteSpace(Command))
        {
            var cmd = p?.ToString();
            if(cmd is null)
                throw new ArgumentNullException(nameof(p),
                    @"Не задан параметр команды при отсутствии текста команды");
            info = new ProcessStartInfo(cmd);
        }
        else
        {
            info = new ProcessStartInfo(Command);
            if(p?.ToString() is { Length: > 0 } args)
                info.Arguments = args;
        }
        info.UseShellExecute = ShellExecute;
        Process.Start(info);
    }
}