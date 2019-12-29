using System;
using System.Runtime.InteropServices;
using System.Windows;
using MathCore.Annotations;

namespace MathCore.WPF.pInvoke
{
    public static class User32
    {
        private const string FileName = "user32.dll";

        [DllImport(FileName, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(this IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport(FileName, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(this IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

        public static IntPtr SendMessage([NotNull] this Window window, uint Msg, IntPtr wParam, IntPtr lParam) => SendMessage(window.GetWindowHandle(), Msg, wParam, lParam);
        public static IntPtr SendMessage([NotNull] this Window window, WM Msg, IntPtr wParam, IntPtr lParam) => SendMessage(window.GetWindowHandle(), Msg, wParam, lParam);
        public static IntPtr SendMessage([NotNull] this Window window, WM Msg, SC wParam, IntPtr lParam = default) => SendMessage(window.GetWindowHandle(), (uint)Msg, (IntPtr)wParam, lParam == default ? (IntPtr)' ' : lParam);

        [DllImport(FileName)]
        public static extern bool GetMonitorInfo(this IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport(FileName)]
        public static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport(FileName)]
        public static extern IntPtr MonitorFromWindow(this IntPtr handle, int flags);
    }
}
