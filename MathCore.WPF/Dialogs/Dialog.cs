using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MathCore.WPF.Commands;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable MemberCanBeProtected.Global

namespace MathCore.WPF.Dialogs;

/// <summary>Диалог</summary>
public abstract class Dialog : DependencyObject, ICommand
{
    #region Dependency properties

    #region Title : bool  - Заголовок диалога

    /// <summary>Заголовок диалога</summary>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(Dialog),
            new(default(string)));

    /// <summary>Заголовок диалога</summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    #endregion

    #region IsOpened : bool - Диалог в данный момент открыт

    private static readonly DependencyPropertyKey __IsOpenedPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(IsOpened),
            typeof(bool),
            typeof(Dialog),
            new FrameworkPropertyMetadata(
                default(bool), 
                (_,_) => CommandManager.InvalidateRequerySuggested()));

    /// <summary>Диалог в данный момент открыт</summary>
    public static readonly DependencyProperty IsOpenedProperty = __IsOpenedPropertyKey.DependencyProperty;

    /// <summary>Диалог в данный момент открыт</summary>
    public bool IsOpened
    {
        get => (bool)GetValue(IsOpenedProperty);
        protected set => SetValue(__IsOpenedPropertyKey, value);
    }

    #endregion

    #region UpdateIfResultFalse : bool - Обновлять состояние в случае отрицательного выбора пользователя

    /// <summary>Обновлять состояние в случае отрицательного выбора пользователя</summary>
    public static readonly DependencyProperty UpdateIfResultFalseProperty =
        DependencyProperty.Register(
            nameof(UpdateIfResultFalse),
            typeof(bool),
            typeof(Dialog),
            new(default(bool)));

    /// <summary>Обновлять состояние в случае отрицательного выбора пользователя</summary>
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
            new(default(Exception)));

    public Exception LastException
    {
        get => (Exception)GetValue(LastExceptionProperty);
        set => SetValue(LastExceptionProperty, value);
    }

    #endregion

    #region Enabled : bool - Включить диалог

    /// <summary>Включить диалог</summary>
    public static readonly DependencyProperty EnabledProperty =
        DependencyProperty.Register(
            nameof(Enabled),
            typeof(bool),
            typeof(Dialog),
            new(default(bool)));

    /// <summary>Включить диалог</summary>
    //[Category("")]
    [Description("Включить диалог")]
    public bool Enabled
    {
        get => (bool)GetValue(EnabledProperty); 
        set => SetValue(EnabledProperty, value);
    }

    #endregion

    #endregion

    /// <summary>Объект для межптоковой синхронизации</summary>
    protected readonly object _OpenSyncRoot = new();

    /// <summary>Команда открытия диалога</summary>
    protected Command? _OpenCommand;

    /// <summary>Команда открытия диалога</summary>
    public ICommand OpenCommand => _OpenCommand ??= Command.New(Open, _ => Enabled && !IsOpened);

    /// <summary>Показать диалог</summary>
    public void Open()
    {
        if(!Enabled || IsOpened) return;
        lock (_OpenSyncRoot)
        {
            if(!Enabled || IsOpened) return;
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

    /// <summary>Показать диалог</summary>
    /// <param name="p">Параметр</param>
    public virtual void Open(object? p)
    {
        if(!Enabled || IsOpened) return;
        lock (_OpenSyncRoot)
        {
            if(!Enabled || IsOpened) return;
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

    /// <summary>Открыть диалог</summary>
    protected virtual void OpenDialog() => OpenDialog(null);

    /// <summary>Открыть диалог</summary>
    protected abstract void OpenDialog(object? p);

    #region ICommand implimentation

    bool ICommand.CanExecute(object? parameter) => OpenCommand.CanExecute(parameter);

    void ICommand.Execute(object? parameter) => OpenCommand.Execute(parameter);

    event EventHandler? ICommand.CanExecuteChanged
    {
        add => OpenCommand.CanExecuteChanged += value;
        remove => OpenCommand.CanExecuteChanged -= value;
    }

    #endregion 
}