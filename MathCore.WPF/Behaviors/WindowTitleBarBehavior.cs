using System.Windows;
using System.Windows.Input;

namespace MathCore.WPF.Behaviors;

public class WindowTitleBarBehavior : DragWindow
{
    protected override void OnMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
    {
        var window = AssociatedWindow;
        if (window is null) return;
        if (E.ClickCount == 1)
        {
            switch (window.WindowState)
            {
                case WindowState.Maximized:
                    window.WindowState = WindowState.Normal;
                    window.Top         = 0;
                    base.OnMouseLeftButtonDown(Sender, E);
                    break;
                case WindowState.Normal:
                    base.OnMouseLeftButtonDown(Sender, E);
                    break;
            }
        }
        else
            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
    }
}