using System.Windows;
using System.Windows.Input;

namespace MathCore.WPF.Commands;

public class CloseWindow : WindowCommand
{
    public bool CtrlAppClose { get; set; }

    protected override void Execute(Window? window)
    {
        window?.Close();
        if(CtrlAppClose && Keyboard.IsKeyDown(Key.LeftCtrl))
            Application.Current.Shutdown();
    }
}