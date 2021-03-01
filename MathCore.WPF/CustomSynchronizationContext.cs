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
            _Thread.Start(this);
        }

        private void DoWork(object obj)
        {
            SetSynchronizationContext(obj as SynchronizationContext);

            while(true)
            {
                while(_WorkItems.TryDequeue(out var lv_WorkItem))
                    lv_WorkItem.Execute();

                //Note: race condition here
                _WorkerResetEvent.Reset();
                _WorkerResetEvent.WaitOne();
            }
        }

        public override void Send(SendOrPostCallback post, object state)
        {
            if(Thread.CurrentThread == _Thread) post(state);
            else
            {
                using var reset_event = new AutoResetEvent(false);
                var wi_execution_info = new WorkItemExecutionInfo();
                _WorkItems.Enqueue(new SynchronousWorkItem(post, state, reset_event, ref wi_execution_info));
                _WorkerResetEvent.Set();

                reset_event.WaitOne();
                if(wi_execution_info.HasException)
                    throw wi_execution_info.Exception;
            }
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            _WorkItems.Enqueue(new AsynchronousWorkItem(d, state));
            _WorkerResetEvent.Set();
        }

        public void Dispose()
        {
            _WorkerResetEvent.Dispose();
            _Thread.Abort();
        }

        private sealed class WorkItemExecutionInfo
        {
            public bool HasException => Exception != null;
            public Exception Exception { get; set; }
        }

        private abstract class WorkItem
        {
            protected readonly SendOrPostCallback SendOrPostCallback;
            protected readonly object State;

            protected WorkItem(SendOrPostCallback sendOrPostCallback, object state)
            {
                SendOrPostCallback = sendOrPostCallback;
                State = state;
            }

            public abstract void Execute();
        }

        private sealed class SynchronousWorkItem : WorkItem
        {
            private readonly AutoResetEvent _SyncObject;
            private readonly WorkItemExecutionInfo _WorkItemExecutionInfo;

            public SynchronousWorkItem(SendOrPostCallback SendOrPostCallback, object state, AutoResetEvent ResetEvent,
                ref WorkItemExecutionInfo WorkItemExecutionInfo) : base(SendOrPostCallback, state)
            {
                if(WorkItemExecutionInfo is null)
                    throw new NullReferenceException(nameof(WorkItemExecutionInfo));

                _SyncObject = ResetEvent;
                _WorkItemExecutionInfo = WorkItemExecutionInfo;
            }

            public override void Execute()
            {
                try { SendOrPostCallback(State); } catch(Exception error) { _WorkItemExecutionInfo.Exception = error; }
                _SyncObject.Set();
            }
        }

        private sealed class AsynchronousWorkItem : WorkItem
        {
            public AsynchronousWorkItem(SendOrPostCallback SendOrPostCallback, object state)
                : base(SendOrPostCallback, state)
            { }

            public override void Execute() => SendOrPostCallback(State);
        }
    }
}
