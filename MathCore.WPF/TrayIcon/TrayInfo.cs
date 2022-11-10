namespace MathCore.WPF.TrayIcon;

/// <summary>Resolves the current tray position.</summary>
public static class TrayInfo
{
    /// <summary>Gets the position of the system tray.</summary>
    /// <returns>Tray coordinates.</returns>
    public static WinApi.Point GetTrayLocation()
    {
        var info = new AppBarInfo();
        info.GetSystemTaskBarPosition();

        var rcWorkArea = info.WorkArea;

        int x = 0, y = 0;
        if(info.Edge == AppBarInfo.ScreenEdge.Left)
        {
            x = rcWorkArea.Left + 2;
            y = rcWorkArea.Bottom;
        }
        else if(info.Edge == AppBarInfo.ScreenEdge.Bottom)
        {
            x = rcWorkArea.Right;
            y = rcWorkArea.Bottom;
        }
        else if(info.Edge == AppBarInfo.ScreenEdge.Top)
        {
            x = rcWorkArea.Right;
            y = rcWorkArea.Top;
        }
        else if(info.Edge == AppBarInfo.ScreenEdge.Right)
        {
            x = rcWorkArea.Right;
            y = rcWorkArea.Bottom;
        }

        return new WinApi.Point { X = x, Y = y };
    }
}