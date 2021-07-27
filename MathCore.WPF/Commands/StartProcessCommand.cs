using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Markup;

namespace MathCore.WPF.Commands
{
    [MarkupExtensionReturnType(typeof(StartProcessCommand))]
    public class StartProcessCommand : Command
    {
        private Process? _Process;

        public bool SingleProcess { get; set; }

        [ConstructorArgument(nameof(Path))]
        public string? Path { get; set; }

        [ConstructorArgument(nameof(CommandLineArgs))]
        public string? CommandLineArgs { get; set; }

        public bool ShellExecute { get; set; }

        public bool ParameterAsArgs { get; set; }

        #region ExitCode : int - Код результата операции

        /// <summary>Код результата операции</summary>
        private int? _ExitCode;

        /// <summary>Код результата операции</summary>
        public int? ExitCode { get => _ExitCode; private set => Set(ref _ExitCode, value); }

        #endregion

        public StartProcessCommand() { }
        public StartProcessCommand(string Path) => this.Path = Path;
        public StartProcessCommand(string Path, string CommandLineArgs) : this(Path) => this.CommandLineArgs = CommandLineArgs;

        public override bool CanExecute(object? parameter)
        {
            if (_Process is null or { HasExited: false }) return false;
            if (ParameterAsArgs) return Path is not null;
            return (parameter as string ?? Path) is not null;
        }

        public override void Execute(object? parameter)
        {
            var path = ParameterAsArgs ? Path : parameter as string ?? Path;
            if(path is not {Length: > 0}) return;
            
            var arg = ParameterAsArgs ? parameter as string ?? CommandLineArgs : CommandLineArgs;

            var info = new ProcessStartInfo(path);
            if (!string.IsNullOrWhiteSpace(arg))
                info.Arguments = arg;

            var process = Process.Start(info);
            if (process is null || !SingleProcess) return;

            process.Exited += OnProcessExited;
            process.EnableRaisingEvents = true;
            _Process = process;
        }

        private void OnProcessExited(object? Sender, EventArgs E)
        {
            if(Sender is not Process process) return;
            ExitCode = process.ExitCode;
            _Process = null;
            CommandManager.InvalidateRequerySuggested();
        }

        #region Command KillProcessCommand - Принудительное завершение процесса

        /// <summary>Принудительное завершение процесса</summary>
        private Command? _KillProcessCommand;

        /// <summary>Принудительное завершение процесса</summary>
        public ICommand KillProcessCommand => _KillProcessCommand ??= New(OnKillProcessCommandExecuted, CanKillProcessCommandExecute);

        /// <summary>Проверка возможности выполнения - Принудительное завершение процесса</summary>
        private bool CanKillProcessCommandExecute() => _Process != null;

        /// <summary>Логика выполнения - Принудительное завершение процесса</summary>
        private void OnKillProcessCommandExecuted() => _Process?.Kill();

        #endregion

        #region Command KillProcessTreeCommand - Принудительное завершение дерева процессов

        /// <summary>Принудительное завершение дерева процессов</summary>
        private Command? _KillProcessTreeCommand;

        /// <summary>Принудительное завершение дерева процессов</summary>
        public ICommand KillProcessTreeCommand => _KillProcessTreeCommand ??= New(OnKillProcessTreeCommandExecuted, CanKillProcessTreeCommandExecute);

        /// <summary>Проверка возможности выполнения - Принудительное завершение дерева процессов</summary>
        private bool CanKillProcessTreeCommandExecute() => _Process != null;

        /// <summary>Логика выполнения - Принудительное завершение дерева процессов</summary>
        private void OnKillProcessTreeCommandExecuted() => _Process?.Kill(true);

        #endregion

        #region Command CloseMainWindowCommand - Закрытие главного окна процесса

        /// <summary>Закрытие главного окна процесса</summary>
        private Command? _CloseMainWindowCommand;

        /// <summary>Закрытие главного окна процесса</summary>
        public ICommand CloseMainWindowCommand => _CloseMainWindowCommand ??= New(OnCloseMainWindowCommandExecuted, CanCloseMainWindowCommandExecute);

        /// <summary>Проверка возможности выполнения - Закрытие главного окна процесса</summary>
        private bool CanCloseMainWindowCommandExecute() => _Process != null;

        /// <summary>Логика выполнения - Закрытие главного окна процесса</summary>
        private void OnCloseMainWindowCommandExecuted() => _Process?.CloseMainWindow();

        #endregion
    }
}
