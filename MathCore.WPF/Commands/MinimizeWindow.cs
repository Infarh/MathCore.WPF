using System.Windows;

namespace MathCore.WPF.Commands
{
    public class MinimizeWindow : WindowCommand
    {
        public override void Execute(object? parameter)
        {
            if (!((parameter ?? RootObject) is Window window)) return;
            window.WindowState = WindowState.Minimized;
        }
    }
}