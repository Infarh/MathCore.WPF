using System.Windows;

namespace MathCore.WPF.Commands
{
    public abstract class WindowCommand : LambdaCommand<Window>
    {
        public override bool CanExecute(object obj) => true;
    }
}