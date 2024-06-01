using System.Runtime.CompilerServices;
using System.Windows.Threading;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

public readonly ref struct TaskDispatcherAwaitable(
    Dispatcher Dispatcher,
    Task? Task = null,
    DispatcherPriority Priority = DispatcherPriority.Normal)
{
    public TaskDispatcherAwaiter GetAwaiter() => new (Dispatcher, Task, Priority);

    public readonly struct TaskDispatcherAwaiter(
        Dispatcher Dispatcher,
        Task? Task = null,
        DispatcherPriority Priority = DispatcherPriority.Normal)
        : ICriticalNotifyCompletion
    {
        public bool IsCompleted => Task?.IsCompleted ?? false;

        public void OnCompleted(Action continuation) => Dispatcher.Invoke(continuation, Priority);

        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

        public void GetResult() => Task?.Wait();
    }
}
