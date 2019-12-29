using System.Windows.Controls;

namespace MathCore.WPF.Commands
{
    public class ClearTextBox : Command
    {
        public override bool CanExecute(object parameter) => parameter is TextBox text_box && text_box.Text.Length > 0;

        public override void Execute(object parameter)
        {
            if (parameter is TextBox text_box) text_box.Text = "";
        }
    }
}