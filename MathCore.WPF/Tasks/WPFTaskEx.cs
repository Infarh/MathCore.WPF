using System.Windows;
using System.Windows.Threading;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

public static class WPFTaskEx
{
    public static DispatcherAwaiter ToUIContext() => (Application.Current.Dispatcher ?? throw new InvalidOperationException("Диспетчер для приложения не определён")).GetAwaiter()!;

    /// <summary>Метод вызывает завершение работы текущего метода в текущем контексте синхронизации и возвращает задачу, результатом которой является диспетчер исходного контекста синхронизации</summary>
    /// <returns>Задача, возвращающая диспетчер исходного контекста синхронизации в одном из потоков из пула потоков</returns>
    public static async Task<Dispatcher> FromDispatcherAsync()
    {
        var dispatcher = Dispatcher.CurrentDispatcher;
        await TaskEx.YieldAsync();
        return dispatcher;
    }

    /// <summary>Waits for the task to complete execution, pumping in the meantime.</summary>
    /// <param name="task">The task for which to wait.</param>
    /// <remarks>This method is intended for usage with Windows Presentation Foundation.</remarks>
    public static void WaitWithPumping(this Task task)
    {
        if (task is null) throw new ArgumentNullException(nameof(task));

        var nested_frame = new DispatcherFrame();
#pragma warning disable CA2008 // Не создавайте задачи без передачи TaskScheduler
        _ = task.ContinueWith(_ => nested_frame.Continue = false);
#pragma warning restore CA2008 // Не создавайте задачи без передачи TaskScheduler
        Dispatcher.PushFrame(nested_frame);
        // ReSharper disable once AsyncConverter.AsyncWait
        task.Wait();
    }
}