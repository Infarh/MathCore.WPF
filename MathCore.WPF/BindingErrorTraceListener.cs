using System.Diagnostics;
using System.Text;
using System.Windows;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF;

public class BindingErrorTraceListener : TraceListener
{
    public static void Initialize()
    {
        PresentationTraceSources.DataBindingSource.Listeners.Add(new BindingErrorTraceListener());
        PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
    }

    private readonly StringBuilder _MessageBuilder = new();

    /// <inheritdoc />
    public override void Write(string? message) => _MessageBuilder.Append(message);

    /// <inheritdoc />
    public override void WriteLine(string? message)
    {
        Write(message);
        MessageBox.Show(_MessageBuilder.ToString(), "Binding error", MessageBoxButton.OK, MessageBoxImage.Warning);
        _MessageBuilder.Clear();
    }
}