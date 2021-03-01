using System.Runtime.CompilerServices;

using MathCore.Annotations;

namespace System.Windows.Threading
{
    public static class DispatcherExtensions
    {
        [NotNull] public static DispatcherAwaiter GetAwaiter([NotNull] this Dispatcher dispatcher) => new(dispatcher);

        public static PriorityDispatcherAwaiter AwaitWithPriority(this Dispatcher dispatcher, DispatcherPriority Priority)
        {
            if (dispatcher is null) throw new ArgumentNullException(nameof(dispatcher));
            return new PriorityDispatcherAwaiter(dispatcher, Priority);
        }
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
        [NotNull] private readonly Dispatcher _Dispatcher;

        public bool IsCompleted => _Dispatcher.CheckAccess();

        public DispatcherAwaiter([NotNull] Dispatcher dispatcher)
        {
            _Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _Priority = DispatcherPriority.Normal;
        }

        public DispatcherAwaiter([NotNull] Dispatcher dispatcher, DispatcherPriority Priority)
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
    }
}
