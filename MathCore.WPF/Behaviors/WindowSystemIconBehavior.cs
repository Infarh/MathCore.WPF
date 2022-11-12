using System.Windows.Input;
using MathCore.WPF.pInvoke;

namespace MathCore.WPF.Behaviors;

public class WindowSystemIconBehavior : WindowBehavior
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
    }

    protected void OnMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
    {
        E.Handled = true;
        if (E.ClickCount > 1)
            AssociatedWindow?.Close();
        else
            AssociatedWindow?.SendMessage(WM.SYSCOMMAND, SC.KEYMENU);
    }
}