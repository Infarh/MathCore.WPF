using System.Windows;
using System.Windows.Threading;

namespace MathCore.WPF.Extensions.AsyncAwait;

public static class ValueTaskEx
{
    public static async ValueTask OnSuccess(this ValueTask task, Action action, bool SaveContext = false)
    {
        if(SaveContext)
            await task;
        else
            await task.ConfigureAwait(false);
        action();
    }

    public static async ValueTask OnSuccess(this ValueTask task, Action action, TaskScheduler Scheduler)
    {
        await task.ConfigureAwait(false);
        await Scheduler.SwitchContext();
        action();
    }

    public static async ValueTask OnSuccess(this ValueTask task, Action action, SynchronizationContext Context)
    {
        await task.ConfigureAwait(false);
        await Context.SwitchContext();
        action();
    }

    public static async ValueTask OnSuccess(this ValueTask task, Action action, Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal)
    {
        await task.ConfigureAwait(false);
        await dispatcher.SwitchContext(Priority);
        action();
    }

    public static ValueTask OnSuccessWPF(this ValueTask task, Action action, DispatcherPriority Priority = DispatcherPriority.Normal) =>
        task.OnSuccess(action, Application.Current.Dispatcher, Priority);

    public static async ValueTask OnSuccess<T>(this ValueTask<T> task, Action<T> action, bool SaveContext = false)
    {
        var result = SaveContext
            ? await task
            : await task.ConfigureAwait(false);
        action(result);
    }

    public static async ValueTask OnSuccess<T>(this ValueTask<T> task, Action<T> action, TaskScheduler Scheduler)
    {
        var result = await task.ConfigureAwait(false);
        await Scheduler.SwitchContext();
        action(result);
    }

    public static async ValueTask OnSuccess<T>(this ValueTask<T> task, Action<T> action, SynchronizationContext Context)
    {
        var result = await task.ConfigureAwait(false);
        await Context.SwitchContext();
        action(result);
    }

    public static async ValueTask OnSuccess<T>(this ValueTask<T> task, Action<T> action, Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal)
    {
        var result = await task.ConfigureAwait(false);
        await dispatcher.SwitchContext(Priority);
        action(result);
    }

    public static ValueTask OnSuccessWPF<T>(this ValueTask<T> task, Action<T> action, DispatcherPriority Priority = DispatcherPriority.Normal) =>
        task.OnSuccess(action, Application.Current.Dispatcher, Priority);


    public static async ValueTask<TResult> OnSuccess<TResult>(this ValueTask task, Func<TResult> action, bool SaveContext = false)
    {
        if(SaveContext)
            await task;
        else
            await task.ConfigureAwait(false);
        return action();
    }

    public static async ValueTask<TResult> OnSuccess<TResult>(this ValueTask task, Func<TResult> action, TaskScheduler Scheduler)
    {
        await task.ConfigureAwait(false);
        await Scheduler.SwitchContext();
        return action();
    }

    public static async ValueTask<TResult> OnSuccess<TResult>(this ValueTask task, Func<TResult> action, SynchronizationContext Context)
    {
        await task.ConfigureAwait(false);
        await Context.SwitchContext();
        return action();
    }

    public static async ValueTask<TResult> OnSuccess<TResult>(this ValueTask task, Func<TResult> action, Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal)
    {
        await task.ConfigureAwait(false);
        await dispatcher.SwitchContext(Priority);
        return action();
    }

    public static ValueTask<TResult> OnSuccessWPF<TResult>(
        this ValueTask task,
        Func<TResult> action,
        DispatcherPriority Priority = DispatcherPriority.Normal) =>
        task.OnSuccess(action, Application.Current.Dispatcher, Priority);


    public static async ValueTask<TResult> OnSuccess<T, TResult>(this ValueTask<T> task, Func<T, TResult> action, bool SaveContext = false)
    {
        var result = SaveContext
            ? await task
            : await task.ConfigureAwait(false);
        return action(result);
    }

    public static async ValueTask<TResult> OnSuccess<T, TResult>(this ValueTask<T> task, Func<T, TResult> action, TaskScheduler Scheduler)
    {
        var result = await task.ConfigureAwait(false);
        await Scheduler.SwitchContext();
        return action(result);
    }

    public static async ValueTask<TResult> OnSuccess<T, TResult>(this ValueTask<T> task, Func<T, TResult> action, SynchronizationContext Context)
    {
        var result = await task.ConfigureAwait(false);
        await Context.SwitchContext();
        return action(result);
    }

    public static async ValueTask<TResult> OnSuccess<T, TResult>(this ValueTask<T> task, Func<T, TResult> action, Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal)
    {
        var result = await task.ConfigureAwait(false);
        await dispatcher.SwitchContext(Priority);
        return action(result);
    }

    public static ValueTask<TResult> OnSuccessWPF<T, TResult>(
        this ValueTask<T> task,
        Func<T, TResult> action,
        DispatcherPriority Priority = DispatcherPriority.Normal) =>
        task.OnSuccess(action, Application.Current.Dispatcher, Priority);


    public static async ValueTask OnFailure(this ValueTask task, Action action, bool SaveContext = false)
    {
        try
        {
            if(SaveContext)
                await task;
            else
                await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
        catch (Exception)
        {
            action();
        }
    }

    public static async ValueTask OnFailure(this ValueTask task, Action action, TaskScheduler Scheduler)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
        catch (Exception)
        {
            await Scheduler.SwitchContext();
            action();
        }
    }

    public static async ValueTask OnFailure(this ValueTask task, Action action, SynchronizationContext Context)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
        catch (Exception)
        {
            await Context.SwitchContext();
            action();
        }
    }

    public static async ValueTask OnFailure(this ValueTask task, Action action, Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
        catch (Exception)
        {
            await dispatcher.SwitchContext(Priority);
            action();
        }
    }

    public static ValueTask OnFailureWPF(this ValueTask task, Action action, DispatcherPriority Priority = DispatcherPriority.Normal) =>
        task.OnFailure(action, Application.Current.Dispatcher, Priority);

    public static async ValueTask OnFailure(this ValueTask task, Action<Exception> action, bool SaveContext = false)
    {
        try
        {
            if(SaveContext)
                await task;
            else
                await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            action(e);
        }
    }

    public static async ValueTask OnFailure(this ValueTask task, Action<Exception> action, TaskScheduler Scheduler)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            await Scheduler.SwitchContext();
            action(e);
        }
    }

    public static async ValueTask OnFailure(this ValueTask task, Action<Exception> action, SynchronizationContext Context)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            await Context.SwitchContext();
            action(e);
        }
    }

    public static async ValueTask OnFailure(this ValueTask task, Action<Exception> action, Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            await dispatcher.SwitchContext(Priority);
            action(e);
        }
    }

    public static ValueTask OnFailureWPF(this ValueTask task, Action<Exception> action, DispatcherPriority Priority = DispatcherPriority.Normal) =>
        task.OnFailure(action, Application.Current.Dispatcher, Priority);

    public static async ValueTask OnCancelled(this ValueTask task, Action action, bool SaveContext = false)
    {
        try
        {
            if(SaveContext)
                await task;
            else
                await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            action();
        }
    }

    public static async ValueTask OnCancelled(this ValueTask task, Action action, TaskScheduler Scheduler)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            await Scheduler.SwitchContext();
            action();
        }
    }

    public static async ValueTask OnCancelled(this ValueTask task, Action action, SynchronizationContext Context)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            await Context.SwitchContext();
            action();
        }
    }

    public static async ValueTask OnCancelled(this ValueTask task, Action action, Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            await dispatcher.SwitchContext(Priority);
            action();
        }
    }

    public static ValueTask OnCancelledWPF(this ValueTask task, Action action, DispatcherPriority Priority = DispatcherPriority.Normal) =>
        task.OnCancelled(action, Application.Current.Dispatcher, Priority);
}
