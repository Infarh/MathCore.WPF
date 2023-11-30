using System.Runtime.CompilerServices;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Windows.Threading;

public static class DispatcherExtensions
{
    public static DispatcherAwaiter GetAwaiter(this Dispatcher dispatcher) => new(dispatcher);

    public static PriorityDispatcherAwaiter AwaitWithPriority(this Dispatcher dispatcher, DispatcherPriority Priority) => 
        dispatcher is null 
            ? throw new ArgumentNullException(nameof(dispatcher)) 
            : new PriorityDispatcherAwaiter(dispatcher, Priority);

    public static PriorityDispatcherAwaiter SwitchContext(this Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal) => 
        dispatcher is null 
            ? throw new ArgumentNullException(nameof(dispatcher)) 
            : new PriorityDispatcherAwaiter(dispatcher, Priority);
}

public readonly ref struct PriorityDispatcherAwaiter(Dispatcher dispatcher, DispatcherPriority Priority)
{
    public DispatcherAwaiter GetAwaiter() => new(dispatcher, Priority);
}

public readonly struct DispatcherAwaiter(Dispatcher dispatcher, DispatcherPriority Priority) : INotifyCompletion
{
    public DispatcherAwaiter(Dispatcher dispatcher) : this(dispatcher, DispatcherPriority.Normal) { }

    private readonly Dispatcher _Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

    public bool IsCompleted => _Dispatcher.CheckAccess();

    public void OnCompleted(Action continuation)
    {
        if (Priority == DispatcherPriority.Normal)
            _Dispatcher.Invoke(continuation);
        else
            _Dispatcher.Invoke(continuation, Priority);
    }

    public void GetResult() { }

    public DispatcherAwaiter GetAwaiter() => this;
}