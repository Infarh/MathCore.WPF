namespace MathCore.WPF.ConsoleTest;

internal class ThreadProcessor<T> : IDisposable where T : class
{
    private readonly Action<T?> _Consumer;
    private readonly AutoResetEvent _ProducerLock = new(true);
    private readonly AutoResetEvent _ConsumerLock = new(false);
    private volatile T? _Value;
    private bool _Disposed;

    public ThreadProcessor(Action<T?> Consumer)
    {
        _Consumer = Consumer;
        var consumer_thread = new Thread(Processing)
        {
            IsBackground = true,
            Name = $"Процессор даных {typeof(T)}"
        };
        consumer_thread.Start();
    }

    public void Process(T value)
    {
        if (_Disposed) return;
        _ProducerLock.WaitOne();
        if (_Disposed) return;
        _Value = value;
        _ConsumerLock.Set();
    }

    private void Processing()
    {
        while (!_Disposed)
        {
            _ConsumerLock.WaitOne();
            if (_Disposed) return;
            _Consumer(_Value);
            _ProducerLock.Set();
        }
    }

    public void Dispose()
    {
        if (_Disposed) return;
        _Disposed = true;
        _ConsumerLock.Set();
        _ProducerLock.Set();
    }
}