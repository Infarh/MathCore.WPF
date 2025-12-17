using System.ComponentModel;
using System.Windows.Threading;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace System.ComponentModel
{
    public static class PropertyChangedEventHandlerExtensions
    {
        /// <summary>
        /// Потоко-безопасный вызов обработчиков события PropertyChanged.
        /// </summary>
        /// <param name="Event">Обработчик события PropertyChanged.</param>
        /// <param name="sender">Объект-отправитель события.</param>
        /// <param name="PropertyName">Имена свойств, которые изменились.</param>
        public static void ThreadSafeInvoke(
            this PropertyChangedEventHandler? Event,
            object? sender,
            params string?[] PropertyName)
        {
            if (PropertyName is null) throw new ArgumentNullException(nameof(PropertyName));

            if (Event is null || PropertyName.Length == 0) return;

            var args = PropertyName.ToArray(name => new PropertyChangedEventArgs(name));
            foreach (var d in Event.GetInvocationList())
                switch (d.Target)
                {
                    case ISynchronizeInvoke { InvokeRequired: true } synchronize_invoke:
                        foreach (var arg in args) synchronize_invoke.Invoke(d, [sender, arg]);
                        break;
                    case DispatcherObject dispatcher_obj when !dispatcher_obj.CheckAccess():
                        foreach (var arg in args) dispatcher_obj.Dispatcher.Invoke(d, sender, arg);
                        break;
                    default:
                        foreach (var arg in args) d.DynamicInvoke(sender, arg);
                        break;
                }
        }

        /// <summary>
        /// Перегруженный метод потоко-безопасного вызова обработчиков события PropertyChanged,
        /// позволяющий передать одно имя свойства.
        /// </summary>
        /// <param name="Handler">Обработчик события PropertyChanged.</param>
        /// <param name="Sender">Объект-отправитель события.</param>
        /// <param name="PropertyName">Имя свойства, которое изменилось.</param>
        public static void ThreadSafeInvoke(
            this PropertyChangedEventHandler? Handler,
            object? Sender,
            string PropertyName)
        {
            if (PropertyName is null) throw new ArgumentNullException(nameof(PropertyName));
            if (Handler is null || PropertyName.Length == 0) return;
            var e = new PropertyChangedEventArgs(PropertyName);
            var invocation_list = Handler.GetInvocationList();
            var args = new[] { Sender, e };
            foreach (var d in invocation_list)
                switch (d.Target)
                {
                    case ISynchronizeInvoke { InvokeRequired: true } synchronize_invoke:
                        synchronize_invoke.Invoke(d, args);
                        break;
                    case DispatcherObject { Dispatcher: { } } dispatcher_obj when !dispatcher_obj.CheckAccess():
                        dispatcher_obj.Dispatcher.Invoke(d, args);
                        break;
                    default:
                        d.DynamicInvoke(args);
                        break;
                }
        }

        /// <summary>
        /// Метод для асинхронного (начального) потоко-безопасного вызова обработчиков события PropertyChanged.
        /// </summary>
        /// <param name="Handler">Обработчик события PropertyChanged.</param>
        /// <param name="Sender">Объект-отправитель события.</param>
        /// <param name="PropertyName">Имя свойства, которое изменилось.</param>
        public static void ThreadSafeBeginInvoke(
            this PropertyChangedEventHandler? Handler,
            object? Sender,
            string PropertyName)
        {
            if (PropertyName is null) throw new ArgumentNullException(nameof(PropertyName));
            if (Handler is null || PropertyName.Length == 0) return;
            var e = new PropertyChangedEventArgs(PropertyName);
            var invocation_list = Handler.GetInvocationList();
            var args = new[] { Sender, e };
            foreach (var d in invocation_list)
                switch (d.Target)
                {
                    case ISynchronizeInvoke { InvokeRequired: true } synchronize_invoke:
                        synchronize_invoke.BeginInvoke(d, args);
                        break;
                    case DispatcherObject { Dispatcher: { } } dispatcher_obj when !dispatcher_obj.CheckAccess():
                        dispatcher_obj.Dispatcher.BeginInvoke(d, DispatcherPriority.DataBind, args);
                        break;
                    default:
                    {
                        var @delegate = d;
                        ((Action<object[]>)(a => @delegate.DynamicInvoke(a))).BeginInvoke(args!, null, null);
                        break;
                    }
                }
        }
    }
}
namespace System.Collections.Specialized
{
    public static class NotifyCollectionChangedEventHandlerExtensions
    {
        /// <summary>
        /// Потоко-безопасный вызов обработчиков события NotifyCollectionChanged.
        /// </summary>
        /// <param name="Handler">Обработчик события NotifyCollectionChanged.</param>
        /// <param name="Sender">Объект-отправитель события.</param>
        /// <param name="E">Аргументы события NotifyCollectionChanged.</param>
        public static void ThreadSafeInvoke(
            this NotifyCollectionChangedEventHandler? Handler,
            object? Sender,
            NotifyCollectionChangedEventArgs E)
        {
            if (E is null) throw new ArgumentNullException(nameof(E));
            if (Handler is null) return;
            var invocation_list = Handler.GetInvocationList();
            var args = new[] { Sender, E };
            foreach (var d in invocation_list)
            {
                var o = d.Target;
                switch (o)
                {
                    case ISynchronizeInvoke { InvokeRequired: true } synchronize_invoke:
                        synchronize_invoke.Invoke(d, args);
                        break;
                    case DispatcherObject { Dispatcher: { } } dispatcher_obj when !dispatcher_obj.CheckAccess():
                        dispatcher_obj.Dispatcher.Invoke(d, args);
                        break;
                    default:
                        d.DynamicInvoke(args);
                        break;
                }
            }
        }

        /// <summary>
        /// Метод для асинхронного (начального) потоко-безопасного вызова обработчиков события NotifyCollectionChanged.
        /// </summary>
        /// <param name="Handler">Обработчик события NotifyCollectionChanged.</param>
        /// <param name="Sender">Объект-отправитель события.</param>
        /// <param name="E">Аргументы события NotifyCollectionChanged.</param>
        public static void ThreadSafeBeginInvoke(
            this NotifyCollectionChangedEventHandler? Handler,
            object? Sender,
            NotifyCollectionChangedEventArgs E)
        {
            if (E is null) throw new ArgumentNullException(nameof(E));
            if (Handler is null) return;
            var invocation_list = Handler.GetInvocationList();
            var args = new[] { Sender, E };
            foreach (var d in invocation_list)
            {
                var o = d.Target;
                switch (o)
                {
                    case ISynchronizeInvoke { InvokeRequired: true } synchronize_invoke:
                        synchronize_invoke.BeginInvoke(d, args);
                        break;
                    case DispatcherObject { Dispatcher: { } } dispatcher_obj when !dispatcher_obj.CheckAccess():
                        dispatcher_obj.Dispatcher.BeginInvoke(d, args);
                        break;
                    default:
                    {
                        var @delegate = d;
                        ((Action<object[]>)(a => @delegate.DynamicInvoke(a))).BeginInvoke(args!, null, null);
                        break;
                    }
                }
            }
        }
    }
}