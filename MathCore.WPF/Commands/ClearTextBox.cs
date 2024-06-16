using System.Windows.Controls;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Commands;

public class ClearTextBox : Command
{
    public override bool CanExecute(object? parameter) => parameter is TextBox { Text.Length: > 0 };

    public override void Execute(object? parameter) => (parameter as TextBox)?.Clear();
}