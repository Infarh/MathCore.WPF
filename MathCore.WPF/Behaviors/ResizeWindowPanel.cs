using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using MathCore.Annotations;
using MathCore.WPF.pInvoke;
using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors
{
    public class ResizeWindowPanel : Behavior<Panel>
    {
        [CanBeNull] private MouseButtonEventHandler _MouseButtonsEventHandler;
        [CanBeNull] private Window _Window;

        protected override void OnAttached()
        {
            base.OnAttached();
            _Window = AssociatedObject.TemplatedParent as Window ?? AssociatedObject.FindVisualParent<Window>();
            if (_Window is null) return;
            _MouseButtonsEventHandler = OnResizeWindowShape_MouseDown;
            AssociatedObject.AddHandler(UIElement.MouseDownEvent, _MouseButtonsEventHandler);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (_MouseButtonsEventHandler != null)
                AssociatedObject.RemoveHandler(UIElement.MouseDownEvent, _MouseButtonsEventHandler);
        }

        private void OnResizeWindowShape_MouseDown([NotNull] object Sender, [NotNull] MouseButtonEventArgs E)
        {
            var window = _Window;
            if (window is null) return;
            switch (E.OriginalSource)
            {
                default: return;
                case Line line:
                    if (line.VerticalAlignment == VerticalAlignment.Top)
                        window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.North), IntPtr.Zero);
                    else if (line.VerticalAlignment == VerticalAlignment.Bottom)
                        window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.South), IntPtr.Zero);
                    else if (line.HorizontalAlignment == HorizontalAlignment.Left)
                        window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.West), IntPtr.Zero);
                    else if (line.HorizontalAlignment == HorizontalAlignment.Right)
                        window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.East), IntPtr.Zero);
                    break;

                case Rectangle rect:
                    if (rect.VerticalAlignment == VerticalAlignment.Top && rect.HorizontalAlignment == HorizontalAlignment.Left)
                        window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.NorthWest), IntPtr.Zero);
                    else if (rect.VerticalAlignment == VerticalAlignment.Top && rect.HorizontalAlignment == HorizontalAlignment.Right)
                        window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.NorthEast), IntPtr.Zero);
                    else if (rect.VerticalAlignment == VerticalAlignment.Bottom && rect.HorizontalAlignment == HorizontalAlignment.Right)
                        window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.SouthEast), IntPtr.Zero);
                    else if (rect.VerticalAlignment == VerticalAlignment.Bottom && rect.HorizontalAlignment == HorizontalAlignment.Left)
                        window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.SouthWest), IntPtr.Zero);
                    break;
            }

            window.SendMessage(WM.LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
        }
    }
}