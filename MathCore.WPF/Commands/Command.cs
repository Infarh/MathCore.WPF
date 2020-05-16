using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xaml;
using MathCore.Annotations;
using MathCore.WPF.ViewModels;
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace MathCore.WPF.Commands
{
    public abstract class Command : MarkupExtension, ICommand, INotifyPropertyChanged, IDisposable
    {
        #region События

        #region INotifyPropertyChanged

        private event PropertyChangedEventHandler? PropertyChangedHandlers;

        event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
        {
            add => PropertyChangedHandlers += value; 
            remove => PropertyChangedHandlers -= value;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName]  string? PropertyName = null) => PropertyChangedHandlers?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

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
        public event EventHandler CanExecuteChanged
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
            var target_value_provider = (IProvideValueTarget)sp.GetService(typeof(IProvideValueTarget));
            if (target_value_provider != null)
            {
                var target = target_value_provider.TargetObject;
                _TargetObjectReference = target is null ? null : new WeakReference(target);
                var target_property = target_value_provider.TargetProperty;
                _TargetPropertyReference = target_property is null ? null : new WeakReference(target_property);
            }

            var root_object_provider = (IRootObjectProvider)sp.GetService(typeof(IRootObjectProvider));
            if (root_object_provider != null)
            {
                var root = root_object_provider.RootObject;
                _RootObjectReference = root is null ? null : new WeakReference(root);
            }

            return this;
        }

        #endregion

        #region ICommand

        public virtual bool CanExecute([CanBeNull] object parameter) => ViewModel.IsDesignMode || _IsCanExecute;
        public abstract void Execute([CanBeNull] object parameter);

        #endregion

        #region IDisposable

        protected bool _Disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _Disposed) return;

            _Disposed = true;
        }

        #endregion
    }
}