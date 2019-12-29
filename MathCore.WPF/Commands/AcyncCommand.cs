using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMemberInSuper.Global


namespace MathCore.WPF.Commands
{
    [Copyright("Шаблоны для асинхронных MVVM-приложений: команды", url = "http://www.oszone.net/24584/")]
    public interface IAsyncTaskCommand : ICommand
    {
        [NotNull]
        Task ExecuteTaskAsync([CanBeNull]object parameter);
    }

    [Copyright("Шаблоны для асинхронных MVVM-приложений: команды", url = "http://www.oszone.net/24584/")]
    public abstract class AsyncTaskCommandBase : IAsyncTaskCommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        protected static void Invoke_OnCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();

        public abstract bool CanExecute([CanBeNull]object parameter);

        public abstract Task ExecuteTaskAsync(object parameter);

        public async void Execute([CanBeNull]object parameter) => await ExecuteTaskAsync(parameter);
    }

    /// <summary>Асинхронная команда</summary>
    /// <example>
    /// Url = "http://www.example.com/";
    /// CountUrlBytesCommand = new AsyncCommand(async () => { ByteCount = await MyService.DownloadAndCountBytesAsync(Url); });
    /// CountUrlBytesCommand = new AsyncCommand(MyService.DownloadAndCountBytesAsync(Url));
    /// </example>
    [Copyright("Шаблоны для асинхронных MVVM-приложений: команды", url = "http://www.oszone.net/24584/")]
    public class AsyncTaskCommand<TResult> : AsyncTaskCommandBase, INotifyPropertyChanged
    {
        private sealed class CancelAsyncCommand : ICommand
        {
            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            private CancellationTokenSource _CancellationToken = new CancellationTokenSource();
            private bool _CommandExecuting;
            public CancellationToken Token => _CancellationToken.Token;

            public void NotifyCommandStarting()
            {
                _CommandExecuting = true;
                if (!_CancellationToken.IsCancellationRequested)
                    return;
                _CancellationToken = new CancellationTokenSource();
                Invoke_OnCanExecuteChanged();
            }

            public void NotifyCommandFinished()
            {
                _CommandExecuting = false;
                Invoke_OnCanExecuteChanged();
            }

            bool ICommand.CanExecute([CanBeNull]object parameter) => _CommandExecuting && !_CancellationToken.IsCancellationRequested;

            void ICommand.Execute([CanBeNull]object parameter)
            {
                _CancellationToken.Cancel();
                Invoke_OnCanExecuteChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        [NotNull]
        private readonly Func<object, CancellationToken, Task<TResult>> _TaskFunction;

        private readonly Func<object, bool>? _CanExecute;

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

        [NotNull]
        // ReSharper disable once ConvertToAutoPropertyWhenPossible
        private CancelAsyncCommand CancelCommand { get; }

        public AsyncTaskCommand([NotNull]Func<object, CancellationToken, Task<TResult>> TaskFunction, Func<object, bool>? CanExecute = null)
        {
            _TaskFunction = TaskFunction;
            _CanExecute = CanExecute;
            CancelCommand = new CancelAsyncCommand();
        }

        public AsyncTaskCommand([NotNull]Task<TResult> task) : this(async (p, c) => await task) { }

        public override bool CanExecute(object parameter) => (_CanExecute?.Invoke(parameter) ?? true) && (Execution?.IsCompleted ?? false);

        public override async Task ExecuteTaskAsync(object parameter)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            CancelCommand.NotifyCommandStarting();
            Execution = new NotifyTaskCompletion<TResult>(_TaskFunction(parameter, CancelCommand.Token));
            Invoke_OnCanExecuteChanged();
            CancelCommand.NotifyCommandFinished();
            await Execution.Task;
            Invoke_OnCanExecuteChanged();
        }
    }

    public class AsyncLambdaCommand : LambdaCommand
    {
        public AsyncLambdaCommand([NotNull]Action<object?> ExecuteAction, Func<object?, bool>? CanExecute = null) : base(ExecuteAction, CanExecute) { }
        public AsyncLambdaCommand([NotNull]Action<object?> ExecuteAction, Func<bool>? CanExecute = null) : base(ExecuteAction, CanExecute) { }
        public AsyncLambdaCommand([NotNull]Action ExecuteAction, Func<object?, bool>? CanExecute = null) : base(ExecuteAction, CanExecute) { }
        public AsyncLambdaCommand([NotNull]Action ExecuteAction, Func<bool>? CanExecute = null) : base(ExecuteAction, CanExecute) { }

        public virtual Task ExecuteAsync([CanBeNull]object parameter) => Task.Factory.StartNew(base.Execute, parameter);

        public override async void Execute(object? parameter) => await ExecuteAsync(parameter);
    }
}