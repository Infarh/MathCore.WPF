using System.Windows;

namespace MathCore.WPF.Commands
{
    public class CloseWindow : WindowCommand
    {
        public override void Execute(object parameter) => ((parameter ?? RootObject) as Window)?.Close();
    }
}