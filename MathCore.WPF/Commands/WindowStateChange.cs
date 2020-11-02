using System.Windows;

namespace MathCore.WPF.Commands
{
    public class WindowStateChange : WindowCommand
    {
        protected override void Execute(Window window)
        {
            if (window != null)
                window.WindowState = window.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
        }
    }
}