using MathCore.Annotations;
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Windows.Threading
{
    public static class DispatcherObjectExtensions
    {
        public static void Invoke<T>(this T obj, Action<T> action, DispatcherPriority priority = DispatcherPriority.Normal)
            where T : DispatcherObject
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));
            if (action is null) throw new ArgumentNullException(nameof(action));

            var obj_dispatcher = obj.Dispatcher;
            if (obj_dispatcher is null)
                action(obj);
            else
                obj_dispatcher.Invoke(action, priority, obj);
        }

        public static TValue GetValue<TObject, TValue>(
            this TObject obj, 
            Func<TObject, TValue> func,
            DispatcherPriority priority = DispatcherPriority.Normal)
            where TObject : DispatcherObject
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));
            if (func is null) throw new ArgumentNullException(nameof(func));

            var obj_dispatcher = obj.Dispatcher;
            return obj_dispatcher is null ? func(obj) : (TValue) obj_dispatcher.Invoke(func, priority, obj);
        }

        public static DispatcherAwaiter SwitchToContext<T>(this T obj) where T : DispatcherObject => obj.Dispatcher.GetAwaiter();
    }
}