using System;
using System.ComponentModel;
using System.Windows.Markup;
using MathCore.Annotations;
using MathCore.WPF.ViewModels;
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
        [NotNull]
        public static LambdaCommand OnExecute([NotNull]Action<object?> ExecuteAction, Func<object?, bool>? CanExecute = null) => new LambdaCommand(ExecuteAction, CanExecute);

        [NotNull]
        public static LambdaCommand OnExecute([NotNull]Action ExecuteAction, Func<object?, bool>? CanExecute = null) => new LambdaCommand(ExecuteAction, CanExecute);

        #region События

        /// <summary>Возникает, когда команда отменена</summary>
        public event EventHandler? Cancelled;

        protected virtual void OnCancelled(EventArgs? args = null) => Cancelled.Start(this, args);

        public event CancelEventHandler? StartExecuting;

        protected virtual void OnStartExecuting([NotNull] CancelEventArgs args) => StartExecuting?.Invoke(this, args);

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
                _CanExecute = value ?? (o => true);
                OnPropertyChanged(nameof(CanExecuteDelegate));
            }
        }

        #endregion


        #region Конструкторы

        protected LambdaCommand() { }

        public LambdaCommand([NotNull]Action<object?> ExecuteAction, Func<object?, bool>? CanExecute = null)
            : this()
        {
            _ExecuteAction = ExecuteAction ?? throw new ArgumentNullException(nameof(ExecuteAction));
            _CanExecute = CanExecute;
        }

        public LambdaCommand([NotNull]Action<object?> ExecuteAction, [CanBeNull] Func<bool>? CanExecute) : this(ExecuteAction, CanExecute is null ? (Func<object?, bool>?)null : o => CanExecute!()) { }

        public LambdaCommand([NotNull]Action ExecuteAction, Func<object?, bool>? CanExecute = null) : this(o => ExecuteAction(), CanExecute) { }

        public LambdaCommand([NotNull]Action ExecuteAction, [CanBeNull] Func<bool>? CanExecute) : this(o => ExecuteAction(), CanExecute is null ? (Func<object?, bool>?)null : o => CanExecute!()) { }

        #endregion


        #region Методы

        /// <summary>Выполнение команды</summary>
        /// <param name="parameter">Параметр процесса выполнения команды</param>
        /// <exception cref="InvalidOperationException">Метод выполнения команды не определён</exception>
        public override void Execute(object? parameter)
        {
            if (_ExecuteAction is null) throw new InvalidOperationException("Метод выполнения команды не определён");
            var cancel_args = new CancelEventArgs();
            OnStartExecuting(cancel_args);
            if (cancel_args.Cancel)
            {
                OnCancelled(cancel_args);
                if (cancel_args.Cancel) return;
            }
            _ExecuteAction?.Invoke(parameter);
            OnCompleteExecuting(new EventArgs<object?>(parameter));
        }

        /// <summary>Проверка возможности выполнения команды</summary>
        /// <param name="parameter">Параметр процесса выполнения команды</param>
        /// <returns>Истина, если команда может быть выполнена</returns>
        public override bool CanExecute(object parameter) => 
            ViewModel.IsDesignMode 
            || IsCanExecute && (_CanExecute?.Invoke(parameter) ?? true);

        /// <summary>Проверка возможности выполнения команды</summary>
        public void CanExecuteCheck() => OnCanExecuteChanged();

        #endregion

        [NotNull] public static explicit operator LambdaCommand([NotNull] Action execute) => ToLambdaCommand(execute);
        [NotNull] public static explicit operator LambdaCommand([NotNull] Action<object?> execute) => ToLambdaCommand(execute);

        [NotNull] public static LambdaCommand ToLambdaCommand([NotNull] Action execute) => new LambdaCommand(execute);
        [NotNull] public static LambdaCommand ToLambdaCommand([NotNull] Action<object?> execute) => new LambdaCommand(execute);
    }

    /// <summary>
    /// Типизированная лямбда-команда
    /// Позволяет быстро указывать методы для выполнения основного тела команды и определения возможности выполнения
    /// </summary>
    public class LambdaCommand<T> : Command
    {
        #region События

        /// <summary>Возникает, когда команда отменена</summary>
        public event EventHandler? Cancelled;

        protected virtual void OnCancelled(EventArgs? args = null) => Cancelled.Start(this, args);

        public event CancelEventHandler? StartExecuting;

        protected virtual void OnStartExecuting([NotNull] CancelEventArgs args) => StartExecuting?.Invoke(this, args);

        public event EventHandler<EventArgs<object?>>? CompleteExecuting;

        protected virtual void OnCompleteExecuting(EventArgs<object?> args) => CompleteExecuting?.Invoke(this, args);

        #endregion

        #region Поля

        /// <summary>Делегат основного тела команды</summary>
        protected Action<T>? _ExecuteAction;

        /// <summary>Функция определения возможности исполнения команды</summary>
        protected Func<T, bool>? _CanExecute;

        #endregion

        #region Свойства

        /// <summary>Функция определения возможности исполнения команды</summary>
        public Func<T, bool>? CanExecuteDelegate
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

        public LambdaCommand([NotNull] Action<T> ExecuteAction, Func<T, bool>? CanExecute = null)
        {
            _ExecuteAction = ExecuteAction ?? throw new ArgumentNullException(nameof(ExecuteAction));
            _CanExecute = ViewModel.IsDesignMode ? (o => true) : CanExecute;
        }

        #endregion

        #region Методы

        [CanBeNull]
        protected object? ConvertParameter([CanBeNull] object? parameter)
        {
            if (parameter is null) return null;
            var command_parameter_type = typeof(T);
            var parameter_type = parameter.GetType();
            if (command_parameter_type.IsAssignableFrom(parameter_type))
                return parameter;
            var converter = TypeDescriptor.GetConverter(command_parameter_type);
            if (converter.CanConvertFrom(parameter_type))
                return converter.ConvertFrom(parameter);
            converter = TypeDescriptor.GetConverter(parameter_type);
            if (converter.CanConvertFrom(command_parameter_type))
                return converter.ConvertFrom(parameter);
            return null;
        }

        public override void Execute(object? parameter)
        {
            var execute_action = _ExecuteAction ?? throw new InvalidOperationException("Метод выполенния команды не определён");
            if (parameter != null && !(parameter is T))
                parameter = ConvertParameter(parameter);
            var cancel_args = new CancelEventArgs();
            OnStartExecuting(cancel_args);
            if (cancel_args.Cancel)
            {
                OnCancelled(cancel_args);
                if (cancel_args.Cancel) return;
            }
            execute_action.Invoke((T)parameter!);
            OnCompleteExecuting(new EventArgs<object?>(parameter));
        }

        public override bool CanExecute(object? obj)
        {
            if (ViewModel.IsDesignMode) return true;
            if (!IsCanExecute) return false;
            var can_execute = _CanExecute;
            if (can_execute is null) return true;
            if (obj is null || obj is T parameter && can_execute(parameter)) return true;
            obj = ConvertParameter(obj);
            return can_execute((T)obj!);
        }

        public void CanExecuteCheck() => OnCanExecuteChanged();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;
            _ExecuteAction = null;
            _CanExecute = null;
        }

        #endregion

        [NotNull] public static explicit operator LambdaCommand<T>([NotNull] Action<T> execute) => new LambdaCommand<T>(execute);
    }
}
