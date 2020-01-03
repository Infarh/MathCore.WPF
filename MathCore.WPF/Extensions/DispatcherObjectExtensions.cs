using MathCore.Annotations;
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Windows.Threading
{
    public static class DispatcherObjectExtensions
    {
        public static void Invoke<T>([NotNull] this T obj, [NotNull] Action<T> action, DispatcherPriority priority = DispatcherPriority.Normal)
            where T : DispatcherObject
        {
            var obj_dispatcher = obj.Dispatcher;
            if (obj_dispatcher is null)
                action(obj);
            else
                obj_dispatcher.Invoke(action, priority, obj);
        }

        public static TValue GetValue<TObject, TValue>([NotNull] this TObject obj, [NotNull] Func<TObject, TValue> func,
            DispatcherPriority priority = DispatcherPriority.Normal)
            where TObject : DispatcherObject
        {
            var obj_dispatcher = obj.Dispatcher;
            return obj_dispatcher is null ? func(obj) : (TValue) obj_dispatcher.Invoke(func, priority, obj);
        }
    }
}