using System.Windows.Input;
using MathCore.WPF.pInvoke;
using MathCore.Annotations;

namespace MathCore.WPF.Behaviors
{
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

        protected void OnMouseLeftButtonDown([NotNull] object Sender, [NotNull] MouseButtonEventArgs E)
        {
            E.Handled = true;
            if (E.ClickCount > 1)
                AssociatedWindow?.Close();
            else
                AssociatedWindow?.SendMessage(WM.SYSCOMMAND, SC.KEYMENU);
        }
    }
}