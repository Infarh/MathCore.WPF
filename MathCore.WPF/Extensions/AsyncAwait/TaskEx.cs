using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

public static class TaskExWPF
{
    /// <summary>Переход в поток интерфейса</summary>
    public static DispatcherAwaiter ConfigureAwaitWPF(this YieldAwaitable _) => Application.Current.Dispatcher.GetAwaiter();

    /// <summary>Переход в поток интерфейса</summary>
    /// <param name="task">Задача, продолжение которой нужно выполнить в потоке интерфейса</param>
    /// <param name="Priority">Приоритет выполнения диспетчером продолжения</param>
    public static TaskDispatcherAwaitable ConfigureAwaitWPF(this Task task, DispatcherPriority Priority = DispatcherPriority.Normal) => new(Application.Current.Dispatcher, task, Priority);


    public static Task OnSuccess(this Task task, Action continuation, Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal) =>
        task.ContinueWith(
            (_, o) => ((Dispatcher)((object[])o)[0]).Invoke((Action)((object[])o)[1], (DispatcherPriority)((object[])o)[2]),
            new object[] { dispatcher, continuation, Priority },
            TaskContinuationOptions.OnlyOnRanToCompletion);

    public static Task OnSuccess(this Task task, Action continuation, Dispatcher dispatcher, CancellationToken Cancel, DispatcherPriority Priority = DispatcherPriority.Normal) =>
        task.ContinueWith(
            (_, o) => ((Dispatcher)((object[])o)[0]).Invoke((Action)((object[])o)[1], (DispatcherPriority)((object[])o)[2], (CancellationToken)((object[])o)[3]),
            new object[] { dispatcher, continuation, Priority, Cancel },
            Cancel,
            TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Current);

    public static Task OnSuccess<T>(this Task<T> task, Action<T> continuation, Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal) => task
       .ContinueWith(
            (t, o) => ((Dispatcher)((object[])o)[0]).Invoke((Delegate)((object[])o)[1], (DispatcherPriority)((object[])o)[2], t.Result),
            new object[] { dispatcher, continuation, Priority },
            TaskContinuationOptions.OnlyOnRanToCompletion);

    public static Task<TResult> OnSuccess<T, TResult>(this Task<T> task, Func<T, TResult> continuation, Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal) => task
       .ContinueWith(
            (t, o) => (TResult)((Dispatcher)((object[])o)[0]).Invoke((Delegate)((object[])o)[1], (DispatcherPriority)((object[])o)[2], t.Result),
            new object[] { dispatcher, continuation, Priority },
            TaskContinuationOptions.OnlyOnRanToCompletion);

    public static Task OnCancelled(this Task task, Action continuation, Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal) => task
       .ContinueWith(
            (_, o) => ((Dispatcher)((object[])o)[0]).Invoke((Action)((object[])o)[1], (DispatcherPriority)((object[])o)[2]),
            new object[] { dispatcher, continuation, Priority },
            TaskContinuationOptions.OnlyOnCanceled);

    public static Task OnFailure(this Task task, Action continuation, Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal) => task
       .ContinueWith(
            (_, o) => ((Dispatcher)((object[])o)[0]).Invoke((Action)((object[])o)[1], (DispatcherPriority)((object[])o)[2]),
            new object[] { dispatcher, continuation, Priority },
            TaskContinuationOptions.OnlyOnFaulted);

    public static Task<TResult> OnFailure<T, TResult>(this Task<T> task, Func<AggregateException, TResult> continuation, Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal) => task
       .ContinueWith(
            (t, o) => (TResult)((Dispatcher)((object[])o)[0]).Invoke((Delegate)((object[])o)[1], (DispatcherPriority)((object[])o)[2], t.Exception),
            new object[] { dispatcher, continuation, Priority },
            TaskContinuationOptions.OnlyOnFaulted);

    public static Task OnSuccessWPF(this Task task, Action continuation, DispatcherPriority Priority = DispatcherPriority.Normal) =>
        task.ContinueWith(
            (_, o) => ((Dispatcher)((object[])o)[0]).Invoke((Action)((object[])o)[1], (DispatcherPriority)((object[])o)[2]),
            new object[] { Application.Current.Dispatcher, continuation, Priority },
            TaskContinuationOptions.OnlyOnRanToCompletion);

    public static Task<TResult> OnSuccessWPF<TResult>(this Task task, Func<TResult> continuation, DispatcherPriority Priority = DispatcherPriority.Normal) =>
        task.ContinueWith(
            (_, o) => ((Dispatcher)((object[])o)[0]).Invoke((Func<TResult>)((object[])o)[1], (DispatcherPriority)((object[])o)[2]),
            new object[] { Application.Current.Dispatcher, continuation, Priority },
            TaskContinuationOptions.OnlyOnRanToCompletion);

    public static Task<TResult> OnSuccessWPF<T, TResult>(this Task<T> task, Func<T, TResult> continuation, DispatcherPriority Priority = DispatcherPriority.Normal) => task
       .ContinueWith(
            (t, o) => (TResult)((Dispatcher)((object[])o)[0]).Invoke((Delegate)((object[])o)[1], (DispatcherPriority)((object[])o)[2], t.Result),
            new object[] { Application.Current.Dispatcher, continuation, Priority },
            TaskContinuationOptions.OnlyOnRanToCompletion);

    public static Task OnCancelledWPF(this Task task, Action continuation, DispatcherPriority Priority = DispatcherPriority.Normal) => task
       .ContinueWith(
            (_, o) => ((Dispatcher)((object[])o)[0]).Invoke((Action)((object[])o)[1], (DispatcherPriority)((object[])o)[2]),
            new object[] { Application.Current.Dispatcher, continuation, Priority },
            TaskContinuationOptions.OnlyOnCanceled);

    public static Task OnFailure(this Task task, Action continuation, DispatcherPriority Priority = DispatcherPriority.Normal) => task
       .ContinueWith(
            (_, o) => ((Dispatcher)((object[])o)[0]).Invoke((Action)((object[])o)[1], (DispatcherPriority)((object[])o)[2]),
            new object[] { Application.Current.Dispatcher, continuation, Priority },
            TaskContinuationOptions.OnlyOnFaulted);

    public static Task<TResult> OnFailure<T, TResult>(this Task<T> task, Func<AggregateException, TResult> continuation, DispatcherPriority Priority = DispatcherPriority.Normal) => task
       .ContinueWith(
            (t, o) => (TResult)((Dispatcher)((object[])o)[0]).Invoke((Delegate)((object[])o)[1], (DispatcherPriority)((object[])o)[2], t.Exception),
            new object[] { Application.Current.Dispatcher, continuation, Priority },
            TaskContinuationOptions.OnlyOnFaulted);
}
