using System;
using System.ComponentModel;
using System.Linq.Reactive;
using System.Windows.Markup;

using MathCore.Annotations;
using MathCore.WPF.ViewModels;
// ReSharper disable InconsistentNaming

// ReSharper disable MemberCanBeProtected.Global

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Commands
{
    /// <summary>
    /// Лямбда-команда
    /// Позволяет быстро указывать методы для выполнения основного тела команды и определения возможности выполнения
    /// </summary>
    [MarkupExtensionReturnType(typeof(LambdaCommand))]
    public class LambdaCommand : Command
    {
        public static LambdaCommand OnExecute(Action<object?> ExecuteAction, Func<object?, bool>? CanExecute = null) => new(ExecuteAction, CanExecute);

        public static LambdaCommand OnExecute(Action ExecuteAction, Func<object?, bool>? CanExecute = null) => new(ExecuteAction, CanExecute);

        #region События

        /// <summary>Возникает, когда команда отменена</summary>
        public event EventHandler? Cancelled;

        protected virtual void OnCancelled(EventArgs? args = null) => Cancelled.Start(this, args);

        public event CancelEventHandler? StartExecuting;

        protected virtual void OnStartExecuting(CancelEventArgs args) => StartExecuting?.Invoke(this, args);

        public event EventHandler<EventArgs<object?>>? CompleteExecuting;

        protected virtual void OnCompleteExecuting(EventArgs<object?> args) => CompleteExecuting?.Invoke(this, args);

        #endregion

        #region Поля

        /// <summary>Делегат основного тела команды</summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        protected Action<object?>? _ExecuteAction;

        /// <summary>Функция определения возможности исполнения команды</summary>
        protected Func<object?, bool>? _CanExecute;

        #endregion

        #region Свойства

        /// <summary>Функция определения возможности исполнения команды</summary>
        public Func<object?, bool>? CanExecuteDelegate
        {
            [NotNull]
            get => _CanExecute;
            [CanBeNull]
            set
            {
                if (ReferenceEquals(_CanExecute, value)) return;
                _CanExecute = value ?? (_ => true);
                OnPropertyChanged(nameof(CanExecuteDelegate));
            }
        }

        #endregion

        #region Конструкторы

        protected LambdaCommand() { }

        public LambdaCommand(Action<object?> ExecuteAction, Func<object?, bool>? CanExecute = null)
            : this()
        {
            _ExecuteAction = ExecuteAction ?? throw new ArgumentNullException(nameof(ExecuteAction));
            _CanExecute = CanExecute;
        }

        public LambdaCommand(Action<object?> ExecuteAction, Func<bool>? CanExecute) : this(ExecuteAction, CanExecute is null ? null : _ => CanExecute!()) { }

        public LambdaCommand(Action ExecuteAction, Func<object?, bool>? CanExecute = null) : this(_ => ExecuteAction(), CanExecute) { }

        public LambdaCommand(Action ExecuteAction, Func<bool>? CanExecute) : this(_ => ExecuteAction(), CanExecute is null ? null : _ => CanExecute!()) { }

        #endregion

        #region Методы

        /// <summary>Выполнение команды</summary>
        /// <param name="parameter">Параметр процесса выполнения команды</param>
        /// <exception cref="InvalidOperationException">Метод выполнения команды не определён</exception>
        public override void Execute(object? parameter)
        {
            if (_ExecuteAction is null) throw new InvalidOperationException("Метод выполнения команды не определён");
            if (!CanExecute(parameter)) return;
            var cancel_args = new CancelEventArgs();
            OnStartExecuting(cancel_args);
            if (cancel_args.Cancel)
            {
                OnCancelled(cancel_args);
                if (cancel_args.Cancel) return;
            }

            try
            {
                _ExecuteAction(parameter);
            }
            catch (Exception e) when (HasErrorHandlers)
            {
                OnError(e);
            }

            OnCompleteExecuting(new EventArgs<object?>(parameter));
        }

        /// <summary>Проверка возможности выполнения команды</summary>
        /// <param name="parameter">Параметр процесса выполнения команды</param>
        /// <returns>Истина, если команда может быть выполнена</returns>
        public override bool CanExecute(object? parameter) =>
            ViewModel.IsDesignMode
            || IsCanExecute && (_CanExecute?.Invoke(parameter) ?? true);

        /// <summary>Проверка возможности выполнения команды</summary>
        public void CanExecuteCheck() => OnCanExecuteChanged();

        #endregion

        public static implicit operator LambdaCommand(Action execute) => ToLambdaCommand(execute);
        public static implicit operator LambdaCommand(Action<object?> execute) => ToLambdaCommand(execute);

        public static implicit operator LambdaCommand((Action Execute, Func<bool> CanExecute) info) => new(info.Execute, info.CanExecute);
        public static implicit operator LambdaCommand((Action<object?> Execute, Func<object?, bool> CanExecute) info) => new(info.Execute, info.CanExecute);

        public static LambdaCommand ToLambdaCommand(Action execute) => new(execute);
        public static LambdaCommand ToLambdaCommand(Action<object?> execute) => new(execute);
    }

    /// <summary>
    /// Типизированная лямбда-команда
    /// Позволяет быстро указывать методы для выполнения основного тела команды и определения возможности выполнения
    /// </summary>
    public class LambdaCommand<T> : Command, IObservableEx<T?>
    {
        #region События

        /// <summary>Возникает, когда команда отменена</summary>
        public event EventHandler? Cancelled;

        protected virtual void OnCancelled(EventArgs? args = null) => Cancelled.Start(this, args);

        public event CancelEventHandler? StartExecuting;

        protected virtual void OnStartExecuting(CancelEventArgs args) => StartExecuting?.Invoke(this, args);

        public event EventHandler<EventArgs<object?>>? CompleteExecuting;

        protected virtual void OnCompleteExecuting(EventArgs<object?> args) => CompleteExecuting?.Invoke(this, args);

        #endregion

        #region Поля

        /// <summary>Делегат основного тела команды</summary>
        protected Action<T?>? _ExecuteAction;

        /// <summary>Функция определения возможности исполнения команды</summary>
        protected Func<T?, bool>? _CanExecute;

        #endregion

        #region Свойства

        /// <summary>Функция определения возможности исполнения команды</summary>
        public Func<T?, bool>? CanExecuteDelegate
        {
            get => _CanExecute;
            set
            {
                if (ReferenceEquals(_CanExecute, value)) return;
                _CanExecute = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Скрытый конструктор для потомков класса, желающих вручную установить значения действия выполнения команды <see cref="_ExecuteAction"/> и функции проверки выполнимости команды <see cref="_CanExecute"/>
        /// Перед началом использования команды поля <see cref="_ExecuteAction"/> и <see cref="_CanExecute"/> должны быть != <see langword="null"/>
        /// </summary>
        protected LambdaCommand() { }

        public LambdaCommand(Action<T?> ExecuteAction, Func<bool>? CanExecute)
            : this(ExecuteAction, CanExecute is null ? null : new Func<T?, bool>(_ => CanExecute()))
        { }

        public LambdaCommand(Action<T?> ExecuteAction, Func<T?, bool>? CanExecute = null)
        {
            _ExecuteAction = ExecuteAction ?? throw new ArgumentNullException(nameof(ExecuteAction));
            _CanExecute = ViewModel.IsDesignMode ? (_ => true) : CanExecute;
        }

        #endregion

        #region Методы

        public static T ConvertParameter(object? parameter)
        {
            if (parameter is null) return default!;
            if (parameter is T result) return result;

            var command_type = typeof(T);
            var parameter_type = parameter.GetType();

            if (command_type.IsAssignableFrom(parameter_type))
                return (T)parameter;

            var command_type_converter = TypeDescriptor.GetConverter(command_type);
            if (command_type_converter.CanConvertFrom(parameter_type))
                return ((T)command_type_converter.ConvertFrom(parameter))!;

            var parameter_converter = TypeDescriptor.GetConverter(parameter_type);
            if (parameter_converter.CanConvertTo(command_type))
                return (T)parameter_converter.ConvertFrom(parameter)!;

            return default!;
        }

        public override void Execute(object? parameter)
        {
            var execute_action = _ExecuteAction
                    ?? throw new InvalidOperationException(@"Метод выполнения команды не определён");

            if (parameter is not T value)
                value = parameter is null
                    ? default!
                    : ConvertParameter(parameter);

            if (!CanExecute(value)) return;

            var cancel_args = new CancelEventArgs();
            OnStartExecuting(cancel_args);
            if (cancel_args.Cancel)
            {
                OnCancelled(cancel_args);
                if (cancel_args.Cancel) return;
            }

            if (_CanExecute?.Invoke(value!) == false) return;
            try
            {
                execute_action.Invoke(value!);
            }
            catch (Exception error)
            {
                _Observable?.OnError(error);
                if (!HasErrorHandlers)
                    throw;

                OnError(error);
            }
            OnCompleteExecuting(new EventArgs<object?>(parameter));
        }

        public override bool CanExecute(object? obj)
        {
            if (ViewModel.IsDesignMode) return true;
            if (!IsCanExecute) return false;
            return _CanExecute is not { } can_execute || obj switch
            {
                null => can_execute(default!),
                T parameter => can_execute(parameter),
                _ => can_execute(ConvertParameter(obj))
            };
        }

        public void CanExecuteCheck() => OnCanExecuteChanged();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;
            _ExecuteAction = null;
            _CanExecute = null;
            _Observable?.OnCompleted();
            _Observable?.Dispose();
            _Observable = null;
        }

        #endregion

        public static implicit operator LambdaCommand<T?>(Action<T?> execute) => new(execute);

        public static implicit operator LambdaCommand<T?>((Action<T?> Execute, Func<T?, bool> CanExecute) info) => new(info.Execute, info.CanExecute);

        #region IObservable<T>

        private SimpleObservableEx<T?>? _Observable;

        public IDisposable Subscribe(IObserverEx<T?> observer) => (_Observable ??= new SimpleObservableEx<T?>()).Subscribe(observer);
        public IDisposable Subscribe(IObserver<T?> observer) => (_Observable ??= new SimpleObservableEx<T?>()).Subscribe(observer);

        #endregion

    }
}
