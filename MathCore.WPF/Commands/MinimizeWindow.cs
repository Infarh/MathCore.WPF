using System.Windows;

namespace MathCore.WPF.Commands
{
    public class MinimizeWindow : WindowCommand
    {
        protected override void Execute(Window? window)
        {
            if (window != null)
                window.WindowState = WindowState.Minimized;
        }
    }
}