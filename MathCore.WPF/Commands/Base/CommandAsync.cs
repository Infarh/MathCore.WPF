using System.Windows.Input;

using MathCore.Annotations;

namespace MathCore.WPF.Commands;

/// <summary>Абстрактная асинхронная команда с поддержкой отмены и параллельного выполнения</summary>
[PublicAPI]
internal abstract class CommandAsync : Command
{
    /// <summary>Список всех активных CancellationTokenSource для отмены и освобождения</summary>
    private readonly List<CancellationTokenSource> _ActiveCancellations = [];

#if NET9_0_OR_GREATER
    /// <summary>Объект блокировки для синхронизации доступа к спискам</summary>
    private readonly Lock _ActiveCancellationsLock = new();
#else
    /// <summary>Объект блокировки для синхронизации доступа к спискам</summary>
    private readonly object _ActiveCancellationsLock = new();
#endif
    /// <summary>Список всех выполняющихся задач</summary>
    private readonly List<Task> _ActiveTasks = [];

    /// <summary>Выполнять команду в UI-контексте</summary>
    public bool ExecuteInUIContext { get; set => Set(ref field, value); } = true;

    /// <summary>Дождаться завершения всех выполняющихся задач перед запуском новой</summary>
    public bool WaitOne { get; set => Set(ref field, value); }

    /// <summary>Отменить все выполняющиеся задачи перед запуском новой</summary>
    public bool CancelOtherExecution { get; set => Set(ref field, value); }

    /// <summary>Признак длительного выполнения команды</summary>
    public bool LongRunning { get; set => Set(ref field, value); }

    /// <summary>Команда отмены всех выполняющихся задач</summary>
    private CancelExecutionCommand? _CancelCommand;

    /// <summary>Команда отмены всех выполняющихся задач</summary>
    public ICommand CancelCommand => _CancelCommand ??= new(this);

    /// <summary>Вспомогательная команда для отмены всех задач</summary>
    private sealed class CancelExecutionCommand(CommandAsync command) : ICommand
    {
        /// <inheritdoc />
        public event EventHandler? CanExecuteChanged;

        /// <summary>Вызвать событие изменения возможности выполнения</summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        /// <inheritdoc />
        public bool CanExecute(object? p)
        {
            lock (command._ActiveCancellationsLock)
                return command._ActiveCancellations.Any(c => !c.IsCancellationRequested);
        }

        /// <inheritdoc />
        public void Execute(object? p)
        {
            CancellationTokenSource[] snapshot;
            lock (command._ActiveCancellationsLock)
                snapshot = command._ActiveCancellations.ToArray();
            foreach (var c in snapshot) c.Cancel();
        }
    }

    /// <inheritdoc />
    public override bool CanExecute(object? parameter)
    {
        if (!base.CanExecute(parameter)) return false;
        lock (_ActiveCancellationsLock)
            return !WaitOne || _ActiveTasks.All(t => t.IsCompleted);
    }

    /// <inheritdoc />
    public override void Execute(object? parameter) => _ = ExecuteInternalAsync(parameter);

    /// <summary>Асинхронный запуск выполнения команды</summary>
    /// <param name="parameter">Параметр команды</param>
    private async Task ExecuteInternalAsync(object? parameter)
    {
        if (!ExecuteInUIContext || LongRunning)
            await Task.Yield().ConfigureAwait(false, LongRunning);

        // Отмена всех предыдущих задач, если требуется
        if (CancelOtherExecution)
        {
            CancellationTokenSource[] snapshot;
            lock (_ActiveCancellationsLock)
                snapshot = _ActiveCancellations.ToArray();
            foreach (var c in snapshot)
#if NET8_0_OR_GREATER
                await c.CancelAsync();
#else
                c.Cancel();
#endif
        }
        else if (WaitOne)
        {
            Task[] need_to_wait;
            lock (_ActiveCancellationsLock)
                need_to_wait = _ActiveTasks.Where(t => !t.IsCompleted).ToArray();
            if (need_to_wait.Length > 0)
                try { await Task.WhenAll(need_to_wait).ConfigureAwait(false); } catch { /*  */ }
        }

        // Создаём новый CTS для текущей задачи
        var cts = new CancellationTokenSource();
        lock (_ActiveCancellationsLock)
            _ActiveCancellations.Add(cts);
        var cancel = cts.Token;

        Task main_task = null;
        try
        {
            OnBeforeExecuted(parameter);
            main_task = ExecuteAsync(parameter, cancel);
            lock (_ActiveCancellationsLock)
                _ActiveTasks.Add(main_task);
            await main_task.ConfigureAwait(false);
            OnExecuted(parameter);
        }
        catch (OperationCanceledException) { /*  */ }
        catch (Exception ex) { if (OnError(ex)) return; throw; }
        finally
        {
            lock (_ActiveCancellationsLock)
            {
                _ActiveCancellations.Remove(cts);
                _ActiveTasks.Remove(main_task);
            }
            DisposeCts(cts);
            _CancelCommand?.RaiseCanExecuteChanged();
        }
        return;

        static void DisposeCts(CancellationTokenSource cts)
        {
            try { cts.Dispose(); } catch { /*  */ }
        }
    }

    /// <summary>Отменить все выполняющиеся задачи</summary>
    private void CancelInternal()
    {
        CancellationTokenSource[] snapshot;
        lock (_ActiveCancellationsLock)
            snapshot = _ActiveCancellations.ToArray();
        foreach (var c in snapshot) c.Cancel();
    }

    /// <summary>Асинхронное выполнение команды</summary>
    /// <param name="parameter">Параметр команды</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Задача выполнения команды</returns>
    public abstract Task ExecuteAsync(object? parameter, CancellationToken Cancel);
}
