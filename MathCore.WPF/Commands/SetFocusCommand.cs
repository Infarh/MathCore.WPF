using System.Windows;
using System.Windows.Input;

namespace MathCore.WPF.Commands;

public class SetFocusCommand : Command
{
    public override bool CanExecute(object? parameter) => parameter is FrameworkElement;

    public override void Execute(object? parameter)
    {
        if(parameter is not FrameworkElement element) return;
        var root = element.FindVisualRoot();
        FocusManager.SetFocusedElement(root!, element);
    }
}