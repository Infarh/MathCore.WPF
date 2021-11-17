using System;
using System.Runtime.InteropServices;

namespace MathCore.WPF.pInvoke;

[StructLayout(LayoutKind.Sequential)]
public struct AppBarData
{
    public uint cbSize;
    public IntPtr hWnd;
    public uint uCallbackMessage;
    public AppBarEdge uEdge;
    public Rect rect;
    public int lParam;
}