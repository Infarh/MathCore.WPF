using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using MathCore.Annotations;
using MathCore.WPF.pInvoke;
using Microsoft.Xaml.Behaviors;
using SuppressMessage = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;

namespace MathCore.WPF.Behaviors
{
    public class WindowMaximizationLimitattor : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ForWindowFromTemplate(SetHandler);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.ForWindowFromTemplate(ResetHandler);
        }

        private void SetHandler([NotNull] Window window) => window.SourceInitialized += OnWindowOnSourceInitialized;

        private void ResetHandler([NotNull] Window window) => window.SourceInitialized -= OnWindowOnSourceInitialized;

        private void OnWindowOnSourceInitialized([NotNull] object? sender, [CanBeNull] EventArgs e)
        {
            if (sender is null) throw new ArgumentNullException(nameof(sender));
            HwndSource.FromHwnd(new WindowInteropHelper((Window) sender).Handle)?.AddHook(WindowProc);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static IntPtr WindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // ReSharper disable once IdentifierTypo
            const int WM_GETMINMAXINFO = 0x0024;
            if (msg == WM_GETMINMAXINFO)
            {
                WmGetMinMaxInfo(hWnd, lParam);
                handled = true;
            }
            return (IntPtr)0;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static void WmGetMinMaxInfo(IntPtr hWnd, IntPtr lParam)
        {
            var mmi = (MinMaxInfo)Marshal.PtrToStructure(lParam, typeof(MinMaxInfo))!;

            // Adjust the maximized size and position to fit the work area of the correct monitor
            // ReSharper disable once InconsistentNaming
            // ReSharper disable once IdentifierTypo
            const int MONITOR_DEFAULTTONEAREST = 0x00000002;
            var monitor = hWnd.MonitorFromWindow(MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MonitorInfo();
                monitor.GetMonitorInfo(monitorInfo);
                var rcWorkArea = monitorInfo.Work;
                var rcMonitorArea = monitorInfo.Monitor;
                mmi.MaxPosition.x = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                mmi.MaxPosition.y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                mmi.MaxSize.x = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                mmi.MaxSize.y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }
    }
}