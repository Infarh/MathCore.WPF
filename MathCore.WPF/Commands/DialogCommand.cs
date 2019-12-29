using System.Windows;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF.Commands
{
    public class DialogCommand : LambdaCommand<bool?>
    {
        private Window Window
        {
            get
            {
                if (RootObject is Window window) return window;
                if (!(TargetObject is FrameworkElement element)) return null;
                return element.FindVisualParent<Window>();
            }
        }

        public override bool CanExecute(object obj) => ViewModel.IsDesignMode || Window != null;

        public override void Execute(object parameter)
        {
            var window = Window;
            if (!(parameter is bool)) parameter = ConvertParameter(parameter);
            window.DialogResult = parameter as bool?;
            window.Close();
        }
    }
}