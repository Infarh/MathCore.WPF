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

    public static PriorityDispatcherAwaiter ChangeContext(this Dispatcher dispatcher, DispatcherPriority Priority = DispatcherPriority.Normal) => 
        dispatcher is null 
            ? throw new ArgumentNullException(nameof(dispatcher)) 
            : new PriorityDispatcherAwaiter(dispatcher, Priority);
}

public readonly ref struct PriorityDispatcherAwaiter
{
    private readonly Dispatcher _Dispatcher;
    private readonly DispatcherPriority _Priority;

    public PriorityDispatcherAwaiter(Dispatcher dispatcher, DispatcherPriority Priority)
    {
        _Dispatcher = dispatcher;
        _Priority = Priority;
    }

    public DispatcherAwaiter GetAwaiter() => new(_Dispatcher, _Priority);
}

public readonly struct DispatcherAwaiter : INotifyCompletion
{
    private readonly DispatcherPriority _Priority;
    private readonly Dispatcher _Dispatcher;

    public bool IsCompleted => _Dispatcher.CheckAccess();

    public DispatcherAwaiter(Dispatcher dispatcher)
    {
        _Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _Priority = DispatcherPriority.Normal;
    }

    public DispatcherAwaiter(Dispatcher dispatcher, DispatcherPriority Priority)
    {
        _Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _Priority = Priority;
    }

    public void OnCompleted(Action continuation)
    {
        if (_Priority == DispatcherPriority.Normal)
            _Dispatcher.Invoke(continuation);
        else
            _Dispatcher.Invoke(continuation, _Priority);
    }

    public void GetResult() { }

    public DispatcherAwaiter GetAwaiter() => this;
}