namespace MathCore.WPF.Commands
{
    public class CancelDialog : DialogCommand
    {
        public override void Execute(object parameter) => base.Execute(parameter ?? false);
    }
}