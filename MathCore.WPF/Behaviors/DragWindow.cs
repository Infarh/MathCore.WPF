using System.Windows.Input;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Behaviors;

public class DragWindow : WindowBehavior
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

    protected virtual void OnMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
    {
        if (E.ClickCount > 1) return;
        AssociatedWindow?.DragMove();
    }
}