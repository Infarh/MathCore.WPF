using System.Diagnostics;
using System.Windows.Input;

using MathCore.WPF.Commands;
using MathCore.WPF.ViewModels;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Operations;

/// <summary>Операция</summary>
/// <remarks>Инициализация новой операции</remarks>
/// <param name="Execute">Функция, выполняемая операцией</param>
/// <param name="CanExecute">Проверка возможности выполнения операции</param>
public class Operation(OperationAction Execute, Func<object?, bool>? CanExecute = null) : ViewModel, IOperation
{
    /// <summary>Выполняемая операция</summary>
    private readonly OperationAction _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));

    /// <summary>Задача операции</summary>
    private Task? _OperationTask;

    /// <summary>Механизм отмены операции</summary>
    private CancellationTokenSource? _Cancellation;

    /// <summary>Таймер замера времени выполнения операции</summary>
    private Stopwatch? _Timer;

    #region Command Start - Выполнить операцию

    /// <summary>Выполнить операцию</summary>
    private Command? _Start;

    /// <summary>Выполнить операцию</summary>
    public ICommand Start => _Start ??= Command.New(OnStartCommandExecutedAsync, CanExecute);

    /// <summary>Логика выполнения - Выполнить операцию</summary>
    private Task OnStartCommandExecutedAsync(object? p)
    {
        Error         = null;
        _Cancellation = new();
        var cancel = _Cancellation.Token;
        _Timer         = Stopwatch.StartNew();
        _OperationTask = _Execute(p, new Progress<double>(progress => Progress = progress), cancel);
        InProgress     = true;

        _ = _OperationTask.ContinueWith(OnOperationCompletedAsync, CancellationToken.None);

        return _OperationTask;
    }

    /// <summary>Действие, выполняемое по завершении операции</summary>
    /// <param name="ResultTask">Завершённая задача выполненной операции</param>
    protected virtual Task OnOperationCompletedAsync(Task ResultTask)
    {
        Error = ResultTask switch
        {
            { IsFaulted                : false }               => null,
            { Exception.InnerExceptions: { Count: 1 } errors } => errors[0],
            _                                                  => ResultTask.Exception
        };

        _Timer?.Stop();
        _Cancellation = null;
        InProgress    = false;
        Progress      = 0;
        SetTime(default, default, double.NaN);

        return Task.CompletedTask;
    }

    #endregion

    #region Progress : double - Прогресс операции

    /// <summary>Прогресс операции</summary>
    private double _Progress;

    /// <summary>Прогресс операции</summary>
    public double Progress
    {
        get => _Progress;
        protected set
        {
            if (!Set(ref _Progress, value) || _Timer?.Elapsed is not { TotalSeconds: > 0 } elapsed) return;

            if (value is <= 0 or >= 1) return;
            var percent_per_second = value / elapsed.TotalSeconds;

            SetTime(elapsed, TimeSpan.FromSeconds((1 - value) / percent_per_second), percent_per_second);
        }
    }

    /// <summary>Метод установки временных метрик</summary>
    /// <param name="Elapsed">Прошедшее время</param>
    /// <param name="Remaining">Вычисленное оставшееся время до достижения значения прогресса == 1</param>
    /// <param name="pps">Скорость движения значения прогресса - число процентов в секунду</param>
    private void SetTime(TimeSpan Elapsed, TimeSpan Remaining, double pps)
    {
        ElapsedTime      = Elapsed;
        RemainingTime    = Remaining;
        PercentPerSecond = pps;

        OnPropertyChanged(nameof(ElapsedTime));
        OnPropertyChanged(nameof(RemainingTime));
        OnPropertyChanged(nameof(pps));
    }

    /// <summary>Прошедшее время</summary>
    public TimeSpan ElapsedTime { get; private set; }

    /// <summary>Оставшееся время до достижения прогресса значения 1</summary>
    public TimeSpan RemainingTime { get; private set; }

    /// <summary>Скорость изменения значения прогресса - число процентов в секунду</summary>
    public double PercentPerSecond { get; private set; } = double.NaN;

    #endregion

    #region Command Cancel - Отмена операции

    /// <summary>Отмена операции</summary>
    private Command? _Cancel;

    /// <summary>Отмена операции</summary>
    public ICommand Cancel => _Cancel ??= Command.New(OnCancelCommandExecuted, CanCancelCommandExecute);

    /// <summary>Проверка возможности выполнения - Отмена операции</summary>
    private bool CanCancelCommandExecute() => _Cancellation != null;

    /// <summary>Логика выполнения - Отмена операции</summary>
    private void OnCancelCommandExecuted()
    {
        _Cancellation?.Cancel();
        Error = null;
    }

    #endregion

    #region Error : Exception - Ошибка операции

    /// <summary>Ошибка операции</summary>
    private Exception? _Error;

    /// <summary>Ошибка операции</summary>
    public Exception? Error { get => _Error; set => Set(ref _Error, value); }

    /// <summary>Операция завершилась ошибкой</summary>
    [DependencyOn(nameof(Error))]
    public bool HasError => _Error is not null;

    /// <summary>Текст ошибки операции</summary>
    [DependencyOn(nameof(Error))]
    public string? ErrorMessage => _Error?.Message;

    #endregion

    #region InProgress : bool - Операция в процессе выполнения

    /// <summary>Операция в процессе выполнения</summary>
    private bool _InProgress;

    /// <summary>Операция в процессе выполнения</summary>
    public bool InProgress { get => _InProgress; protected set => Set(ref _InProgress, value); }

    #endregion

    #region ICommand

    bool ICommand.CanExecute(object? parameter) => Start.CanExecute(parameter);

    void ICommand.Execute(object? parameter) => Start.Execute(parameter);

    event EventHandler? ICommand.CanExecuteChanged
    {
        add => Start.CanExecuteChanged += value;
        remove => Start.CanExecuteChanged -= value;
    } 

    #endregion
}

/// <summary>Операция</summary>
/// <remarks>Инициализация новой операции</remarks>
/// <param name="Execute">Функция, выполняемая операцией</param>
/// <param name="CanExecute">Проверка возможности выполнения операции</param>
public class Operation<T>(OperationAction<T> Execute, Func<T?, bool>? CanExecute = null) : ViewModel, IOperation
{
    /// <summary>Выполняемая операция</summary>
    private readonly OperationAction<T> _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));

    /// <summary>Задача операции</summary>
    private Task? _OperationTask;

    /// <summary>Механизм отмены операции</summary>
    private CancellationTokenSource? _Cancellation;

    /// <summary>Таймер замера времени выполнения операции</summary>
    private Stopwatch? _Timer;

    #region Command Start - Выполнить операцию

    /// <summary>Выполнить операцию</summary>
    private Command? _Start;

    /// <summary>Выполнить операцию</summary>
    public ICommand Start => _Start ??= Command.New(OnStartCommandExecutedAsync, CanExecute);

    /// <summary>Логика выполнения - Выполнить операцию</summary>
    private Task OnStartCommandExecutedAsync(T? p)
    {
        Error         = null;
        _Cancellation = new();
        var cancel = _Cancellation.Token;
        _Timer         = Stopwatch.StartNew();
        _OperationTask = _Execute(p, new Progress<double>(progress => Progress = progress), cancel);
        InProgress     = true;

        _ = _OperationTask.ContinueWith(OnOperationCompletedAsync, CancellationToken.None);

        return _OperationTask;
    }

    /// <summary>Действие, выполняемое по завершении операции</summary>
    /// <param name="ResultTask">Завершённая задача выполненной операции</param>
    protected virtual Task OnOperationCompletedAsync(Task ResultTask)
    {
        Error = ResultTask switch
        {
            { IsFaulted                : false }               => null,
            { Exception.InnerExceptions: { Count: 1 } errors } => errors[0],
            _                                                  => ResultTask.Exception
        };

        _Timer?.Stop();
        _Cancellation = null;
        InProgress    = false;
        Progress      = 0;
        SetTime(default, default, double.NaN);

        return Task.CompletedTask;
    }

    #endregion

    #region Progress : double - Прогресс операции

    /// <summary>Прогресс операции</summary>
    private double _Progress;

    /// <summary>Прогресс операции</summary>
    public double Progress
    {
        get => _Progress;
        protected set
        {
            if (!Set(ref _Progress, value) || _Timer?.Elapsed is not { TotalSeconds: > 0 } elapsed) return;

            if (value is <= 0 or >= 1) return;
            var percent_per_second = value / elapsed.TotalSeconds;

            SetTime(elapsed, TimeSpan.FromSeconds((1 - value) / percent_per_second), percent_per_second);
        }
    }

    /// <summary>Метод установки временных метрик</summary>
    /// <param name="Elapsed">Прошедшее время</param>
    /// <param name="Remaining">Вычисленное оставшееся время до достижения значения прогресса == 1</param>
    /// <param name="pps">Скорость движения значения прогресса - число процентов в секунду</param>
    private void SetTime(TimeSpan Elapsed, TimeSpan Remaining, double pps)
    {
        ElapsedTime      = Elapsed;
        RemainingTime    = Remaining;
        PercentPerSecond = pps;

        OnPropertyChanged(nameof(ElapsedTime));
        OnPropertyChanged(nameof(RemainingTime));
        OnPropertyChanged(nameof(pps));
    }

    /// <summary>Прошедшее время</summary>
    public TimeSpan ElapsedTime { get; private set; }

    /// <summary>Оставшееся время до достижения прогресса значения 1</summary>
    public TimeSpan RemainingTime { get; private set; }

    /// <summary>Скорость изменения значения прогресса - число процентов в секунду</summary>
    public double PercentPerSecond { get; private set; } = double.NaN;

    #endregion

    #region Command Cancel - Отмена операции

    /// <summary>Отмена операции</summary>
    private Command? _Cancel;

    /// <summary>Отмена операции</summary>
    public ICommand Cancel => _Cancel ??= Command.New(OnCancelCommandExecuted, CanCancelCommandExecute);

    /// <summary>Проверка возможности выполнения - Отмена операции</summary>
    private bool CanCancelCommandExecute() => _Cancellation != null;

    /// <summary>Логика выполнения - Отмена операции</summary>
    private void OnCancelCommandExecuted()
    {
        _Cancellation?.Cancel();
        Error = null;
    }

    #endregion

    #region Error : Exception - Ошибка операции

    /// <summary>Ошибка операции</summary>
    private Exception? _Error;

    /// <summary>Ошибка операции</summary>
    public Exception? Error { get => _Error; set => Set(ref _Error, value); }

    /// <summary>Операция завершилась ошибкой</summary>
    [DependencyOn(nameof(Error))]
    public bool HasError => _Error is not null;

    /// <summary>Текст ошибки операции</summary>
    [DependencyOn(nameof(Error))]
    public string? ErrorMessage => _Error?.Message;

    #endregion

    #region InProgress : bool - Операция в процессе выполнения

    /// <summary>Операция в процессе выполнения</summary>
    private bool _InProgress;

    /// <summary>Операция в процессе выполнения</summary>
    public bool InProgress { get => _InProgress; protected set => Set(ref _InProgress, value); }

    #endregion

    #region ICommand

    bool ICommand.CanExecute(object? parameter) => Start.CanExecute(parameter);

    void ICommand.Execute(object? parameter) => Start.Execute(parameter);

    event EventHandler? ICommand.CanExecuteChanged
    {
        add => Start.CanExecuteChanged += value;
        remove => Start.CanExecuteChanged -= value;
    }

    #endregion
}

/// <summary>Операция</summary>
/// <remarks>Инициализация новой операции</remarks>
/// <param name="Execute">Функция, выполняемая операцией</param>
/// <param name="CanExecute">Проверка возможности выполнения операции</param>
public class Operation<T, TResult>(OperationFunc<T, TResult> Execute, Func<T?, bool>? CanExecute = null) : ViewModel, IOperation
{
    /// <summary>Выполняемая операция</summary>
    private readonly OperationFunc<T, TResult> _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));

    /// <summary>Задача операции</summary>
    private Task<TResult>? _OperationTask;

    /// <summary>Механизм отмены операции</summary>
    private CancellationTokenSource? _Cancellation;

    /// <summary>Таймер замера времени выполнения операции</summary>
    private Stopwatch? _Timer;

    #region Command Start - Выполнить операцию

    /// <summary>Выполнить операцию</summary>
    private Command? _Start;

    /// <summary>Выполнить операцию</summary>
    public ICommand Start => _Start ??= Command.New(OnStartCommandExecutedAsync, CanExecute);

    /// <summary>Логика выполнения - Выполнить операцию</summary>
    private Task OnStartCommandExecutedAsync(T? p)
    {
        Error         = null;
        _Cancellation = new();
        var cancel = _Cancellation.Token;
        _Timer         = Stopwatch.StartNew();
        _OperationTask = _Execute(p, new Progress<double>(progress => Progress = progress), cancel);
        InProgress     = true;

        _ = _OperationTask.ContinueWith(OnOperationCompletedAsync, CancellationToken.None);

        return _OperationTask;
    }

    /// <summary>Действие, выполняемое по завершении операции</summary>
    /// <param name="ResultTask">Завершённая задача выполненной операции</param>
    protected virtual async Task OnOperationCompletedAsync(Task ResultTask)
    {
        Error = ResultTask switch
        {
            { IsFaulted                : false }               => null,
            { Exception.InnerExceptions: { Count: 1 } errors } => errors[0],
            _                                                  => ResultTask.Exception
        };

        if (!ResultTask.IsFaulted)
            Result = await (Task<TResult>)ResultTask;

        _Timer?.Stop();
        _Cancellation = null;
        InProgress    = false;
        Progress      = 0;
        SetTime(default, default, double.NaN);
    }

    #endregion

    #region Result : Результат операции

    /// <summary>Результат операции</summary>
    private TResult? _Result;

    /// <summary>Результат операции</summary>
    public TResult? Result { get => _Result; protected set => Set(ref _Result, value); }

    #endregion

    #region Progress : double - Прогресс операции

    /// <summary>Прогресс операции</summary>
    private double _Progress;

    /// <summary>Прогресс операции</summary>
    public double Progress
    {
        get => _Progress;
        protected set
        {
            if (!Set(ref _Progress, value) || _Timer?.Elapsed is not { TotalSeconds: > 0 } elapsed) return;

            if (value is <= 0 or >= 1) return;
            var percent_per_second = value / elapsed.TotalSeconds;

            SetTime(elapsed, TimeSpan.FromSeconds((1 - value) / percent_per_second), percent_per_second);
        }
    }

    /// <summary>Метод установки временных метрик</summary>
    /// <param name="Elapsed">Прошедшее время</param>
    /// <param name="Remaining">Вычисленное оставшееся время до достижения значения прогресса == 1</param>
    /// <param name="pps">Скорость движения значения прогресса - число процентов в секунду</param>
    private void SetTime(TimeSpan Elapsed, TimeSpan Remaining, double pps)
    {
        ElapsedTime      = Elapsed;
        RemainingTime    = Remaining;
        PercentPerSecond = pps;

        OnPropertyChanged(nameof(ElapsedTime));
        OnPropertyChanged(nameof(RemainingTime));
        OnPropertyChanged(nameof(pps));
    }

    /// <summary>Прошедшее время</summary>
    public TimeSpan ElapsedTime { get; private set; }

    /// <summary>Оставшееся время до достижения прогресса значения 1</summary>
    public TimeSpan RemainingTime { get; private set; }

    /// <summary>Скорость изменения значения прогресса - число процентов в секунду</summary>
    public double PercentPerSecond { get; private set; } = double.NaN;

    #endregion

    #region Command Cancel - Отмена операции

    /// <summary>Отмена операции</summary>
    private Command? _Cancel;

    /// <summary>Отмена операции</summary>
    public ICommand Cancel => _Cancel ??= Command.New(OnCancelCommandExecuted, CanCancelCommandExecute);

    /// <summary>Проверка возможности выполнения - Отмена операции</summary>
    private bool CanCancelCommandExecute() => _Cancellation != null;

    /// <summary>Логика выполнения - Отмена операции</summary>
    private void OnCancelCommandExecuted()
    {
        _Cancellation?.Cancel();
        Error = null;
    }

    #endregion

    #region Error : Exception - Ошибка операции

    /// <summary>Ошибка операции</summary>
    private Exception? _Error;

    /// <summary>Ошибка операции</summary>
    public Exception? Error { get => _Error; set => Set(ref _Error, value); }

    /// <summary>Операция завершилась ошибкой</summary>
    [DependencyOn(nameof(Error))]
    public bool HasError => _Error is not null;

    /// <summary>Текст ошибки операции</summary>
    [DependencyOn(nameof(Error))]
    public string? ErrorMessage => _Error?.Message;

    #endregion

    #region InProgress : bool - Операция в процессе выполнения

    /// <summary>Операция в процессе выполнения</summary>
    private bool _InProgress;

    /// <summary>Операция в процессе выполнения</summary>
    public bool InProgress { get => _InProgress; protected set => Set(ref _InProgress, value); }

    #endregion

    #region ICommand

    bool ICommand.CanExecute(object? parameter) => Start.CanExecute(parameter);

    void ICommand.Execute(object? parameter) => Start.Execute(parameter);

    event EventHandler? ICommand.CanExecuteChanged
    {
        add => Start.CanExecuteChanged += value;
        remove => Start.CanExecuteChanged -= value;
    }

    #endregion
}