using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

namespace System.Threading.Tasks
{
    public static class WPFTaskEx
    {
        [NotNull] public static DispatcherAwaiter ToUIContext() => (Application.Current.Dispatcher ?? throw new InvalidOperationException("Диспетчер для приложения не определён")).GetAwaiter()!;

        /// <summary>
        /// Метод вызывает завершение работы текущего метода в текущем контексте синхронизации и возвращает задачу, результатом которой является диспетчер исходного контекста синхронизации
        /// </summary>
        /// <returns>Задача, возвращающая диспетчер исходного контекста синхронизации в одном из потоков из пула потоков</returns>
        [ItemNotNull]
        public static async Task<Dispatcher> FromDispatcherAsync()
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            await TaskEx.YieldAsync();
            return dispatcher;
        }

        /// <summary>Waits for the task to complete execution, pumping in the meantime.</summary>
        /// <param name="task">The task for which to wait.</param>
        /// <remarks>This method is intended for usage with Windows Presentation Foundation.</remarks>
        public static void WaitWithPumping([NotNull] this Task task)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));

            var nested_frame = new DispatcherFrame();
            task.ContinueWith(_ => nested_frame.Continue = false);
            Dispatcher.PushFrame(nested_frame);
            task.Wait();
        }
    }
}
namespace System.Windows.Threading
{
    public static class DispatcherExtensions
    {
        [NotNull] public static DispatcherAwaiter GetAwaiter([NotNull] this Dispatcher dispatcher) => new DispatcherAwaiter(dispatcher);
    }

    public class DispatcherAwaiter : INotifyCompletion
    {
        [NotNull] private readonly Dispatcher _Dispatcher;

        public bool IsCompleted => _Dispatcher.CheckAccess();

        public DispatcherAwaiter([NotNull] Dispatcher dispatcher) => _Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

        public void OnCompleted([NotNull] Action continuation) => _Dispatcher.Invoke(continuation);

        public void GetResult() { }
    }
}
