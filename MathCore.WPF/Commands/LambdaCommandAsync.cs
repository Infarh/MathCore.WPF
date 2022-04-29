

// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Commands;

public class LambdaCommandAsync : Command
{
    private readonly Func<object?, Task> _ExecuteAsync;
    private readonly Func<object?, bool>? _CanExecuteAsync;

    private volatile Task? _ExecutingTask;

    /// <summary>Выполнять задачу принудительно в фоновом потоке</summary>
    public bool Background { get; set; }

    public LambdaCommandAsync(Func<Task> ExecuteAsync, Func<bool>? CanExecute = null)
        : this(
            ExecuteAsync is null ? throw new ArgumentNullException(nameof(ExecuteAsync)) : new Func<object?, Task>(_ => ExecuteAsync()),
            CanExecute is null ? null : _ => CanExecute!())
    { }

    public LambdaCommandAsync(Func<object?, Task> ExecuteAsync, Func<bool>? CanExecute = null)
        : this(ExecuteAsync, CanExecute is null ? null : _ => CanExecute!())
    { }

    public LambdaCommandAsync(Func<object?, Task> ExecuteAsync, Func<object?, bool>? CanExecuteAsync = null)
    {
        _ExecuteAsync = ExecuteAsync ?? throw new ArgumentNullException(nameof(ExecuteAsync));
        _CanExecuteAsync = CanExecuteAsync;
    }

    public override bool CanExecute(object? parameter) =>
        (_ExecutingTask is null || _ExecutingTask.IsCompleted)
        && (_CanExecuteAsync?.Invoke(parameter) ?? true);

    public override async void Execute(object? parameter)
    {
        var background = Background;

        var can_execute = background 
            ? await Task.Run(() => CanExecute(parameter))
            : CanExecute(parameter);
        if (!can_execute) return;

        var execute_async = background ? Task.Run(() => _ExecuteAsync(parameter)) : _ExecuteAsync(parameter);
        _ = Interlocked.Exchange(ref _ExecutingTask, execute_async);
        _ExecutingTask = execute_async;
        OnCanExecuteChanged();

        try
        {
            await execute_async.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {

        }

        OnCanExecuteChanged();
    }
}

public class LambdaCommandAsync<T> : Command
{
    private readonly Func<T?, Task> _ExecuteAsync;
    private readonly Func<T?, bool>? _CanExecuteAsync;

    private volatile Task? _ExecutingTask;

    /// <summary>Выполнять задачу принудительно в фоновом потоке</summary>
    public bool Background { get; set; }

    public LambdaCommandAsync(Func<Task> ExecuteAsync, Func<bool>? CanExecuteAsync = null)
        :this(
            ExecuteAsync is null ? throw new ArgumentNullException(nameof(ExecuteAsync)) : new Func<T?, Task>(_ => ExecuteAsync()),
            CanExecuteAsync is null ? null : new Func<T?, bool>(_ => CanExecuteAsync()))
    { }

    public LambdaCommandAsync(Func<T?, Task> ExecuteAsync, Func<bool>? CanExecuteAsync = null)
        : this(ExecuteAsync, CanExecuteAsync is null ? null : new Func<T?, bool>(_ => CanExecuteAsync()))
    { }

    public LambdaCommandAsync(Func<T?, Task> ExecuteAsync, Func<T?, bool>? CanExecuteAsync = null)
    {
        _ExecuteAsync = ExecuteAsync ?? throw new ArgumentNullException(nameof(ExecuteAsync));
        _CanExecuteAsync = CanExecuteAsync;
    }

    public override bool CanExecute(object? parameter) =>
        (_ExecutingTask is null || _ExecutingTask.IsCompleted)
        && (_CanExecuteAsync?.Invoke(LambdaCommand<T?>.ConvertParameter(parameter)) ?? true);

    public override async void Execute(object? parameter)
    {
        var background = Background;
        if (parameter is not T value)
            value = parameter is null
                ? default!
                : background 
                    ? await Task.Run(() => LambdaCommand<T>.ConvertParameter(parameter)) 
                    : LambdaCommand<T>.ConvertParameter(parameter);

        var can_execute = background
            ? await Task.Run(() => CanExecute(value))
            : CanExecute(value);

        if (!can_execute) return;

        var execute_async = background
            ? Task.Run(() => _ExecuteAsync(value!)) 
            : _ExecuteAsync(value!);

        _ = Interlocked.Exchange(ref _ExecutingTask, execute_async);
        _ExecutingTask = execute_async;
        OnCanExecuteChanged();

        try
        {
            await execute_async.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {

        }

        OnCanExecuteChanged();
    }
}