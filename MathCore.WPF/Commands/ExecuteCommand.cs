using System;
using System.Diagnostics;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Commands
{
    public class ExecuteCommand : LambdaCommand
    {
        public string? Command { get; set; }
        public bool ShellExecute { get; set; }

        public ExecuteCommand() { }

        public ExecuteCommand(string Command) => this.Command = Command;

        public ExecuteCommand(string Command, bool ShellExecute) : this(Command) => this.ShellExecute = ShellExecute;

        public override bool CanExecute(object p) => !string.IsNullOrWhiteSpace(p as string) || !string.IsNullOrWhiteSpace(Command);

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
                if(p != null)
                    info.Arguments = p.ToString();
            }
            info.UseShellExecute = ShellExecute;
            Process.Start(info);
        }
    }
}