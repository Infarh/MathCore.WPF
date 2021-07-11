using System.Windows;

namespace MathCore.WPF.Commands
{
    public class ToggleVisibilityCommand : Command
    {
        public bool Colapse { get; set; }

        public override bool CanExecute(object? parameter) => parameter is UIElement;

        public override void Execute(object? parameter)
        {
            if (parameter is UIElement control)
                control.Visibility = (control.Visibility, Colapse) switch
                {
                    (Visibility.Visible, true) => Visibility.Collapsed,
                    (Visibility.Visible, false) => Visibility.Hidden,
                    _ => Visibility.Visible
                };
        }
    }
}
