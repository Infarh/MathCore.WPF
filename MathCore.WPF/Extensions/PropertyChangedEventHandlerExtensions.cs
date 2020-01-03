using System.Linq;
using System.Windows.Threading;
using MathCore.Annotations;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace System.ComponentModel
{
    public static class PropertyChangedEventHandlerExtensions
    {
        [Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static void ThreadSafeInvoke(
            [CanBeNull] this PropertyChangedEventHandler Event,
            [CanBeNull] object sender,
            [NotNull, ItemCanBeNull] params string[] PropertyName)
        {
            if (PropertyName is null) throw new ArgumentNullException(nameof(PropertyName));
            if (Event is null || PropertyName.Length == 0) return;
            var args = PropertyName.ToArray(name => new PropertyChangedEventArgs(name));
            foreach (var d in Event.GetInvocationList())
                switch (d.Target)
                {
                    case ISynchronizeInvoke synchronize_invoke when synchronize_invoke.InvokeRequired:
                        foreach (var arg in args) synchronize_invoke.Invoke(d, new[] { sender, arg });
                        break;
                    case DispatcherObject dispatcher_obj when !dispatcher_obj.CheckAccess():
                        foreach (var arg in args) dispatcher_obj.Dispatcher.Invoke(d, arg);
                        break;
                    default:
                        foreach (var arg in args) d.DynamicInvoke(sender, arg);
                        break;
                }
        }

        public static void ThreadSafeInvoke(
            [CanBeNull] this PropertyChangedEventHandler Handler, 
            [CanBeNull] object Sender,
            [NotNull] 
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
                    case ISynchronizeInvoke synchronize_invoke when synchronize_invoke.InvokeRequired:
                        synchronize_invoke.Invoke(d, args);
                        break;
                    case DispatcherObject dispatcher_obj when !dispatcher_obj.CheckAccess() && dispatcher_obj.Dispatcher != null:
                        dispatcher_obj.Dispatcher.Invoke(d, args);
                        break;
                    default:
                        d.DynamicInvoke(args);
                        break;
                }
        }

        public static void ThreadSafeBeginInvoke(
            [CanBeNull] this PropertyChangedEventHandler Handler,
            [CanBeNull] object Sender,
            [NotNull] string PropertyName)
        {
            if (PropertyName is null) throw new ArgumentNullException(nameof(PropertyName));
            if (Handler is null || PropertyName.Length == 0) return;
            var e = new PropertyChangedEventArgs(PropertyName);
            var invocation_list = Handler.GetInvocationList();
            var args = new[] { Sender, e };
            foreach (var d in invocation_list)
                switch (d.Target)
                {
                    case ISynchronizeInvoke synchronize_invoke when synchronize_invoke.InvokeRequired:
                        synchronize_invoke.BeginInvoke(d, args);
                        break;
                    case DispatcherObject dispatcher_obj when !dispatcher_obj.CheckAccess() && dispatcher_obj.Dispatcher != null:
                        dispatcher_obj.Dispatcher.BeginInvoke(d, DispatcherPriority.DataBind, args);
                        break;
                    default:
                        {
                            var @delegate = d;
                            ((Action<object[]>)(a => @delegate.DynamicInvoke(a))).BeginInvoke(args, null, null);
                            break;
                        }
                }
        }
    }
}
namespace System.Collections.Specialized
{
    using ComponentModel;
    public static class NotifyCollectionChangedEventHandlerExtensions
    {
        public static void ThreadSafeInvoke(
            [CanBeNull] this NotifyCollectionChangedEventHandler Handler,
            [CanBeNull] object Sender,
            [NotNull] NotifyCollectionChangedEventArgs E)
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
                    case ISynchronizeInvoke synchronize_invoke when synchronize_invoke.InvokeRequired:
                        synchronize_invoke.Invoke(d, args);
                        break;
                    case DispatcherObject dispatcher_obj when !dispatcher_obj.CheckAccess() && dispatcher_obj.Dispatcher != null:
                        dispatcher_obj.Dispatcher.Invoke(d, args);
                        break;
                    default:
                        d.DynamicInvoke(args);
                        break;
                }
            }
        }

        public static void ThreadSafeBeginInvoke(
            [CanBeNull] this NotifyCollectionChangedEventHandler Handler,
            [CanBeNull] object Sender,
            [NotNull] NotifyCollectionChangedEventArgs E)
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
                    case ISynchronizeInvoke synchronize_invoke when synchronize_invoke.InvokeRequired:
                        synchronize_invoke.BeginInvoke(d, args);
                        break;
                    case DispatcherObject dispatcher_obj when !dispatcher_obj.CheckAccess() && dispatcher_obj.Dispatcher != null:
                        dispatcher_obj.Dispatcher.BeginInvoke(d, args);
                        break;
                    default:
                        {
                            var @delegate = d;
                            ((Action<object[]>)(a => @delegate.DynamicInvoke(a))).BeginInvoke(args, null, null);
                            break;
                        }
                }
            }
        }
    }
}