using System.ComponentModel;
using System.Windows.Threading;

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Статический класс-расширение для потокобезопасного вызова событий</summary>
public static class EventHandlerExtensions
{
    /// <summary>Потокобезопасный вызов события EventHandler</summary>
    /// <param name="Event">Событие</param>
    /// <param name="Sender">Источник события</param>
    /// <param name="E">Аргументы события</param>
    public static void ThreadSafeInvoke(this EventHandler? Event, object? Sender, EventArgs? E)
        => ThreadSafeInvokeInternal(Event, Sender, E ?? EventArgs.Empty);

    /// <summary>Потокобезопасный вызов события EventHandler с аргументом типа TArg</summary>
    /// <typeparam name="TArg">Тип аргумента события</typeparam>
    /// <param name="Event">Событие</param>
    /// <param name="Sender">Источник события</param>
    /// <param name="E">Аргументы события</param>
    public static void ThreadSafeInvoke<TArg>(this EventHandler<TArg>? Event, object? Sender, TArg E)
        where TArg : EventArgs
        => ThreadSafeInvokeInternal(Event, Sender, E);

    /// <summary>Универсальный потокобезопасный вызов делегатов событий</summary>
    /// <param name="EventDelegate">Делегат события</param>
    /// <param name="Sender">Источник события</param>
    /// <param name="E">Аргументы события</param>
    private static void ThreadSafeInvokeInternal(Delegate? EventDelegate, object? Sender, object E)
    {
        if (EventDelegate.GetInvocationList() is not { Length: > 0 } delegates) return;
        var args = new[] { Sender, E };
        List<(Delegate d, Exception error)>? exceptions = null;
        foreach (var d in delegates)
            try
            {
                switch (d.Target)
                {
                    case ISynchronizeInvoke { InvokeRequired: true } sync:
                        sync.Invoke(d, args); // Потокобезопасный вызов через ISynchronizeInvoke
                        break;
                    case DispatcherObject dispatcher_obj when !dispatcher_obj.CheckAccess():
                        dispatcher_obj.Dispatcher?.Invoke(() =>
                            d.DynamicInvoke(args)); // Потокобезопасный вызов через Dispatcher
                        break;
                    default:
                        d.DynamicInvoke(args); // Обычный вызов
                        break;
                }
            }
            catch (Exception error) when (delegates.Length > 1)
            {
                (exceptions ??= []).Add((d, error));
            }

        if (exceptions is not { Count: > 0 }) return;

        throw new AggregateException(
            $"Ошибка при выполнении обработчика события\r\n{exceptions.Select(d => $"    {d.d.Target}.{d.d.Method.Name} : {d.error.Message}").ToSeparatedStr("\r\n")}",
            [.. exceptions.Select(d => d.error)]);
    }
}
