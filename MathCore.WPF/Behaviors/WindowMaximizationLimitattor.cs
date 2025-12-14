using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using MathCore.WPF.pInvoke;
using Microsoft.Xaml.Behaviors;
using SuppressMessage = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для ограничения максимального размера окна при развертывании на границах рабочей области монитора</summary>
public class WindowMaximizationLimitattor : Behavior<UIElement>
{
    /// <summary>Вызывается при присоединении поведения к элементу</summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.ForWindowFromTemplate(SetHandler);
    }

    /// <summary>Вызывается при отсоединении поведения от элемента</summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.ForWindowFromTemplate(ResetHandler);
    }

    /// <summary>Устанавливает обработчик инициализации окна</summary>
    /// <param name="window">Окно для установки обработчика</param>
    private static void SetHandler(Window window) => window.SourceInitialized += OnWindowOnSourceInitialized;

    /// <summary>Сбрасывает обработчик инициализации окна</summary>
    /// <param name="window">Окно для сброса обработчика</param>
    private static void ResetHandler(Window window) => window.SourceInitialized -= OnWindowOnSourceInitialized;

    /// <summary>Обработчик инициализации источника окна</summary>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    private static void OnWindowOnSourceInitialized(object sender, EventArgs? e)
    {
        if (sender is null) throw new ArgumentNullException(nameof(sender));
        HwndSource.FromHwnd(new WindowInteropHelper((Window) sender).Handle)?.AddHook(WindowProc);
    }

    /// <summary>Процедура обработки оконных сообщений</summary>
    /// <param name="hWnd">Дескриптор окна</param>
    /// <param name="msg">Код сообщения</param>
    /// <param name="wParam">Параметр wParam</param>
    /// <param name="lParam">Параметр lParam</param>
    /// <param name="handled">Флаг обработки сообщения</param>
    /// <returns>Результат обработки сообщения</returns>
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

    /// <summary>Обрабатывает сообщение WM_GETMINMAXINFO для корректировки максимального размера окна</summary>
    /// <param name="hWnd">Дескриптор окна</param>
    /// <param name="lParam">Указатель на структуру MINMAXINFO</param>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static void WmGetMinMaxInfo(IntPtr hWnd, IntPtr lParam)
    {
        var mmi = (MinMaxInfo)Marshal.PtrToStructure(lParam, typeof(MinMaxInfo))!;

        // Adjust the maximized size and position to fit the work area of the correct monitor
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once IdentifierTypo
        const int MONITOR_DEFAULTTONEAREST = 0x00000002;
        var       monitor                  = hWnd.MonitorFromWindow(MONITOR_DEFAULTTONEAREST);

        if (monitor != IntPtr.Zero)
        {
            var monitorInfo = new MonitorInfo();
            monitor.GetMonitorInfo(monitorInfo);
            var rcWorkArea    = monitorInfo.Work;
            var rcMonitorArea = monitorInfo.Monitor;
            mmi.MaxPosition.x = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
            mmi.MaxPosition.y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
            mmi.MaxSize.x     = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
            mmi.MaxSize.y     = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
        }
        Marshal.StructureToPtr(mmi, lParam, true);
    }
}