using System.ComponentModel;
using System.Windows.Threading;

// ReSharper disable once CheckNamespace
namespace System;

public static class EventHandlerExtensions
{
    public static void ThreadSafeInvoke(this EventHandler? Event, object? Sender, EventArgs? E)
    {
        if (Event is null) return;

        var e = E ?? EventArgs.Empty;
        object[]? args = null;
        foreach (var d in Event.GetInvocationList())
            switch (d.Target)
            {
                case ISynchronizeInvoke { InvokeRequired: true } synchronize_invoke:
                    synchronize_invoke.Invoke(d, args ??= new[] { Sender, e });
                    break;

                case DispatcherObject dispatcher_obj when !dispatcher_obj.CheckAccess():
                    dispatcher_obj.Dispatcher.Invoke(d, Sender, e);
                    break;

                default:
                    d.DynamicInvoke(Sender, e);
                    break;
            }
    }

    public static void ThreadSafeInvoke<TArg>(this EventHandler<TArg>? Event, object? Sender, TArg E)
        where TArg : EventArgs
    {
        if (Event is null) return;

        object[]? args = null;
        foreach (var d in Event.GetInvocationList())
            switch (d.Target)
            {
                case ISynchronizeInvoke { InvokeRequired: true } synchronize_invoke:
                    synchronize_invoke.Invoke(d, args ??= new[] { Sender, E });
                    break;

                case DispatcherObject dispatcher_obj when !dispatcher_obj.CheckAccess():
                    dispatcher_obj.Dispatcher.Invoke(d, Sender, E);
                    break;

                default:
                    d.DynamicInvoke(Sender, E);
                    break;
            }
    }
}
