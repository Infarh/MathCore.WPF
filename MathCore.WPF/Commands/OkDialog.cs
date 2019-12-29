namespace MathCore.WPF.Commands
{
    public class OkDialog : DialogCommand
    {
        public override void Execute(object parameter) => base.Execute(parameter ?? true);
    }
}