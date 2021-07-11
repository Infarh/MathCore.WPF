using System;
using System.Globalization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MathCore.WPF.ConsoleTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sh = Channel.CreateBounded<int>(new BoundedChannelOptions(10)
            {
                AllowSynchronousContinuations = true,
                SingleReader = true,
                SingleWriter = true,
                FullMode = BoundedChannelFullMode.DropWrite
            });
            var writer = sh.Writer;
            var reader = sh.Reader;

            var processing_task = Task.Run(
                async () =>
                {
                    await foreach (var v in reader.ReadAllAsync())
                    {
                        Console.WriteLine("    {0}", v);
                        await Task.Delay(50);
                    }
                });

            for (var i = 0; i < 20; i++)
            {
                Console.WriteLine("{0}", i);
                await writer.WriteAsync(i);
            }

            writer.Complete();
            Console.WriteLine("Producer completed");

            await sh.Reader.Completion;
            Console.WriteLine("Reader completed");

            await processing_task;

            Console.WriteLine("Completed");
            Console.ReadLine();
        }
    }

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
}
