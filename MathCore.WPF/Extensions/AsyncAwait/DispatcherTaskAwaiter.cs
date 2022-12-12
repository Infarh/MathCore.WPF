using System.Runtime.CompilerServices;
using System.Windows.Threading;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

public readonly ref struct TaskDispatcherAwaitable
{
    private readonly Dispatcher _Dispatcher;
    private readonly Task? _Task;
    private readonly DispatcherPriority _Priority;

    public TaskDispatcherAwaitable(Dispatcher Dispatcher, Task? Task = null, DispatcherPriority Priority = DispatcherPriority.Normal)
    {
        _Dispatcher = Dispatcher;
        _Task       = Task;
        _Priority   = Priority;
    }

    public TaskDispatcherAwaitable.TaskDispatcherAwaiter GetAwaiter() => new (_Dispatcher, _Task, _Priority);

    public readonly struct TaskDispatcherAwaiter : ICriticalNotifyCompletion
    {
        private readonly Task? _Task;
        private readonly DispatcherPriority _Priority;
        private readonly Dispatcher _Dispatcher;

        public bool IsCompleted => _Task?.IsCompleted ?? false;

        public TaskDispatcherAwaiter(Dispatcher Dispatcher, Task? Task = null, DispatcherPriority Priority = DispatcherPriority.Normal)
        {
            _Task       = Task;
            _Priority   = Priority;
            _Dispatcher = Dispatcher;
        }

        public void OnCompleted(Action continuation) => _Dispatcher.Invoke(continuation, _Priority);

        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

        public void GetResult() => _Task?.Wait();
    }
}
