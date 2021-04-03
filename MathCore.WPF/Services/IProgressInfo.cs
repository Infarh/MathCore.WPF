using System;
using System.Threading;

namespace MathCore.WPF.Services
{
    /// <summary>Управление диалогом</summary>
    public interface IProgressInfo : IDisposable
    {
        /// <summary>Событие возникает в момент вызова отмены операции в диалоге (вызове команды отмены)</summary>
        event EventHandler? Cancelled;

        /// <summary>Объект уведомления о прогрессе операции</summary>
        IProgress<double> Progress { get; }

        /// <summary>Объект передачи информационных сообщений</summary>
        IProgress<string> Information { get; }

        /// <summary>Объект передачи статусных сообщений</summary>
        IProgress<string> Status { get; }

        /// <summary>Объект отмены операции</summary>
        CancellationToken Cancel { get; }
    }
}