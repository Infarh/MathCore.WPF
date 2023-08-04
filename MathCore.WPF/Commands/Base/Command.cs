using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Reactive;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Markup;

using MathCore.Annotations;
using MathCore.WPF.ViewModels;
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable EventNeverSubscribedTo.Global

namespace MathCore.WPF.Commands;

/// <summary>Команда</summary>
public abstract partial class Command : MarkupExtension, ICommand, INotifyPropertyChanged, IDisposable, IObservableEx<object?>
{
    /// <summary>Создать команду <see cref="LambdaCommand"/></summary>
    /// <param name="OnExecute">Действие, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommand"/></returns>
    public static LambdaCommand New(Action OnExecute, Func<bool>? CanExecute = null) => new(OnExecute, CanExecute);

    /// <summary>Создать команду <see cref="LambdaCommand"/></summary>
    /// <param name="OnExecute">Действие с параметром, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommand"/></returns>
    public static LambdaCommand New(Action<object?> OnExecute, Func<object?, bool>? CanExecute = null) => new(OnExecute, CanExecute);

    /// <summary>Создать команду <see cref="LambdaCommand"/></summary>
    /// <param name="OnExecute">Действие с параметром, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommand"/></returns>
    public static LambdaCommand New(Action<object?> OnExecute, Func<bool>? CanExecute) => new(OnExecute, CanExecute);

    /// <summary>Создать команду <see cref="LambdaCommand{T}"/></summary>
    /// <param name="OnExecute">Действие с параметром <typeparamref name="T"/>, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommand{T}"/></returns>
    public static LambdaCommand<T> New<T>(Action<T?> OnExecute, Func<T?, bool>? CanExecute = null) => new(OnExecute, CanExecute);

    /// <summary>Создать команду <see cref="LambdaCommand{T}"/></summary>
    /// <param name="OnExecute">Действие с параметром <typeparamref name="T"/>, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommand{T}"/></returns>
    public static LambdaCommand<T> New<T>(Action<T?> OnExecute, Func<bool>? CanExecute) => new(OnExecute, CanExecute);

    /// <summary>Создать асинхронную команду <see cref="LambdaCommandAsync"/></summary>
    /// <param name="OnExecute">Асинхронное действие, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommandAsync"/></returns>
    public static LambdaCommandAsync New(Func<Task> OnExecute, Func<bool>? CanExecute = null) => new(OnExecute, CanExecute);

    /// <summary>Создать асинхронную команду <see cref="LambdaCommandAsync"/></summary>
    /// <param name="OnExecute">Асинхронное действие, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommandAsync"/></returns>
    public static LambdaCommandAsync New(Func<object?, Task> OnExecute, Func<object?, bool>? CanExecute = null) => new(OnExecute, CanExecute);

    /// <summary>Создать асинхронную команду <see cref="LambdaCommandAsync"/></summary>
    /// <param name="OnExecute">Асинхронное действие, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommandAsync"/></returns>
    public static LambdaCommandAsync New(Func<object?, Task> OnExecute, Func<bool>? CanExecute) => new(OnExecute, CanExecute);

    /// <summary>Создать асинхронную команду <see cref="LambdaCommandAsync{T}"/></summary>
    /// <param name="OnExecute">Асинхронное действие с параметром <typeparamref name="T"/>, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommandAsync{T}"/></returns>
    public static LambdaCommandAsync<T> New<T>(Func<T?, Task> OnExecute, Func<T?, bool>? CanExecute = null) => new(OnExecute, CanExecute);

    /// <summary>Создать асинхронную команду <see cref="LambdaCommandAsync{T}"/></summary>
    /// <param name="OnExecute">Асинхронное действие с параметром <typeparamref name="T"/>, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommandAsync{T}"/></returns>
    public static LambdaCommandAsync<T> New<T>(Func<T?, Task> OnExecute, Func<bool>? CanExecute) => new(OnExecute, CanExecute);

    /// <summary>Создать асинхронную команду <see cref="LambdaCommandAsync"/>, выполняемую в фоновом потоке</summary>
    /// <param name="OnExecute">Асинхронное действие, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommandAsync"/></returns>
    public static LambdaCommandAsync NewBackground(Func<Task> OnExecute, Func<bool>? CanExecute = null) => new(OnExecute, CanExecute) { Background = true };

    /// <summary>Создать асинхронную команду <see cref="LambdaCommandAsync"/>, выполняемую в фоновом потоке</summary>
    /// <param name="OnExecute">Асинхронное действие, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommandAsync"/></returns>
    public static LambdaCommandAsync NewBackground(Func<object?, Task> OnExecute, Func<object?, bool>? CanExecute = null) => new(OnExecute, CanExecute) { Background = true };

    /// <summary>Создать асинхронную команду <see cref="LambdaCommandAsync"/>, выполняемую в фоновом потоке</summary>
    /// <param name="OnExecute">Асинхронное действие, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommandAsync"/></returns>
    public static LambdaCommandAsync NewBackground(Func<object?, Task> OnExecute, Func<bool>? CanExecute) => new(OnExecute, CanExecute) { Background = true };

    /// <summary>Создать асинхронную команду <see cref="LambdaCommandAsync{T}"/>, выполняемую в фоновом потоке</summary>
    /// <param name="OnExecute">Асинхронное действие с параметром <typeparamref name="T"/>, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommandAsync{T}"/></returns>
    public static LambdaCommandAsync<T> NewBackground<T>(Func<T?, Task> OnExecute, Func<T?, bool>? CanExecute = null) => new(OnExecute, CanExecute) { Background = true };

    /// <summary>Создать асинхронную команду <see cref="LambdaCommandAsync{T}"/>, выполняемую в фоновом потоке</summary>
    /// <param name="OnExecute">Асинхронное действие с параметром <typeparamref name="T"/>, выполняемое командой</param>
    /// <param name="CanExecute">Функция проверки возможности выполнения действия</param>
    /// <returns>Новая <see cref="LambdaCommandAsync{T}"/></returns>
    public static LambdaCommandAsync<T> NewBackground<T>(Func<T?, Task> OnExecute, Func<bool>? CanExecute) => new(OnExecute, CanExecute) { Background = true };

    public static DialogWindowCommand Dialog() => new();

    #region События

    #region INotifyPropertyChanged

    private event PropertyChangedEventHandler? PropertyChangedHandlers;

    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add => PropertyChangedHandlers += value;
        remove => PropertyChangedHandlers -= value;
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? PropertyName = null) => PropertyChangedHandlers?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

    [NotifyPropertyChangedInvocator]
    protected virtual bool Set<T>(ref T? field, T? value, [CallerMemberName] string? PropertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(PropertyName);
        return true;
    }

    #endregion

    #region ICommand 

    private event EventHandler? CanExecuteChangedHandlers;

    protected virtual void OnCanExecuteChanged(EventArgs? e = null) => CanExecuteChangedHandlers?.Invoke(this, e ?? EventArgs.Empty);

    /// <summary>Событие возникает при изменении возможности исполнения команды</summary>
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
            CanExecuteChangedHandlers += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
            CanExecuteChangedHandlers -= value;
        }
    }

    #endregion

    public event ExceptionEventHandler<Exception>? Error;

    protected bool HasErrorHandlers => Error is not null;

    protected virtual bool OnError(Exception error)
    {
        if (Error is not { } handlers) return false;

        var args = new ExceptionEventHandlerArgs<Exception>(error);
        handlers.Invoke(this, args);

        return !args.NeedToThrow;
    }

    public Command TraceErrors() => OnError((s, e) => Trace.TraceError("Ошибка при выполнении {0}: {1}", s, e));

    public Command SkipErrors() => OnError((_, _) => { });

    public Command OnError(Action<Exception> Handler) => OnError((_, e) => Handler(e));

    public Command OnError(ExceptionEventHandler<Exception> Handler)
    {
        Error += Handler;
        return this;
    }

    public event EventHandler<EventArgs<object?>>? Executed;

    protected virtual void OnExecuted(object? p)
    {
        if(Executed is not { } handler) return;

        handler.ThreadSafeInvoke(this, p);
    }

    public event EventHandler<EventArgs<object?>>? BeforeExecuted;

    protected virtual void OnBeforeExecuted(object? p)
    {
        if (BeforeExecuted is not { } handler) return;

        handler.ThreadSafeInvoke(this, p);
    }

    #endregion

    #region Поля

    private bool _IsCanExecute = true;

    #endregion

    #region Свойства

    private WeakReference? _TargetObjectReference;
    private WeakReference? _RootObjectReference;
    private WeakReference? _TargetPropertyReference;

    protected object? TargetObject => _TargetObjectReference?.Target;

    protected object? RootObject => _RootObjectReference?.Target;

    protected object? TargetProperty => _TargetPropertyReference?.Target;

    /// <summary>Признак возможности исполнения</summary>
    public virtual bool IsCanExecute
    {
        get => _IsCanExecute;
        set
        {
            if (_IsCanExecute == value) return;
            _IsCanExecute = value;
            OnPropertyChanged(nameof(IsCanExecute));
            CommandManager.InvalidateRequerySuggested();
        }
    }

    #endregion

    #region MarkupExtension

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider sp)
    {
        //var target_value_provider = (IProvideValueTarget)sp.GetService(typeof(IProvideValueTarget));
        var target_value_provider = sp.GetValueTargetProvider();
        if (target_value_provider != null)
        {
            var target = target_value_provider.TargetObject;
            _TargetObjectReference = target is null ? null : new WeakReference(target);
            var target_property = target_value_provider.TargetProperty;
            _TargetPropertyReference = target_property is null ? null : new WeakReference(target_property);
        }

        //var root_object_provider = (IRootObjectProvider)sp.GetService(typeof(IRootObjectProvider));
        var root_object_provider = sp.GetRootObjectProvider();
        if (root_object_provider != null)
        {
            var root = root_object_provider.RootObject;
            _RootObjectReference = root is null ? null : new WeakReference(root);
        }

        return this;
    }

    #endregion

    public virtual bool CanExecute(object? parameter) => ViewModel.IsDesignMode || _IsCanExecute;

    public abstract void Execute(object? parameter);

    #region ICommand

    bool ICommand.CanExecute(object? parameter) => CanExecute(parameter);

    void ICommand.Execute(object? parameter)
    {
        if (!CanExecute(parameter)) return;
        try
        {
            OnBeforeExecuted(parameter);

            Execute(parameter);
            _Observable?.OnNext(parameter!);

            OnExecuted(parameter);
        }
        catch (Exception error)
        {
            _Observable?.OnError(error);

            if(!OnError(error))
                throw;
        }
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool _Disposed;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || _Disposed) return;

        _Observable?.OnCompleted();
        _Observable?.Dispose();
        _Observable = null;
        _Disposed = true;
    }

    #endregion

    #region IObservable<object>

    private SimpleObservableEx<object?>? _Observable;

    public IDisposable Subscribe(IObserverEx<object?> observer) => (_Observable ??= new SimpleObservableEx<object?>()).Subscribe(observer);
    public IDisposable Subscribe(IObserver<object?> observer) => (_Observable ??= new SimpleObservableEx<object?>()).Subscribe(observer);

    #endregion 
}