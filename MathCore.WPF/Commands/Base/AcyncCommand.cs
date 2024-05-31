using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMemberInSuper.Global


namespace MathCore.WPF.Commands;

[Copyright("Шаблоны для асинхронных MVVM-приложений: команды", url = "http://www.oszone.net/24584/")]
public interface IAsyncTaskCommand : ICommand
{
    Task ExecuteTaskAsync(object? parameter);
}

[Copyright("Шаблоны для асинхронных MVVM-приложений: команды", url = "http://www.oszone.net/24584/")]
public abstract class AsyncTaskCommandBase : IAsyncTaskCommand
{
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    protected static void Invoke_OnCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();

    public abstract bool CanExecute(object? parameter);

    public abstract Task ExecuteTaskAsync(object? parameter);

    public async void Execute(object? parameter) => await ExecuteTaskAsync(parameter);
}

/// <summary>Асинхронная команда</summary>
/// <example>
/// Url = "http://www.example.com/";
/// CountUrlBytesCommand = new AsyncCommand(async () => { ByteCount = await MyService.DownloadAndCountBytesAsync(Url); });
/// CountUrlBytesCommand = new AsyncCommand(MyService.DownloadAndCountBytesAsync(Url));
/// </example>
[Copyright("Шаблоны для асинхронных MVVM-приложений: команды", url = "http://www.oszone.net/24584/")]
public class AsyncTaskCommand<TResult>(Func<object?, CancellationToken, Task<TResult>> TaskFunction, Func<object?, bool>? CanExecute = null) : AsyncTaskCommandBase, INotifyPropertyChanged
{
    public AsyncTaskCommand(Task<TResult> task) : this(async (_, _) => await task) { }

    private sealed class CancelAsyncCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        private CancellationTokenSource _CancellationToken = new();
        private bool _CommandExecuting;
        public CancellationToken Token => _CancellationToken.Token;

        public void NotifyCommandStarting()
        {
            _CommandExecuting = true;
            if (!_CancellationToken.IsCancellationRequested)
                return;
            _CancellationToken = new();
            Invoke_OnCanExecuteChanged();
        }

        public void NotifyCommandFinished()
        {
            _CommandExecuting = false;
            Invoke_OnCanExecuteChanged();
        }

        bool ICommand.CanExecute(object? parameter) => _CommandExecuting && !_CancellationToken.IsCancellationRequested;

        void ICommand.Execute(object? parameter)
        {
            _CancellationToken.Cancel();
            Invoke_OnCanExecuteChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null!) => PropertyChanged?.Invoke(this, new(PropertyName));

    private readonly Func<object?, bool>? _CanExecute = CanExecute;

    private NotifyTaskCompletion<TResult>? _Execution;
    public NotifyTaskCompletion<TResult>? Execution
    {
        get => _Execution;
        private set
        {
            if (ReferenceEquals(_Execution, value)) return;
            _Execution = value;
            OnPropertyChanged();
        }
    }

    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    private CancelAsyncCommand CancelCommand { get; } = new();

    public override bool CanExecute(object? parameter) => (_CanExecute?.Invoke(parameter) ?? true) && (Execution?.IsCompleted ?? false);

    public override async Task ExecuteTaskAsync(object? parameter)
    {
        CancelCommand.NotifyCommandStarting();
        Execution = new(TaskFunction(parameter, CancelCommand.Token));
        Invoke_OnCanExecuteChanged();
        CancelCommand.NotifyCommandFinished();
        await Execution.Task;
        Invoke_OnCanExecuteChanged();
    }
}