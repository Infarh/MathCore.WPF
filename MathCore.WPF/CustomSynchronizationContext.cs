using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MathCore.WPF
{
    /// <summary></summary>
    /// <example>
    /// static void Main(string[] args)
    /// {
    ///    var syncContext = new CustomSynchronizationContext();
    ///    try
    ///    {
    ///       syncContext.Send(o => { throw new Exception("TestException"); }, null);
    ///    } catch(Exception ex)
    ///    {
    ///        Console.WriteLine(ex.Message);
    ///    }
    /// }
    /// -----
    /// static void Main(string[] args)
    /// {
    ///     var syncContext = new CustomSynchronizationContext();
    ///     syncContext.Post(TestAsyncMethod, null);
    /// }
    /// 
    /// async static void TestAsyncMethod(object obj)
    /// {
    ///     Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
    ///     await Task.Factory.StartNew(() => Console.WriteLine(Thread.CurrentThread.ManagedThreadId));
    ///     Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
    /// }
    /// </example>
    [Copyright("Сергей Асеев @Serg046", url = "http://habrahabr.ru/post/269985/")]
    public class CustomSynchronizationContext : SynchronizationContext, IDisposable
    {
        private readonly AutoResetEvent _WorkerResetEvent;
        private readonly ConcurrentQueue<WorkItem> _WorkItems;
        private readonly Thread _Thread;

        public CustomSynchronizationContext()
        {
            _WorkerResetEvent = new AutoResetEvent(false);
            _WorkItems = new ConcurrentQueue<WorkItem>();
            _Thread = new Thread(DoWork);
            _Thread.Start();
        }

        private void DoWork()
        {
            SetSynchronizationContext(this);

            while(!_Disposed)
            {
                while(_WorkItems.TryDequeue(out var work_item))
                    work_item.Execute();

                //Note: race condition here
                _WorkerResetEvent.Reset();
                _WorkerResetEvent.WaitOne();
            }
        }

        public override void Send(SendOrPostCallback post, object? state)
        {
            if(Thread.CurrentThread == _Thread) post(state);
            else
            {
                using var reset_event = new AutoResetEvent(false);
                var work = new SynchronousWorkItem(post, state, reset_event);
                _WorkItems.Enqueue(work);
                _WorkerResetEvent.Set();

                reset_event.WaitOne();
                if(work.Error is { } error)
                    throw new InvalidOperationException("Ошибка асинхронной операции", error);
            }
        }

        public override void Post(SendOrPostCallback d, object? state)
        {
            _WorkItems.Enqueue(new AsynchronousWorkItem(d, state));
            _WorkerResetEvent.Set();
        }

        private volatile bool _Disposed;
        public void Dispose()
        {
            if(_Disposed) return;
            _WorkerResetEvent.Dispose();
            _Disposed = true;
        }

        private abstract class WorkItem
        {
            protected readonly SendOrPostCallback Callback;
            protected readonly object? State;

            protected WorkItem(SendOrPostCallback Callback, object? state)
            {
                this.Callback = Callback;
                State = state;
            }

            public abstract void Execute();
        }

        private sealed class SynchronousWorkItem : WorkItem
        {
            private readonly AutoResetEvent _SyncObject;

            public Exception? Error { get; private set; }

            public SynchronousWorkItem(SendOrPostCallback Callback, object? state, AutoResetEvent Reset) : base(Callback, state) => _SyncObject = Reset;

            public override void Execute()
            {
                try { Callback(State); } catch(Exception error) { Error = error; }
                _SyncObject.Set();
            }
        }

        private sealed class AsynchronousWorkItem : WorkItem
        {
            public AsynchronousWorkItem(SendOrPostCallback Callback, object? state)
                : base(Callback, state)
            { }

            public override void Execute() => Callback(State);
        }
    }
}
