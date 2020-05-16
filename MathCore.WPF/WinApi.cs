using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using MathCore.WPF.TrayIcon;

namespace MathCore.WPF
{
    /// <summary>Win32 API imports</summary>
    public static class WinApi
    {
        /// <summary>
        /// Win API struct providing coordinates for a single point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            /// <summary>
            /// X coordinate.
            /// </summary>
            public int X;
            /// <summary>
            /// Y coordinate.
            /// </summary>
            public int Y;
        }

        /// <summary>
        /// Creates, updates or deletes the taskbar icon.
        /// </summary>
        [DllImport("shell32.Dll", CharSet = CharSet.Unicode)]
        public static extern bool Shell_NotifyIcon(NotifyCommand cmd, [In] ref NotifyIconData data);


        /// <summary>
        /// Creates the helper window that receives messages from the taskar icon.
        /// </summary>
        [DllImport("USER32.DLL", EntryPoint = "CreateWindowExW", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(int dwExStyle, [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, int dwStyle, int x, int y,
            int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance,
            IntPtr lpParam);


        /// <summary>
        /// Processes a default windows procedure.
        /// </summary>
        [DllImport("USER32.DLL")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wparam, IntPtr lparam);

        /// <summary>
        /// Registers the helper window class.
        /// </summary>
        [DllImport("USER32.DLL", EntryPoint = "RegisterClassW", SetLastError = true)]
        public static extern short RegisterClass(ref WindowClass lpWndClass);

        /// <summary>
        /// Registers a listener for a window message.
        /// </summary>
        /// <param name="lpString"></param>
        /// <returns></returns>
        [DllImport("User32.Dll", EntryPoint = "RegisterWindowMessageW")]
        public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

        /// <summary>
        /// Used to destroy the hidden helper window that receives messages from the
        /// taskbar icon.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("USER32.DLL", SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hWnd);


        /// <summary>
        /// Gives focus to a given window.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);


        /// <summary>
        /// Gets the maximum number of milliseconds that can elapse between a
        /// first click and a second click for the OS to consider the
        /// mouse action a double-click.
        /// </summary>
        /// <returns>The maximum amount of time, in milliseconds, that can
        /// elapse between a first click and a second click for the OS to
        /// consider the mouse action a double-click.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetDoubleClickTime();


        /// <summary>
        /// Gets the screen coordinates of the current mouse position.
        /// </summary>
        [DllImport("USER32.DLL", SetLastError = true)]
        public static extern bool GetPhysicalCursorPos(ref Point lpPoint);


        [DllImport("USER32.DLL", SetLastError = true)]
        public static extern bool GetCursorPos(ref Point lpPoint);
    }

    /// <summary>
    /// Win API WNDCLASS struct - represents a single window.
    /// Used to receive window messages.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowClass
    {
#pragma warning disable 1591

        public uint style;
        public WindowProcedureHandler lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;

#pragma warning restore 1591
    }

    /// <summary>
    /// Callback delegate which is used by the Windows API to
    /// submit window messages.
    /// </summary>
    public delegate IntPtr WindowProcedureHandler(IntPtr hwnd, uint uMsg, IntPtr wparam, IntPtr lparam);

    /// <summary>
    /// Receives messages from the taskbar icon through
    /// window messages of an underlying helper window.
    /// </summary>
    public class WindowMessageSink : IDisposable
    {
        #region members

        /// <summary>
        /// The ID of messages that are received from the the
        /// taskbar icon.
        /// </summary>
        public const int CallbackMessageId = 0x400;

        /// <summary>
        /// The ID of the message that is being received if the
        /// taskbar is (re)started.
        /// </summary>
        private uint taskbarRestartMessageId;

        /// <summary>
        /// Used to track whether a mouse-up event is just
        /// the aftermath of a double-click and therefore needs
        /// to be suppressed.
        /// </summary>
        private bool isDoubleClick;

        /// <summary>
        /// A delegate that processes messages of the hidden
        /// native window that receives window messages. Storing
        /// this reference makes sure we don't loose our reference
        /// to the message window.
        /// </summary>
        private WindowProcedureHandler messageHandler;

        /// <summary>
        /// Window class ID.
        /// </summary>
        internal string WindowId { get; private set; }

        /// <summary>
        /// Handle for the message window.
        /// </summary> 
        internal IntPtr MessageWindowHandle { get; private set; }

        /// <summary>
        /// The version of the underlying icon. Defines how
        /// incoming messages are interpreted.
        /// </summary>
        public NotifyIconVersion Version { get; set; }

        #endregion

        #region events

        /// <summary>
        /// The custom tooltip should be closed or hidden.
        /// </summary>
        public event Action<bool> ChangeToolTipStateRequest;

        /// <summary>
        /// Fired in case the user clicked or moved within
        /// the taskbar icon area.
        /// </summary>
        public event Action<MouseEvent> MouseEventReceived;

        /// <summary>
        /// Fired if a balloon ToolTip was either displayed
        /// or closed (indicated by the boolean flag).
        /// </summary>
        public event Action<bool> BalloonToolTipChanged;

        /// <summary>
        /// Fired if the taskbar was created or restarted. Requires the taskbar
        /// icon to be reset.
        /// </summary>
        public event Action TaskbarCreated;

        #endregion

        #region construction

        /// <summary>
        /// Creates a new message sink that receives message from
        /// a given taskbar icon.
        /// </summary>
        /// <param name="version"></param>
        public WindowMessageSink(NotifyIconVersion version)
        {
            Version = version;
            CreateMessageWindow();
        }


        private WindowMessageSink()
        {
        }


        /// <summary>
        /// Creates a dummy instance that provides an empty
        /// pointer rather than a real window handler.<br/>
        /// Used at design time.
        /// </summary>
        /// <returns></returns>
        internal static WindowMessageSink CreateEmpty()
        {
            return new WindowMessageSink
            {
                MessageWindowHandle = IntPtr.Zero,
                Version = NotifyIconVersion.Vista
            };
        }

        #endregion

        #region CreateMessageWindow

        /// <summary>
        /// Creates the helper message window that is used
        /// to receive messages from the taskbar icon.
        /// </summary>
        private void CreateMessageWindow()
        {
            //generate a unique ID for the window
            WindowId = "WPFTaskbarIcon_" + DateTime.Now.Ticks;

            //register window message handler
            messageHandler = OnWindowMessageReceived;

            // Create a simple window class which is reference through
            //the messageHandler delegate
            WindowClass wc;

            wc.style = 0;
            wc.lpfnWndProc = messageHandler;
            wc.cbClsExtra = 0;
            wc.cbWndExtra = 0;
            wc.hInstance = IntPtr.Zero;
            wc.hIcon = IntPtr.Zero;
            wc.hCursor = IntPtr.Zero;
            wc.hbrBackground = IntPtr.Zero;
            wc.lpszMenuName = "";
            wc.lpszClassName = WindowId;

            // Register the window class
            WinApi.RegisterClass(ref wc);

            // Get the message used to indicate the taskbar has been restarted
            // This is used to re-add icons when the taskbar restarts
            taskbarRestartMessageId = WinApi.RegisterWindowMessage("TaskbarCreated");

            // Create the message window
            MessageWindowHandle = WinApi.CreateWindowEx(0, WindowId, "", 0, 0, 0, 1, 1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            if (MessageWindowHandle == IntPtr.Zero)
            {
#if SILVERLIGHT
                throw new Exception("Message window handle was not a valid pointer.");
#else
                throw new Win32Exception("Message window handle was not a valid pointer");
#endif
            }
        }

        #endregion

        #region Handle Window Messages

        /// <summary>
        /// Callback method that receives messages from the taskbar area.
        /// </summary>
        private IntPtr OnWindowMessageReceived(IntPtr hwnd, uint messageId, IntPtr wparam, IntPtr lparam)
        {
            if (messageId == taskbarRestartMessageId)
            {
                //recreate the icon if the taskbar was restarted (e.g. due to Win Explorer shutdown)
                TaskbarCreated();
            }

            //forward message
            ProcessWindowMessage(messageId, wparam, lparam);

            // Pass the message to the default window procedure
            return WinApi.DefWindowProc(hwnd, messageId, wparam, lparam);
        }


        /// <summary>
        /// Processes incoming system messages.
        /// </summary>
        /// <param name="msg">Callback ID.</param>
        /// <param name="wParam">If the version is <see cref="NotifyIconVersion.Vista"/>
        /// or higher, this parameter can be used to resolve mouse coordinates.
        /// Currently not in use.</param>
        /// <param name="lParam">Provides information about the event.</param>
        private void ProcessWindowMessage(uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg != CallbackMessageId) return;

            switch (lParam.ToInt32())
            {
                case 0x200:
                    MouseEventReceived(MouseEvent.MouseMove);
                    break;

                case 0x201:
                    MouseEventReceived(MouseEvent.IconLeftMouseDown);
                    break;

                case 0x202:
                    if (!isDoubleClick)
                    {
                        MouseEventReceived(MouseEvent.IconLeftMouseUp);
                    }
                    isDoubleClick = false;
                    break;

                case 0x203:
                    isDoubleClick = true;
                    MouseEventReceived(MouseEvent.IconDoubleClick);
                    break;

                case 0x204:
                    MouseEventReceived(MouseEvent.IconRightMouseDown);
                    break;

                case 0x205:
                    MouseEventReceived(MouseEvent.IconRightMouseUp);
                    break;

                case 0x206:
                    //double click with right mouse button - do not trigger event
                    break;

                case 0x207:
                    MouseEventReceived(MouseEvent.IconMiddleMouseDown);
                    break;

                case 520:
                    MouseEventReceived(MouseEvent.IconMiddleMouseUp);
                    break;

                case 0x209:
                    //double click with middle mouse button - do not trigger event
                    break;

                case 0x402:
                    BalloonToolTipChanged(true);
                    break;

                case 0x403:
                case 0x404:
                    BalloonToolTipChanged(false);
                    break;

                case 0x405:
                    MouseEventReceived(MouseEvent.BalloonToolTipClicked);
                    break;

                case 0x406:
                    ChangeToolTipStateRequest(true);
                    break;

                case 0x407:
                    ChangeToolTipStateRequest(false);
                    break;

                default:
                    Debug.WriteLine("Unhandled NotifyIcon message ID: " + lParam);
                    break;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Set to true as soon as <c>Dispose</c> has been invoked.
        /// </summary>
        public bool IsDisposed { get; private set; }


        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <remarks>This method is not virtual by design. Derived classes
        /// should override <see cref="Dispose(bool)"/>.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This destructor will run only if the <see cref="Dispose()"/>
        /// method does not get called. This gives this base class the
        /// opportunity to finalize.
        /// <para>
        /// Important: Do not provide destructors in types derived from
        /// this class.
        /// </para>
        /// </summary>
        ~WindowMessageSink()
        {
            Dispose(false);
        }


        /// <summary>
        /// Removes the windows hook that receives window
        /// messages and closes the underlying helper window.
        /// </summary>
        private void Dispose(bool disposing)
        {
            //don't do anything if the component is already disposed
            if (IsDisposed) return;
            IsDisposed = true;

            //always destroy the unmanaged handle (even if called from the GC)
            WinApi.DestroyWindow(MessageWindowHandle);
            messageHandler = null;
        }

        #endregion
    }

    /// <summary>
    /// Event flags for clicked events.
    /// </summary>
    public enum MouseEvent
    {
        /// <summary>
        /// The mouse was moved withing the
        /// taskbar icon's area.
        /// </summary>
        MouseMove,

        /// <summary>
        /// The right mouse button was clicked.
        /// </summary>
        IconRightMouseDown,

        /// <summary>
        /// The left mouse button was clicked.
        /// </summary>
        IconLeftMouseDown,

        /// <summary>
        /// The right mouse button was released.
        /// </summary>
        IconRightMouseUp,

        /// <summary>
        /// The left mouse button was released.
        /// </summary>
        IconLeftMouseUp,

        /// <summary>
        /// The middle mouse button was clicked.
        /// </summary>
        IconMiddleMouseDown,

        /// <summary>
        /// The middle mouse button was released.
        /// </summary>
        IconMiddleMouseUp,

        /// <summary>
        /// The taskbar icon was double clicked.
        /// </summary>
        IconDoubleClick,

        /// <summary>
        /// The balloon tip was clicked.
        /// </summary>
        BalloonToolTipClicked
    }

    /// <summary>
    /// The notify icon version that is used. The higher
    /// the version, the more capabilities are available.
    /// </summary>
    public enum NotifyIconVersion
    {
        /// <summary>
        /// Default behavior (legacy Win95). Expects
        /// a <see cref="NotifyIconData"/> size of 488.
        /// </summary>
        Win95 = 0x0,

        /// <summary>
        /// Behavior representing Win2000 an higher. Expects
        /// a <see cref="NotifyIconData"/> size of 504.
        /// </summary>
        Win2000 = 0x3,

        /// <summary>
        /// Extended tooltip support, which is available
        /// for Vista and later.
        /// </summary>
        Vista = 0x4
    }

    /// <summary>
    /// A struct that is submitted in order to configure
    /// the taskbar icon. Provides various members that
    /// can be configured partially, according to the
    /// values of the <see cref="IconDataMembers"/>
    /// that were defined.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct NotifyIconData
    {
        /// <summary>
        /// Size of this structure, in bytes.
        /// </summary>
        public uint cbSize;

        /// <summary>
        /// Handle to the window that receives notification messages associated with an icon in the
        /// taskbar status area. The Shell uses hWnd and uID to identify which icon to operate on
        /// when Shell_NotifyIcon is invoked.
        /// </summary>
        public IntPtr WindowHandle;

        /// <summary>
        /// Application-defined identifier of the taskbar icon. The Shell uses hWnd and uID to identify
        /// which icon to operate on when Shell_NotifyIcon is invoked. You can have multiple icons
        /// associated with a single hWnd by assigning each a different uID. This feature, however
        /// is currently not used.
        /// </summary>
        public uint TaskbarIconId;

        /// <summary>
        /// Flags that indicate which of the other members contain valid data. This member can be
        /// a combination of the NIF_XXX constants.
        /// </summary>
        public IconDataMembers ValidMembers;

        /// <summary>
        /// Application-defined message identifier. The system uses this identifier to send
        /// notifications to the window identified in hWnd.
        /// </summary>
        public uint CallbackMessageId;

        /// <summary>
        /// A handle to the icon that should be displayed. Just
        /// <c>Icon.Handle</c>.
        /// </summary>
        public IntPtr IconHandle;

        /// <summary>
        /// String with the text for a standard ToolTip. It can have a maximum of 64 characters including
        /// the terminating NULL. For Version 5.0 and later, szTip can have a maximum of
        /// 128 characters, including the terminating NULL.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string ToolTipText;


        /// <summary>
        /// State of the icon. Remember to also set the <see cref="StateMask"/>.
        /// </summary>
        public IconState IconState;

        /// <summary>
        /// A value that specifies which bits of the state member are retrieved or modified.
        /// For example, setting this member to <see cref="IconState.Hidden"/>
        /// causes only the item's hidden
        /// state to be retrieved.
        /// </summary>
        public IconState StateMask;

        /// <summary>
        /// String with the text for a balloon ToolTip. It can have a maximum of 255 characters.
        /// To remove the ToolTip, set the NIF_INFO flag in uFlags and set szInfo to an empty string.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string BalloonText;

        /// <summary>
        /// Mainly used to set the version when <see cref="WinApi.Shell_NotifyIcon"/> is invoked
        /// with <see cref="NotifyCommand.SetVersion"/>. However, for legacy operations,
        /// the same member is also used to set timouts for balloon ToolTips.
        /// </summary>
        public uint VersionOrTimeout;

        /// <summary>
        /// String containing a title for a balloon ToolTip. This title appears in boldface
        /// above the text. It can have a maximum of 63 characters.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string BalloonTitle;

        /// <summary>
        /// Adds an icon to a balloon ToolTip, which is placed to the left of the title. If the
        /// <see cref="BalloonTitle"/> member is zero-length, the icon is not shown.
        /// </summary>
        public BalloonFlags BalloonFlags;

        /// <summary>
        /// Windows XP (Shell32.dll version 6.0) and later.<br/>
        /// - Windows 7 and later: A registered GUID that identifies the icon.
        ///   This value overrides uID and is the recommended method of identifying the icon.<br/>
        /// - Windows XP through Windows Vista: Reserved.
        /// </summary>
        public Guid TaskbarIconGuid;

        /// <summary>
        /// Windows Vista (Shell32.dll version 6.0.6) and later. The handle of a customized
        /// balloon icon provided by the application that should be used independently
        /// of the tray icon. If this member is non-NULL and the <see cref="BalloonFlags.User"/>
        /// flag is set, this icon is used as the balloon icon.<br/>
        /// If this member is NULL, the legacy behavior is carried out.
        /// </summary>
        public IntPtr CustomBalloonIconHandle;


        /// <summary>
        /// Creates a default data structure that provides
        /// a hidden taskbar icon without the icon being set.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static NotifyIconData CreateDefault(IntPtr handle)
        {
            var data = new NotifyIconData();

            if (Environment.OSVersion.Version.Major >= 6)
            {
                //use the current size
                data.cbSize = (uint)Marshal.SizeOf(data);
            }
            else
            {
                //we need to set another size on xp/2003- otherwise certain
                //features (e.g. balloon tooltips) don't work.
                data.cbSize = 952; // NOTIFYICONDATAW_V3_SIZE

                //set to fixed timeout
                data.VersionOrTimeout = 10;
            }

            data.WindowHandle = handle;
            data.TaskbarIconId = 0x0;
            data.CallbackMessageId = WindowMessageSink.CallbackMessageId;
            data.VersionOrTimeout = (uint)NotifyIconVersion.Win95;

            data.IconHandle = IntPtr.Zero;

            //hide initially
            data.IconState = IconState.Hidden;
            data.StateMask = IconState.Hidden;

            //set flags
            data.ValidMembers = IconDataMembers.Message
                                | IconDataMembers.Icon
                                | IconDataMembers.Tip;

            //reset strings
            data.ToolTipText = data.BalloonText = data.BalloonTitle = string.Empty;

            return data;
        }
    }

    /// <summary>
    /// Flags that define the icon that is shown on a balloon
    /// tooltip.
    /// </summary>
    public enum BalloonFlags
    {
        /// <summary>
        /// No icon is displayed.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// An information icon is displayed.
        /// </summary>
        Info = 0x01,

        /// <summary>
        /// A warning icon is displayed.
        /// </summary>
        Warning = 0x02,

        /// <summary>
        /// An error icon is displayed.
        /// </summary>
        Error = 0x03,

        /// <summary>
        /// Windows XP Service Pack 2 (SP2) and later.
        /// Use a custom icon as the title icon.
        /// </summary>
        User = 0x04,

        /// <summary>
        /// Windows XP (Shell32.dll version 6.0) and later.
        /// Do not play the associated sound. Applies only to balloon ToolTips.
        /// </summary>
        NoSound = 0x10,

        /// <summary>
        /// Windows Vista (Shell32.dll version 6.0.6) and later. The large version
        /// of the icon should be used as the balloon icon. This corresponds to the
        /// icon with dimensions SM_CXICON x SM_CYICON. If this flag is not set,
        /// the icon with dimensions XM_CXSMICON x SM_CYSMICON is used.<br/>
        /// - This flag can be used with all stock icons.<br/>
        /// - Applications that use older customized icons (NIIF_USER with hIcon) must
        ///   provide a new SM_CXICON x SM_CYICON version in the tray icon (hIcon). These
        ///   icons are scaled down when they are displayed in the System Tray or
        ///   System Control Area (SCA).<br/>
        /// - New customized icons (NIIF_USER with hBalloonIcon) must supply an
        ///   SM_CXICON x SM_CYICON version in the supplied icon (hBalloonIcon).
        /// </summary>
        LargeIcon = 0x20,

        /// <summary>
        /// Windows 7 and later.
        /// </summary>
        RespectQuietTime = 0x80
    }

    /// <summary>
    /// Indicates which members of a <see cref="NotifyIconData"/> structure
    /// were set, and thus contain valid data or provide additional information
    /// to the ToolTip as to how it should display.
    /// </summary>
    [Flags]
    public enum IconDataMembers
    {
        /// <summary>
        /// The message ID is set.
        /// </summary>
        Message = 0x01,

        /// <summary>
        /// The notification icon is set.
        /// </summary>
        Icon = 0x02,

        /// <summary>
        /// The tooltip is set.
        /// </summary>
        Tip = 0x04,

        /// <summary>
        /// State information (<see cref="IconState"/>) is set. This
        /// applies to both <see cref="NotifyIconData.IconState"/> and
        /// <see cref="NotifyIconData.StateMask"/>.
        /// </summary>
        State = 0x08,

        /// <summary>
        /// The balloon ToolTip is set. Accordingly, the following
        /// members are set: <see cref="NotifyIconData.BalloonText"/>,
        /// <see cref="NotifyIconData.BalloonTitle"/>, <see cref="NotifyIconData.BalloonFlags"/>,
        /// and <see cref="NotifyIconData.VersionOrTimeout"/>.
        /// </summary>
        Info = 0x10,

        // Internal identifier is set. Reserved, thus commented out.
        //Guid = 0x20,

        /// <summary>
        /// Windows Vista (Shell32.dll version 6.0.6) and later. If the ToolTip
        /// cannot be displayed immediately, discard it.<br/>
        /// Use this flag for ToolTips that represent real-time information which
        /// would be meaningless or misleading if displayed at a later time.
        /// For example, a message that states "Your telephone is ringing."<br/>
        /// This modifies and must be combined with the <see cref="Info"/> flag.
        /// </summary>
        Realtime = 0x40,

        /// <summary>
        /// Windows Vista (Shell32.dll version 6.0.6) and later.
        /// Use the standard ToolTip. Normally, when uVersion is set
        /// to NOTIFYICON_VERSION_4, the standard ToolTip is replaced
        /// by the application-drawn pop-up user interface (UI).
        /// If the application wants to show the standard tooltip
        /// in that case, regardless of whether the on-hover UI is showing,
        /// it can specify NIF_SHOWTIP to indicate the standard tooltip
        /// should still be shown.<br/>
        /// Note that the NIF_SHOWTIP flag is effective until the next call 
        /// to Shell_NotifyIcon.
        /// </summary>
        UseLegacyToolTips = 0x80
    }

    /// <summary>
    /// The state of the icon - can be set to
    /// hide the icon.
    /// </summary>
    public enum IconState
    {
        /// <summary>
        /// The icon is visible.
        /// </summary>
        Visible = 0x00,

        /// <summary>
        /// Hide the icon.
        /// </summary>
        Hidden = 0x01,

        // The icon is shared - currently not supported, thus commented out.
        //Shared = 0x02
    }

    internal class AppBarInfo
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("shell32.dll")]
        private static extern uint SHAppBarMessage(uint dwMessage, ref APPBARDATA data);

        [DllImport("user32.dll")]
        private static extern int SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);


        private const int ABE_BOTTOM = 3;
        private const int ABE_LEFT = 0;
        private const int ABE_RIGHT = 2;
        private const int ABE_TOP = 1;

        private const int ABM_GETTASKBARPOS = 0x00000005;

        // SystemParametersInfo constants
        private const uint SPI_GETWORKAREA = 0x0030;

        private APPBARDATA m_data;

        public ScreenEdge Edge => (ScreenEdge)m_data.uEdge;


        public Rectangle WorkArea
        {
            get
            {
                var bResult = 0;
                var rc = new RECT();
                var rawRect = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
                bResult = SystemParametersInfo(SPI_GETWORKAREA, 0, rawRect, 0);
                rc = (RECT)Marshal.PtrToStructure(rawRect, rc.GetType());

                if (bResult != 1) return new Rectangle(0, 0, 0, 0);
                Marshal.FreeHGlobal(rawRect);
                return new Rectangle(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
            }
        }


        public void GetPosition(string strClassName, string strWindowName)
        {
            m_data = new APPBARDATA();
            m_data.cbSize = (uint)Marshal.SizeOf(m_data.GetType());

            if (FindWindow(strClassName, strWindowName) == IntPtr.Zero)
                throw new Exception("Failed to find an AppBar that matched the given criteria");

            if (SHAppBarMessage(ABM_GETTASKBARPOS, ref m_data) != 1)
                throw new Exception("Failed to communicate with the given AppBar");
        }


        public void GetSystemTaskBarPosition() => GetPosition("Shell_TrayWnd", null);

        public enum ScreenEdge
        {
            Undefined = -1,
            Left = ABE_LEFT,
            Top = ABE_TOP,
            Right = ABE_RIGHT,
            Bottom = ABE_BOTTOM
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public int lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }

}