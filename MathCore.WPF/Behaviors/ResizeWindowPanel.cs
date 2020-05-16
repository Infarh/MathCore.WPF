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
        private MouseButtonEventHandler? _MouseButtonsEventHandler;
        private Window? _Window;

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
                    switch (line.VerticalAlignment)
                    {
                        case VerticalAlignment.Top:
                            window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.North), IntPtr.Zero);
                            break;
                        case VerticalAlignment.Bottom:
                            window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.South), IntPtr.Zero);
                            break;
                        default:
                            {
                                switch (line.HorizontalAlignment)
                                {
                                    case HorizontalAlignment.Left:
                                        window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.West), IntPtr.Zero);
                                        break;
                                    case HorizontalAlignment.Right:
                                        window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.East), IntPtr.Zero);
                                        break;
                                }

                                break;
                            }
                    }
                    break;

                case Rectangle rect:
                    switch (rect.VerticalAlignment)
                    {
                        case VerticalAlignment.Top when rect.HorizontalAlignment == HorizontalAlignment.Left:
                            window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.NorthWest), IntPtr.Zero);
                            break;
                        case VerticalAlignment.Top when rect.HorizontalAlignment == HorizontalAlignment.Right:
                            window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.NorthEast), IntPtr.Zero);
                            break;
                        case VerticalAlignment.Bottom when rect.HorizontalAlignment == HorizontalAlignment.Right:
                            window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.SouthEast), IntPtr.Zero);
                            break;
                        case VerticalAlignment.Bottom when rect.HorizontalAlignment == HorizontalAlignment.Left:
                            window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.SouthWest), IntPtr.Zero);
                            break;
                    }
                    break;
            }

            window.SendMessage(WM.LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
        }
    }
}