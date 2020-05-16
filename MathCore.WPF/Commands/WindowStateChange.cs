using System.Windows;

namespace MathCore.WPF.Commands
{
    public class WindowStateChange : WindowCommand
    {
        public override void Execute(object? parameter)
        {
            if (!((parameter ?? RootObject) is Window window)) return;
            window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
    }
}