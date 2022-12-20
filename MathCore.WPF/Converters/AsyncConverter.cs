using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(Task), typeof(AsyncConverterTaskCompletionNotifier))]
public class AsyncConverter : MarkupExtension, IValueConverter
{
    public object? RunningPlaceholder { get; set; }

    public object? Convert(object v, Type t, object p, CultureInfo c) => v switch
    {
        Task task => new AsyncConverterTaskCompletionNotifier(task, RunningPlaceholder),
        { }       => throw new InvalidOperationException($"Тип значения {v.GetType()} не поддерживается"),
        _         => null
    };

    public object? ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotSupportedException();

    public override object ProvideValue(IServiceProvider sp) => this;
}

public sealed class AsyncConverterTaskCompletionNotifier : INotifyPropertyChanged
{
    private readonly object? _RunningPlaceholder;

    public Task Task { get; }

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

    public TaskStatus Status => Task.Status;

    public bool IsCompleted => Task.IsCompleted;

    public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

    public bool IsCanceled => Task.IsCanceled;

    public bool IsFaulted => Task.IsFaulted;

    public AggregateException? Exception => Task.Exception;

    public Exception? InnerException => Exception?.InnerException;

    public string? ErrorMessage => InnerException?.Message;

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

        on_property_changed(this, new("IsCompleted"));
        if (task.IsCanceled)
            on_property_changed(this, new("IsCanceled"));
        else if (task.IsFaulted)
        {
            on_property_changed(this, new("IsFaulted"));
            on_property_changed(this, new("ErrorMessage"));
        }
        else
        {
            on_property_changed(this, new("IsSuccessfullyCompleted"));
            on_property_changed(this, new("Result"));
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;
}
