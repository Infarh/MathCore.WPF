using System.Windows;

using MathCore.WPF.ViewModels;

namespace MathCore.WPF.Commands;

/// <summary>Команда кнопок окна диалога</summary>
public class DialogCommand : LambdaCommand<bool?>
{
    private Window? Window
    {
        get
        {
            if (RootObject is Window window) return window;
            if (TargetObject is not FrameworkElement element) return null;
            return element.FindVisualParent<Window>();
        }
    }

    public override bool CanExecute(object? obj) => ViewModel.IsDesignMode || Window != null;

    public override void Execute(object? parameter)
    {
        var window = Window ?? throw new InvalidOperationException("Отсутствует ссылка на окно");
        window.DialogResult = parameter as bool? ?? ConvertParameter(parameter) ?? false;

        window.Close();
    }
}