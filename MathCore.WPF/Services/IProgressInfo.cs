namespace MathCore.WPF.Services;

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

    /// <summary>Отмена операции, вызываемая закрытием окна, либо нажатием на кнопку отмены</summary>
    CancellationToken Cancel { get; }

    /// <summary>Показать окно прогресса</summary>
    void Show();
}

public static class ProgressInfoEx
{
    public static void Deconstruct(this IProgressInfo Info, out IProgress<double> Progress, out CancellationToken Cancel)
    {
        Progress = Info.Progress;
        Cancel = Info.Cancel;
    }

    public static void Deconstruct(this IProgressInfo Info, out IProgress<double> Progress, out IProgress<string> Information, out IProgress<string> Status)
    {
        Progress = Info.Progress;
        Information = Info.Information;
        Status = Info.Status;
    }

    public static void Deconstruct(this IProgressInfo Info, out IProgress<double> Progress, out IProgress<string> Information, out IProgress<string> Status, out CancellationToken Cancel)
    {
        Progress = Info.Progress;
        Information = Info.Information;
        Status = Info.Status;
        Cancel = Info.Cancel;
    }
}