using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

/// <summary>Класс объектов, извещающих об завершении получения значения свойства</summary>
/// <typeparam name="T">Тип значения свойства</typeparam>
public sealed class NotifyTaskCompletion<T> : INotifyPropertyChanged
{
    #region События

    /// <summary>Событие возникает при изменении значения свойства</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Поля

    /// <summary>Задача получения значения свойства</summary>
    private readonly Task<T> _Task;

    #endregion

    #region Свойства

    /// <summary>Задача получения значения свойства</summary>
    public Task<T> Task => _Task;

    /// <summary>Результат задачи</summary>
    // ReSharper disable once AsyncConverter.AsyncWait
    public T? Result => _Task.Status == TaskStatus.RanToCompletion ? _Task.Result : default;

    /// <summary>Статус задачи</summary>
    public TaskStatus Status => _Task.Status;

    /// <summary>Признак завершения задачи</summary>
    public bool IsCompleted => _Task.IsCompleted;

    /// <summary>Признак незавершённости задачи</summary>
    public bool IsNotCompleted => !_Task.IsCompleted;

    /// <summary>Признак успешного завершения задачи</summary>
    public bool IsSuccessfullyCompleted => _Task.Status == TaskStatus.RanToCompletion;

    /// <summary>Признак отмены задачи</summary>
    public bool IsCanceled => _Task.IsCanceled;

    /// <summary>Признак наличия ошибки при выполнении задачи</summary>
    public bool IsFaulted => _Task.IsFaulted;

    /// <summary>Ошибки, полученные в результате выполнения задачи</summary>
    public AggregateException? Exception => _Task.Exception;

    /// <summary>Ошибка, породившая основное исключение</summary>
    public Exception? InnerException => Exception?.InnerException;

    /// <summary>Список произошедших исключений</summary>
    public ReadOnlyCollection<Exception>? InnerExceptions => Exception?.InnerExceptions;

    /// <summary>Сообщение об ошибке</summary>
    public string? ErrorMessage => InnerException?.Message;

    #endregion

    #region Конструкторы

    /// <summary>Инициализация экземпляра объекта, следящего за выполнением задачи получения значений</summary>
    /// <param name="task">Задача получения значения</param>
    public NotifyTaskCompletion(Task<T> task)
    {
        _Task = task;
        if (!task.IsCompleted) 
            _ = WatchTaskAsync(task);
    }

    #endregion

    #region Методы

    /// <summary>Метод слежения за задачей</summary>
    /// <param name="task">Отслеживаемая задача получения значения</param>
    /// <returns>Задача отслеживания задачи получения данных</returns>
    private Task? WatchTaskAsync(Task task)
    {
        try
        {
            return task.ContinueWith(OnTaskComplete);
        }
        catch (Exception e)
        {
            Trace.Fail(e.Message, e.ToString());
        }
        return null;

        //try
        //{
        //    await task.ConfigureAwait(false);
        //} catch(Exception e)
        //{
        //    Trace.Fail(e.Message, e.ToString());
        //}
        //var handlers = PropertyChanged;
        //if(handlers is null) return;
        //handlers(this, new PropertyChangedEventArgs(nameof(Status)));
        //handlers(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
        //handlers(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));
        //if(task.IsCanceled) handlers(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
        //else if(task.IsFaulted)
        //{
        //    handlers(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
        //    handlers(this, new PropertyChangedEventArgs(nameof(Exception)));
        //    handlers(this, new PropertyChangedEventArgs(nameof(InnerException)));
        //    handlers(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
        //}
        //else
        //{
        //    handlers(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
        //    handlers(this, new PropertyChangedEventArgs(nameof(Result)));
        //}
    }

    private void OnTaskComplete(Task task)
    {
        var handlers = PropertyChanged;
        if (handlers is null) return;
        handlers(this, new PropertyChangedEventArgs(nameof(Status)));
        handlers(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
        handlers(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));
        if (task.IsCanceled) handlers(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
        else if (task.IsFaulted)
        {
            handlers(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
            handlers(this, new PropertyChangedEventArgs(nameof(Exception)));
            handlers(this, new PropertyChangedEventArgs(nameof(InnerException)));
            handlers(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
        }
        else
        {
            handlers(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
            handlers(this, new PropertyChangedEventArgs(nameof(Result)));
        }
    }

    #endregion
}