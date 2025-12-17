using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace MathCore.WPF.Converters;

/// <summary>Преобразует задачу в наблюдаемый объект для привязки результатов асинхронных операций</summary>
[ValueConversion(typeof(Task), typeof(AsyncConverterTaskCompletionNotifier))]
[MarkupExtensionReturnType(typeof(AsyncConverter))]
public class AsyncConverter : MarkupExtension, IValueConverter
{
    /// <summary>Плейсхолдер возвращаемого значения пока задача выполняется</summary>
    public object? RunningPlaceholder { get; set; }

    /// <summary>Преобразует Task{T} в объект-обёртку с уведомлениями об изменениях</summary>
    public object? Convert(object? v, Type t, object? p, CultureInfo c) => v switch
    {
        Task task => new AsyncConverterTaskCompletionNotifier(task, RunningPlaceholder),
        not null  => throw new InvalidOperationException($"Тип значения {v.GetType()} не поддерживается"),
        _         => null
    };

    /// <summary>Обратное преобразование не поддерживается</summary>
    public object? ConvertBack(object? v, Type t, object? p, CultureInfo c) => throw new NotSupportedException();

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider sp) => this;
}

/// <summary>Обёртка для наблюдения за завершением Task{T} и оповещения UI</summary>
public sealed class AsyncConverterTaskCompletionNotifier : INotifyPropertyChanged
{
    private readonly object? _RunningPlaceholder;

    /// <summary>Исходная задача</summary>
    public Task Task { get; }

    /// <summary>Результат задачи или плейсхолдер, если задача ещё не завершена</summary>
    public object? Result
    {
        get
        {
            if (Task.Status != TaskStatus.RanToCompletion) return _RunningPlaceholder;

#if NETCOREAPP
            dynamic task   = Task;
            return (object?)task.Result;
#else
            var task            = Task;
            var type            = task.GetType();
            var result_property = type.GetProperty(nameof(Task<object>.Result));
            return result_property.GetValue(task);
#endif
        }
    }

    /// <summary>Статус задачи</summary>
    public TaskStatus Status => Task.Status;

    /// <summary>Возвращает true если задача завершена</summary>
    public bool IsCompleted => Task.IsCompleted;

    /// <summary>Возвращает true если задача успешно завершена</summary>
    public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

    /// <summary>Возвращает true если задача отменена</summary>
    public bool IsCanceled => Task.IsCanceled;

    /// <summary>Возвращает true если задача завершилась с ошибкой</summary>
    public bool IsFaulted => Task.IsFaulted;

    /// <summary>Исключение, связанное с задачей</summary>
    public AggregateException? Exception => Task.Exception;

    /// <summary>Внутреннее исключение, если есть</summary>
    public Exception? InnerException => Exception?.InnerException;

    /// <summary>Текст ошибки, если есть</summary>
    public string? ErrorMessage => InnerException?.Message;

    /// <summary>Создаёт обёртку для наблюдения за задачей</summary>
    public AsyncConverterTaskCompletionNotifier(Task task, object? RunningPlaceholder)
    {
        _RunningPlaceholder = RunningPlaceholder;
        var task_type = task.GetType();
        if (task_type.GenericTypeArguments is not { Length: 1 })
            throw new InvalidOperationException($"Тип задачи {task_type} не поддерживается. Требуется тип Task<T>.");

        Task = task;
        if (task.IsCompleted) return;

        var scheduler = (SynchronizationContext.Current == null) 
            ? TaskScheduler.Current 
            : TaskScheduler.FromCurrentSynchronizationContext();

        task.ContinueWith(TaskContinuation,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            scheduler);
    }

    private void TaskContinuation(Task task)
    {
        if (PropertyChanged is not { } on_property_changed) return;

        on_property_changed(this, new(nameof(IsCompleted)));
        if (task.IsCanceled)
            on_property_changed(this, new(nameof(IsCanceled)));
        else if (task.IsFaulted)
        {
            on_property_changed(this, new(nameof(IsFaulted)));
            on_property_changed(this, new(nameof(ErrorMessage)));
        }
        else
        {
            on_property_changed(this, new(nameof(IsSuccessfullyCompleted)));
            on_property_changed(this, new(nameof(Result)));
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;
}
