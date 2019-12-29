using System;
using System.Windows;
using System.Windows.Input;
using MathCore.WPF.Commands;
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Dialogs
{
    public abstract class Dialog : DependencyObject, ICommand
    {
        #region Dependency properties

        #region Title property

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(Dialog),
                new PropertyMetadata(default(string)));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        #region IsOpened property

        private static readonly DependencyPropertyKey IsOpenedPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsOpened),
                typeof(bool),
                typeof(Dialog),
                new FrameworkPropertyMetadata(default(bool),
                    (d, e) => CommandManager.InvalidateRequerySuggested()));

        public static readonly DependencyProperty IsOpenedProperty = IsOpenedPropertyKey.DependencyProperty;

        public bool IsOpened
        {
            get => (bool)GetValue(IsOpenedProperty);
            protected set => SetValue(IsOpenedPropertyKey, value);
        }

        #endregion

        #region UpdateIfResultFalse property

        public static readonly DependencyProperty UpdateIfResultFalseProperty =
            DependencyProperty.Register(
                nameof(UpdateIfResultFalse),
                typeof(bool),
                typeof(Dialog),
                new PropertyMetadata(default(bool)));

        public bool UpdateIfResultFalse
        {
            get => (bool)GetValue(UpdateIfResultFalseProperty);
            set => SetValue(UpdateIfResultFalseProperty, value);
        }

        #endregion

        #region LastException property

        public static readonly DependencyProperty LastExceptionProperty =
            DependencyProperty.Register(
                nameof(LastException),
                typeof(Exception),
                typeof(Dialog),
                new PropertyMetadata(default(Exception)));

        public Exception LastException
        {
            get => (Exception)GetValue(LastExceptionProperty);
            set => SetValue(LastExceptionProperty, value);
        }

        #endregion

        #endregion

        protected readonly object _OpenSyncRoot = new object();
        protected ICommand _OpenCommand;

        public ICommand OpenCommand => _OpenCommand;

        protected Dialog() => _OpenCommand = new LambdaCommand((Action)Open, p => !IsOpened);

        public void Open()
        {
            if(IsOpened) return;
            lock (_OpenSyncRoot)
            {
                if(IsOpened) return;
                IsOpened = true;
                try
                {
                    OpenDialog();
                } catch(Exception error)
                {
                    LastException = error;
                    throw new ApplicationException($"Ошибка диалога {GetType()}", error);
                } finally
                {
                    IsOpened = false;
                }
            }
        }

        public virtual void Open(object? p)
        {
            if(IsOpened) return;
            lock (_OpenSyncRoot)
            {
                if(IsOpened) return;
                IsOpened = true;
                try
                {
                    OpenDialog(p);
                } catch(Exception error)
                {
                    LastException = error;
                    throw new ApplicationException($"Ошибка диалога {GetType()}", error);
                } finally
                {
                    IsOpened = false;
                }
            }
        }

        protected virtual void OpenDialog() => OpenDialog(null);

        protected abstract void OpenDialog(object? p);

        #region ICommand implimentation

        bool ICommand.CanExecute(object parameter) => _OpenCommand.CanExecute(parameter);

        void ICommand.Execute(object parameter) => _OpenCommand.Execute(parameter);

        event EventHandler ICommand.CanExecuteChanged
        {
            add => _OpenCommand.CanExecuteChanged += value;
            remove => _OpenCommand.CanExecuteChanged -= value;
        }

        #endregion 
    }
}