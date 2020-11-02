using System.Windows;

namespace MathCore.WPF.Commands
{
    public class CloseWindow : WindowCommand
    {
        protected override void Execute(Window window) => window?.Close();
    }
}