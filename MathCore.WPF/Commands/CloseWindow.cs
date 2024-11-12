using System.Windows;
using System.Windows.Input;

namespace MathCore.WPF.Commands;

/// <summary>Команда для закрытия окна.</summary>
public class CloseWindow : WindowCommand
{
    /// <summary>Флаг, указывающий на необходимость закрытия всего приложения при нажатии Ctrl.</summary>
    public bool CtrlAppClose { get; set; }

    /// <summary>Выполняет команду закрытия окна.</summary>
    /// <param name="window">Окно, которое необходимо закрыть.</param>
    protected override void Execute(Window? window)
    {
        // Закрываем окно, если оно не null.
        window?.Close();

        // Если флаг CtrlAppClose установлен и нажата клавиша Ctrl, то закрываем всё приложение.
        if (CtrlAppClose && Keyboard.IsKeyDown(Key.LeftCtrl))
            Application.Current.Shutdown();
    }
}