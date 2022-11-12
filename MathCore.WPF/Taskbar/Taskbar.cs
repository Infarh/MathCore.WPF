using System.Drawing;
using System.Runtime.InteropServices;

using MathCore.WPF.pInvoke;

// ReSharper disable InconsistentNaming

namespace MathCore.WPF.Taskbar;

[Copyright("franzalex", url = "https://gist.github.com/franzalex/e747e6b318ab8f328aa02301f25ec534")]
public static class Taskbar
{
    private enum ABS
    {
        AutoHide = 0x01,
        AlwaysOnTop = 0x02,
    }

    private const string ClassName = "Shell_TrayWnd";
    private static AppBarData _AppBarData;

    /// <summary>Static initializer of the <see cref="Taskbar" /> class.</summary>
    static Taskbar() => _AppBarData = new AppBarData
    {
        cbSize = (uint)Marshal.SizeOf(typeof(AppBarData)),
        hWnd   = User32.FindWindow(ClassName, null)
    };

    /// <summary>  Gets a value indicating whether the taskbar is always on top of other windows.</summary>
    /// <value><c>true</c> if the taskbar is always on top of other windows; otherwise, <c>false</c>.</value>
    /// <remarks>This property always returns <c>false</c> on Windows 7 and newer.</remarks>
    public static bool AlwaysOnTop
    {
        get
        {
            var state = Shell32.SHAppBarMessage(AppBarMessage.GetState, ref _AppBarData).ToInt32();
            return ((ABS)state).HasFlag(ABS.AlwaysOnTop);
        }
    }

    /// <summary>  Gets a value indicating whether the taskbar is automatically hidden when inactive.</summary>
    /// <value><c>true</c> if the taskbar is set to auto-hide is enabled; otherwise, <c>false</c>.</value>
    public static bool AutoHide
    {
        get
        {
            var state = Shell32.SHAppBarMessage(AppBarMessage.GetState, ref _AppBarData).ToInt32();
            return ((ABS)state).HasFlag(ABS.AutoHide);
        }
    }

    /// <summary>Gets the current display bounds of the taskbar.</summary>
    public static Rectangle CurrentBounds
    {
        get
        {
            var rect = new Rect();
            return User32.GetWindowRect(Handle, ref rect)
                ? Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom)
                : Rectangle.Empty;
        }
    }

    /// <summary>Gets the display bounds when the taskbar is fully visible.</summary>
    public static Rectangle DisplayBounds =>
        RefreshBoundsAndPosition()
            ? Rectangle.FromLTRB(
                _AppBarData.rect.Left,
                _AppBarData.rect.Top,
                _AppBarData.rect.Right,
                _AppBarData.rect.Bottom)
            : CurrentBounds;

    /// <summary>Gets the taskbar's window handle.</summary>
    public static IntPtr Handle => _AppBarData.hWnd;

    /// <summary>Gets the taskbar's position on the screen.</summary>
    public static TaskbarPosition Position => RefreshBoundsAndPosition()
        ? (TaskbarPosition)_AppBarData.uEdge
        : TaskbarPosition.Unknown;

    private const int SW_HIDE = 0;
    /// <summary>Hides the taskbar.</summary>
    public static void Hide() => User32.ShowWindow(Handle, SW_HIDE);

    private const int SW_SHOW = 1;

    /// <summary>Shows the taskbar.</summary>
    public static void Show() => User32.ShowWindow(Handle, SW_SHOW);

    private static bool RefreshBoundsAndPosition() =>
        //! SHAppBarMessage returns IntPtr.Zero **if it fails**
        Shell32.SHAppBarMessage(AppBarMessage.GetTaskbarPos, ref _AppBarData) != IntPtr.Zero;
}