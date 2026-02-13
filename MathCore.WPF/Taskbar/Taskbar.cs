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

    /// <summary>Статический инициализатор класса <see cref="Taskbar" /></summary>
    static Taskbar() => _AppBarData = new()
    {
        cbSize = (uint)Marshal.SizeOf(typeof(AppBarData)),
        hWnd = User32.FindWindow(ClassName, null)
    };

    /// <summary>Возвращает значение, указывающее, что панель задач всегда поверх других окон</summary>
    /// <value><c>true</c>, если панель задач всегда поверх других окон; иначе <c>false</c></value>
    /// <remarks>Это свойство всегда возвращает <c>false</c> в Windows 7 и новее</remarks>
    public static bool AlwaysOnTop
    {
        get
        {
            var state = Shell32.SHAppBarMessage(AppBarMessage.GetState, ref _AppBarData).ToInt32();
            return ((ABS)state).HasFlag(ABS.AlwaysOnTop);
        }
    }

    /// <summary>Возвращает значение, указывающее, что панель задач автоматически скрывается при неактивности</summary>
    /// <value><c>true</c>, если включено автоскрытие панели задач; иначе <c>false</c></value>
    public static bool AutoHide
    {
        get
        {
            var state = Shell32.SHAppBarMessage(AppBarMessage.GetState, ref _AppBarData).ToInt32();
            return ((ABS)state).HasFlag(ABS.AutoHide);
        }
    }

    /// <summary>Возвращает текущие границы отображения панели задач</summary>
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

    /// <summary>Возвращает границы отображения при полностью видимой панели задач</summary>
    public static Rectangle DisplayBounds =>
        RefreshBoundsAndPosition()
            ? Rectangle.FromLTRB(
                _AppBarData.rect.Left,
                _AppBarData.rect.Top,
                _AppBarData.rect.Right,
                _AppBarData.rect.Bottom)
            : CurrentBounds;

    /// <summary>Возвращает дескриптор окна панели задач</summary>
    public static IntPtr Handle => _AppBarData.hWnd;

    /// <summary>Возвращает положение панели задач на экране</summary>
    public static TaskbarPosition Position => RefreshBoundsAndPosition()
        ? (TaskbarPosition)_AppBarData.uEdge
        : TaskbarPosition.Unknown;

    private const int SW_HIDE = 0;
    /// <summary>Скрывает панель задач</summary>
    public static void Hide() => _ = User32.ShowWindow(Handle, SW_HIDE);

    private const int SW_SHOW = 1;

    /// <summary>Показывает панель задач</summary>
    public static void Show() => _ = User32.ShowWindow(Handle, SW_SHOW);

    private static bool RefreshBoundsAndPosition() =>
        //! SHAppBarMessage возвращает IntPtr.Zero при ошибке
        Shell32.SHAppBarMessage(AppBarMessage.GetTaskbarPos, ref _AppBarData) != IntPtr.Zero;
}