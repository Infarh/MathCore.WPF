using System;
using System.ComponentModel;
using System.Linq.Reactive;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;
using MathCore.Annotations;
using MathCore.WPF.ViewModels;
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace MathCore.WPF.Commands
{
    /// <summary>Команда</summary>
    public abstract class Command : MarkupExtension, ICommand, INotifyPropertyChanged, IDisposable, IObservableEx<object?>
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
        protected virtual bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [CallerMemberName] string? PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }

        #endregion

        #region ICommand 

        private event EventHandler? CanExecuteChangedHandlers;

        protected virtual void OnCanExecuteChanged([CanBeNull] EventArgs? e = null) => CanExecuteChangedHandlers?.Invoke(this, e ?? EventArgs.Empty);

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

        #endregion

        #region Поля

        private bool _IsCanExecute = true;

        #endregion

        #region Свойства

        private WeakReference? _TargetObjectReference;
        private WeakReference? _RootObjectReference;
        private WeakReference? _TargetPropertyReference;

        [CanBeNull] protected object? TargetObject => _TargetObjectReference?.Target;

        [CanBeNull] protected object? RootObject => _RootObjectReference?.Target;

        [CanBeNull] protected object? TargetProperty => _TargetPropertyReference?.Target;

        /// <summary>Признак возможности исполнения</summary>
        public bool IsCanExecute
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
        [NotNull]
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
                Execute(parameter);
                _Observable?.OnNext(parameter!);
            }
            catch (Exception error)
            {
                _Observable?.OnError(error);
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
}