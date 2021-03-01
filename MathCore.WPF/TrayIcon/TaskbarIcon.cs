using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Threading;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.TrayIcon
{
    /// <summary>
    /// A WPF proxy to for a taskbar icon (NotifyIcon) that sits in the system's
    /// taskbar notification area ("system tray").
    /// </summary>
    public partial class TaskbarIcon : FrameworkElement, IDisposable
    {
        #region Members

        /// <summary>
        /// Represents the current icon data.
        /// </summary>
        private NotifyIconData _IconData;

        /// <summary>
        /// Receives messages from the taskbar icon.
        /// </summary>
        private readonly WindowMessageSink _MessageSink;

        /// <summary>
        /// An action that is being invoked if the
        /// <see cref="_SingleClickTimer"/> fires.
        /// </summary>
        private Action? _SingleClickTimerAction;

        /// <summary>
        /// A timer that is used to differentiate between single
        /// and double clicks.
        /// </summary>
        private readonly Timer _SingleClickTimer;

        /// <summary>
        /// A timer that is used to close open balloon tooltips.
        /// </summary>
        private readonly Timer _BalloonCloseTimer;

        /// <summary>
        /// Indicates whether the taskbar icon has been created or not.
        /// </summary>
        public bool IsTaskbarIconCreated { get; private set; }

        /// <summary>
        /// Indicates whether custom tooltips are supported, which depends
        /// on the OS. Windows Vista or higher is required in order to
        /// support this feature.
        /// </summary>
        public bool SupportsCustomToolTips => _MessageSink.Version == NotifyIconVersion.Vista;


        /// <summary>
        /// Checks whether a non-tooltip popup is currently opened.
        /// </summary>
        private bool IsPopupOpen =>
            TrayPopupResolved != null && TrayPopupResolved.IsOpen
            || ContextMenu != null && ContextMenu.IsOpen
            || CustomBalloon != null && CustomBalloon.IsOpen;

        private double _ScalingFactor = double.NaN;

        #endregion

        #region Construction

        /// <summary>
        /// Inits the taskbar icon and registers a message listener
        /// in order to receive events from the taskbar area.
        /// </summary>
        public TaskbarIcon()
        {
            //using dummy sink in design mode
            _MessageSink = TaskBarIconUtilities.IsDesignMode
                ? WindowMessageSink.CreateEmpty()
                : new WindowMessageSink(NotifyIconVersion.Win95);

            //init icon data structure
            _IconData = NotifyIconData.CreateDefault(_MessageSink.MessageWindowHandle);

            //create the taskbar icon
            CreateTaskbarIcon();

            //register event listeners
            _MessageSink.MouseEventReceived += OnMouseEvent;
            _MessageSink.TaskbarCreated += OnTaskbarCreated;
            _MessageSink.ChangeToolTipStateRequest += OnToolTipChange;
            _MessageSink.BalloonToolTipChanged += OnBalloonToolTipChanged;

            //init single click / balloon timers
            _SingleClickTimer = new Timer(DoSingleClickAction);
            _BalloonCloseTimer = new Timer(CloseBalloonCallback);

            //register listener in order to get notified when the application closes
            if (Application.Current != null) Application.Current.Exit += OnExit;
        }

        #endregion

        #region Custom Balloons

        /// <summary>
        /// Shows a custom control as a tooltip in the tray location.
        /// </summary>
        /// <param name="balloon"></param>
        /// <param name="animation">An optional animation for the popup.</param>
        /// <param name="timeout">The time after which the popup is being closed.
        /// Submit null in order to keep the balloon open
        /// </param>
        /// <exception cref="ArgumentNullException">If <paramref name="balloon"/>
        /// is a null reference.</exception>
        public void ShowCustomBalloon(UIElement balloon, PopupAnimation animation, int? timeout)
        {
            var dispatcher = this.GetDispatcher() ?? throw new InvalidOperationException("Не получен диспетчер UI");
            if (!dispatcher.CheckAccess())
            {
                var action = new Action(() => ShowCustomBalloon(balloon, animation, timeout));
                dispatcher.Invoke(DispatcherPriority.Normal, action);
                return;
            }

            if (balloon is null) throw new ArgumentNullException(nameof(balloon));
            if (timeout < 500)
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, $"Invalid timeout of {timeout} milliseconds. Timeout must be at least 500 ms");

            EnsureNotDisposed();

            //make sure we don't have an open balloon
            lock (this)
                CloseBalloon();

            //create an invisible popup that hosts the UIElement
            var popup = new Popup { AllowsTransparency = true };

            //provide the popup with the taskbar icon's data context
            UpdateDataContext(popup, null, DataContext);

            //don't animate by default - dev can use attached
            //events or override
            popup.PopupAnimation = animation;

            //in case the balloon is cleaned up through routed events, the
            //control didn't remove the balloon from its parent popup when
            //if was closed the last time - just make sure it doesn't have
            //a parent that is a popup
            var parent = LogicalTreeHelper.GetParent(balloon) as Popup;
            if (parent != null) parent.Child = null;

            if (parent != null)
                throw new InvalidOperationException($"Cannot display control [{balloon}] in a new balloon popup - that control already has a parent. You may consider creating new balloons every time you want to show one.");

            popup.Child = balloon;

            //don't set the PlacementTarget as it causes the popup to become hidden if the
            //TaskbarIcon's parent is hidden, too...
            //popup.PlacementTarget = this;

            popup.Placement = PlacementMode.AbsolutePoint;
            popup.StaysOpen = true;

            var position = TrayInfo.GetTrayLocation();
            position = GetDeviceCoordinates(position);
            popup.HorizontalOffset = position.X - 1;
            popup.VerticalOffset = position.Y - 1;

            //store reference
            lock (this)
                SetCustomBalloon(popup);

            //assign this instance as an attached property
            SetParentTaskbarIcon(balloon, this);

            //fire attached event
            RaiseBalloonShowingEvent(balloon, this);

            //display item
            popup.IsOpen = true;

            if (timeout.HasValue) //register timer to close the popup
                _BalloonCloseTimer.Change(timeout.Value, Timeout.Infinite);
        }


        /// <summary>
        /// Resets the closing timeout, which effectively
        /// keeps a displayed balloon message open until
        /// it is either closed programmatically through
        /// <see cref="CloseBalloon"/> or due to a new
        /// message being displayed.
        /// </summary>
        public void ResetBalloonCloseTimer()
        {
            if (IsDisposed) return;

            lock (this) //reset timer in any case
                _BalloonCloseTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }


        /// <summary>
        /// Closes the current <see cref="CustomBalloon"/>, if the
        /// property is set.
        /// </summary>
        public void CloseBalloon()
        {
            if (IsDisposed) return;

            var dispatcher = this.GetDispatcher().NotNull();
            if (!dispatcher.CheckAccess())
            {
                Action action = CloseBalloon;
                dispatcher.Invoke(DispatcherPriority.Normal, action);
                return;
            }

            lock (this)
            {
                //reset timer in any case
                _BalloonCloseTimer.Change(Timeout.Infinite, Timeout.Infinite);

                //reset old popup, if we still have one
                var popup = CustomBalloon;
                if (popup is null) return;
                var element = popup.Child;

                //announce closing
                if (RaiseBalloonClosingEvent(element, this)?.Handled != true)
                {
                    //if the event was handled, clear the reference to the popup,
                    //but don't close it - the handling code has to manage this stuff now

                    //close the popup
                    popup.IsOpen = false;

                    //remove the reference of the popup to the balloon in case we want to reuse
                    //the balloon (then added to a new popup)
                    popup.Child = null;

                    //reset attached property
                    if (element != null) SetParentTaskbarIcon(element, null);
                }

                //remove custom balloon anyway
                SetCustomBalloon(null);
            }
        }


        /// <summary>
        /// Timer-invoke event which closes the currently open balloon and
        /// resets the <see cref="CustomBalloon"/> dependency property.
        /// </summary>
        private void CloseBalloonCallback(object? state)
        {
            if (IsDisposed) return;

            //switch to UI thread
            Action action = CloseBalloon;
            this.GetDispatcher().NotNull().Invoke(action);
        }

        #endregion

        #region Process Incoming Mouse Events

        /// <summary>
        /// Processes mouse events, which are bubbled
        /// through the class' routed events, trigger
        /// certain actions (e.g. show a popup), or
        /// both.
        /// </summary>
        /// <param name="me">Event flag.</param>
        private void OnMouseEvent(MouseEvent me)
        {
            if (IsDisposed) return;

            switch (me)
            {
                case MouseEvent.MouseMove:
                    RaiseTrayMouseMoveEvent();
                    //immediately return - there's nothing left to evaluate
                    return;
                case MouseEvent.IconRightMouseDown:
                    RaiseTrayRightMouseDownEvent();
                    break;
                case MouseEvent.IconLeftMouseDown:
                    RaiseTrayLeftMouseDownEvent();
                    break;
                case MouseEvent.IconRightMouseUp:
                    RaiseTrayRightMouseUpEvent();
                    break;
                case MouseEvent.IconLeftMouseUp:
                    RaiseTrayLeftMouseUpEvent();
                    RaiseTrayMouseClickEvent();
                    break;
                case MouseEvent.IconMiddleMouseDown:
                    RaiseTrayMiddleMouseDownEvent();
                    break;
                case MouseEvent.IconMiddleMouseUp:
                    RaiseTrayMiddleMouseUpEvent();
                    break;
                case MouseEvent.IconDoubleClick:
                    //cancel single click timer
                    _SingleClickTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    //bubble event
                    RaiseTrayMouseDoubleClickEvent();
                    break;
                case MouseEvent.BalloonToolTipClicked:
                    RaiseTrayBalloonTipClickedEvent();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(me), $@"Missing handler for mouse event flag: {me}");
            }


            //get mouse coordinates
            var cursor_position = new WinApi.Point();
            //physical cursor position is supported for Vista and above
            if (_MessageSink.Version == NotifyIconVersion.Vista)
                WinApi.GetPhysicalCursorPos(ref cursor_position);
            else
                WinApi.GetCursorPos(ref cursor_position);

            cursor_position = GetDeviceCoordinates(cursor_position);

            var isLeftClickCommandInvoked = false;

            //show popup, if requested
            if (me.IsMatch(PopupActivation))
            {
                if (me == MouseEvent.IconLeftMouseUp)
                {
                    //show popup once we are sure it's not a double click
                    _SingleClickTimerAction = () =>
                    {
                        LeftClickCommand.ExecuteIfEnabled(LeftClickCommandParameter, LeftClickCommandTarget ?? this);
                        ShowTrayPopup(cursor_position);
                    };
                    _SingleClickTimer.Change(WinApi.GetDoubleClickTime(), Timeout.Infinite);
                    isLeftClickCommandInvoked = true;
                }
                else //show popup immediately
                    ShowTrayPopup(cursor_position);
            }


            //show context menu, if requested
            if (me.IsMatch(MenuActivation))
            {
                if (me == MouseEvent.IconLeftMouseUp)
                {
                    //show context menu once we are sure it's not a double click
                    _SingleClickTimerAction = () =>
                    {
                        LeftClickCommand.ExecuteIfEnabled(LeftClickCommandParameter, LeftClickCommandTarget ?? this);
                        ShowContextMenu(cursor_position);
                    };
                    _SingleClickTimer.Change(WinApi.GetDoubleClickTime(), Timeout.Infinite);
                    isLeftClickCommandInvoked = true;
                }
                else //show context menu immediately
                    ShowContextMenu(cursor_position);
            }

            //make sure the left click command is invoked on mouse clicks
            if (me != MouseEvent.IconLeftMouseUp || isLeftClickCommandInvoked) return;
            //show context menu once we are sure it's not a double click
            _SingleClickTimerAction = () => LeftClickCommand.ExecuteIfEnabled(LeftClickCommandParameter, LeftClickCommandTarget ?? this);
            _SingleClickTimer.Change(WinApi.GetDoubleClickTime(), Timeout.Infinite);
        }

        #endregion

        #region ToolTips

        /// <summary>
        /// Displays a custom tooltip, if available. This method is only
        /// invoked for Windows Vista and above.
        /// </summary>
        /// <param name="visible">Whether to show or hide the tooltip.</param>
        private void OnToolTipChange(bool visible)
        {
            //if we don't have a tooltip, there's nothing to do here...
            if (TrayToolTipResolved is null) return;

            if (visible)
            {
                //ignore if we are already displaying something down there
                if (IsPopupOpen) return;

                var args = RaisePreviewTrayToolTipOpenEvent();
                if (args?.Handled != true) return;

                TrayToolTipResolved.IsOpen = true;

                //raise attached event first
                if (TrayToolTip != null) RaiseToolTipOpenedEvent(TrayToolTip);

                //bubble routed event
                RaiseTrayToolTipOpenEvent();
            }
            else
            {
                var args = RaisePreviewTrayToolTipCloseEvent();
                if (args?.Handled != true) return;

                //raise attached event first
                if (TrayToolTip != null) RaiseToolTipCloseEvent(TrayToolTip);

                TrayToolTipResolved.IsOpen = false;

                //bubble event
                RaiseTrayToolTipCloseEvent();
            }
        }


        /// <summary>
        /// Creates a <see cref="ToolTip"/> control that either
        /// wraps the currently set <see cref="TrayToolTip"/>
        /// control or the <see cref="ToolTipText"/> string.<br/>
        /// If <see cref="TrayToolTip"/> itself is already
        /// a <see cref="ToolTip"/> instance, it will be used directly.
        /// </summary>
        /// <remarks>We use a <see cref="ToolTip"/> rather than
        /// <see cref="Popup"/> because there was no way to prevent a
        /// popup from causing cyclic open/close commands if it was
        /// placed under the mouse. ToolTip internally uses a Popup of
        /// its own, but takes advance of Popup's internal <see cref="UIElement.IsHitTestVisible"/>
        /// property which prevents this issue.</remarks>
        private void CreateCustomToolTip()
        {
            //check if the item itself is a tooltip
            var tool_tip = TrayToolTip as ToolTip;

            if (tool_tip is null && TrayToolTip != null)
            {
                //create an invisible wrapper tooltip that hosts the UIElement
                tool_tip = new ToolTip
                {
                    Placement = PlacementMode.Mouse,
                    HasDropShadow = false,
                    BorderThickness = new Thickness(0),
                    Background = System.Windows.Media.Brushes.Transparent,
                    StaysOpen = true,
                    Content = TrayToolTip
                };

                //do *not* set the placement target, as it causes the popup to become hidden if the
                //TaskbarIcon's parent is hidden, too. At runtime, the parent can be resolved through
                //the ParentTaskbarIcon attached dependency property:
                //tt.PlacementTarget = this;

                //make sure the tooltip is invisible

                //setting the 
            }
            else if (tool_tip is null && !string.IsNullOrEmpty(ToolTipText))
            {
                //create a simple tooltip for the ToolTipText string
                tool_tip = new ToolTip { Content = ToolTipText };
            }

            //the tooltip explicitly gets the DataContext of this instance.
            //If there is no DataContext, the TaskbarIcon assigns itself
            if (tool_tip != null)
                UpdateDataContext(tool_tip, null, DataContext);

            //store a reference to the used tooltip
            SetTrayToolTipResolved(tool_tip);
        }


        /// <summary>
        /// Sets tooltip settings for the class depending on defined
        /// dependency properties and OS support.
        /// </summary>
        private void WriteToolTipSettings()
        {
            const IconDataMembers flags = IconDataMembers.Tip;
            _IconData.ToolTipText = ToolTipText;

            if (_MessageSink.Version == NotifyIconVersion.Vista)
            {
                //we need to set a tooltip text to get tooltip events from the
                //taskbar icon
                if (string.IsNullOrEmpty(_IconData.ToolTipText) && TrayToolTipResolved != null)
                {
                    //if we have not tooltip text but a custom tooltip, we
                    //need to set a dummy value (we're displaying the ToolTip control, not the string)
                    _IconData.ToolTipText = "ToolTip";
                }
            }

            //update the tooltip text
            TaskBarIconUtilities.WriteIconData(ref _IconData, NotifyCommand.Modify, flags);
        }

        #endregion

        #region Custom Popup

        /// <summary>
        /// Creates a <see cref="ToolTip"/> control that either
        /// wraps the currently set <see cref="TrayToolTip"/>
        /// control or the <see cref="ToolTipText"/> string.<br/>
        /// If <see cref="TrayToolTip"/> itself is already
        /// a <see cref="ToolTip"/> instance, it will be used directly.
        /// </summary>
        /// <remarks>We use a <see cref="ToolTip"/> rather than
        /// <see cref="Popup"/> because there was no way to prevent a
        /// popup from causing cyclic open/close commands if it was
        /// placed under the mouse. ToolTip internally uses a Popup of
        /// its own, but takes advance of Popup's internal <see cref="UIElement.IsHitTestVisible"/>
        /// property which prevents this issue.</remarks>
        private void CreatePopup()
        {
            //check if the item itself is a popup
            var popup = TrayPopup as Popup;

            if (popup is null && TrayPopup != null)
            {
                //create an invisible popup that hosts the UIElement
                popup = new Popup
                {
                    AllowsTransparency = true,
                    //don't animate by default - dev can use attached events or override
                    PopupAnimation = PopupAnimation.None,
                    Child = TrayPopup,
                    Placement = PlacementMode.AbsolutePoint,
                    StaysOpen = false
                };


                //the CreateRootPopup method outputs binding errors in the debug window because
                //it tries to bind to "Popup-specific" properties in case they are provided by the child.
                //We don't need that so just assign the control as the child.

                //do *not* set the placement target, as it causes the popup to become hidden if the
                //TaskbarIcon's parent is hidden, too. At runtime, the parent can be resolved through
                //the ParentTaskbarIcon attached dependency property:
                //popup.PlacementTarget = this;

            }

            //the popup explicitly gets the DataContext of this instance.
            //If there is no DataContext, the TaskbarIcon assigns itself
            if (popup != null)
                UpdateDataContext(popup, null, DataContext);

            //store a reference to the used tooltip
            SetTrayPopupResolved(popup);
        }


        /// <summary>
        /// Displays the <see cref="TrayPopup"/> control if
        /// it was set.
        /// </summary>
        private void ShowTrayPopup(WinApi.Point CursorPosition)
        {
            if (IsDisposed) return;

            //raise preview event no matter whether popup is currently set
            //or not (enables client to set it on demand)
            if (RaisePreviewTrayPopupOpenEvent()?.Handled != true) return;

            if (TrayPopup is null) return;
            //use absolute position, but place the popup centered above the icon
            var tray_popup = TrayPopupResolved ?? throw new InvalidOperationException();
            tray_popup.Placement = PlacementMode.AbsolutePoint;
            tray_popup.HorizontalOffset = CursorPosition.X;
            tray_popup.VerticalOffset = CursorPosition.Y;

            //open popup
            tray_popup.IsOpen = true;

            var handle = IntPtr.Zero;
            if (tray_popup.Child != null)
            {
                //try to get a handle on the popup itself (via its child)
                var source = (HwndSource)PresentationSource.FromVisual(tray_popup.Child);
                if (source != null) handle = source.Handle;
            }

            //if we don't have a handle for the popup, fall back to the message sink
            if (handle == IntPtr.Zero) handle = _MessageSink.MessageWindowHandle;

            //activate either popup or message sink to track deactivation.
            //otherwise, the popup does not close if the user clicks somewhere else
            WinApi.SetForegroundWindow(handle);

            //raise attached event - item should never be null unless developers
            //changed the CustomPopup directly...
            if (TrayPopup != null) RaisePopupOpenedEvent(TrayPopup);

            //bubble routed event
            RaiseTrayPopupOpenEvent();
        }

        #endregion

        #region Context Menu

        /// <summary>
        /// Displays the <see cref="ContextMenu"/> if
        /// it was set.
        /// </summary>
        private void ShowContextMenu(WinApi.Point CursorPosition)
        {
            if (IsDisposed) return;

            //raise preview event no matter whether context menu is currently set
            //or not (enables client to set it on demand)
            var args = RaisePreviewTrayContextMenuOpenEvent();
            if (args?.Handled != true) return;

            if (ContextMenu is null) return;
            //use absolute positioning. We need to set the coordinates, or a delayed opening
            //(e.g. when left-clicked) opens the context menu at the wrong place if the mouse
            //is moved!
            ContextMenu.Placement = PlacementMode.AbsolutePoint;
            ContextMenu.HorizontalOffset = CursorPosition.X;
            ContextMenu.VerticalOffset = CursorPosition.Y;
            ContextMenu.IsOpen = true;

            var handle = IntPtr.Zero;

            //try to get a handle on the context itself
            var source = (HwndSource)PresentationSource.FromVisual(ContextMenu);
            if (source != null) handle = source.Handle;

            //if we don't have a handle for the popup, fall back to the message sink
            if (handle == IntPtr.Zero) handle = _MessageSink.MessageWindowHandle;

            //activate the context menu or the message window to track deactivation - otherwise, the context menu
            //does not close if the user clicks somewhere else. With the message window
            //fallback, the context menu can't receive keyboard events - should not happen though
            WinApi.SetForegroundWindow(handle);

            //bubble event
            RaiseTrayContextMenuOpenEvent();
        }

        #endregion

        #region Balloon Tips

        /// <summary>
        /// Bubbles events if a balloon ToolTip was displayed
        /// or removed.
        /// </summary>
        /// <param name="visible">Whether the ToolTip was just displayed
        /// or removed.</param>
        private void OnBalloonToolTipChanged(bool visible)
        {
            if (visible)
                RaiseTrayBalloonTipShownEvent();
            else
                RaiseTrayBalloonTipClosedEvent();
        }

        /// <summary>
        /// Displays a balloon tip with the specified title,
        /// text, and icon in the taskbar for the specified time period.
        /// </summary>
        /// <param name="Title">The title to display on the balloon tip.</param>
        /// <param name="Message">The text to display on the balloon tip.</param>
        /// <param name="symbol">A symbol that indicates the severity.</param>
        public void ShowBalloonTip(string Title, string Message, BalloonIcon symbol)
        {
            lock (this) ShowBalloonTip(Title, Message, symbol.GetBalloonFlag(), IntPtr.Zero);
        }


        /// <summary>
        /// Displays a balloon tip with the specified title,
        /// text, and a custom icon in the taskbar for the specified time period.
        /// </summary>
        /// <param name="Title">The title to display on the balloon tip.</param>
        /// <param name="Message">The text to display on the balloon tip.</param>
        /// <param name="customIcon">A custom icon.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="customIcon"/>
        /// is a null reference.</exception>
        public void ShowBalloonTip(string Title, string Message, Icon customIcon)
        {
            if (customIcon is null) throw new ArgumentNullException(nameof(customIcon));

            lock (this) ShowBalloonTip(Title, Message, BalloonFlags.User, customIcon.Handle);
        }


        /// <summary>
        /// Invokes <see cref="WinApi.Shell_NotifyIcon"/> in order to display
        /// a given balloon ToolTip.
        /// </summary>
        /// <param name="Title">The title to display on the balloon tip.</param>
        /// <param name="Message">The text to display on the balloon tip.</param>
        /// <param name="Flags">Indicates what icon to use.</param>
        /// <param name="BalloonIconHandle">A handle to a custom icon, if any, or
        /// <see cref="IntPtr.Zero"/>.</param>
        private void ShowBalloonTip(string Title, string Message, BalloonFlags Flags, IntPtr BalloonIconHandle)
        {
            EnsureNotDisposed();

            _IconData.BalloonText = Message ?? string.Empty;
            _IconData.BalloonTitle = Title ?? string.Empty;

            _IconData.BalloonFlags = Flags;
            _IconData.CustomBalloonIconHandle = BalloonIconHandle;
            TaskBarIconUtilities.WriteIconData(ref _IconData, NotifyCommand.Modify, IconDataMembers.Info | IconDataMembers.Icon);
        }


        /// <summary>
        /// Hides a balloon ToolTip, if any is displayed.
        /// </summary>
        public void HideBalloonTip()
        {
            EnsureNotDisposed();

            //reset balloon by just setting the info to an empty string
            _IconData.BalloonText = _IconData.BalloonTitle = string.Empty;
            TaskBarIconUtilities.WriteIconData(ref _IconData, NotifyCommand.Modify, IconDataMembers.Info);
        }

        #endregion

        #region Single Click Timer event

        /// <summary>
        /// Performs a delayed action if the user requested an action
        /// based on a single click of the left mouse.<br/>
        /// This method is invoked by the <see cref="_SingleClickTimer"/>.
        /// </summary>
        private void DoSingleClickAction(object? state)
        {
            if (IsDisposed) return;

            //run action
            var action = _SingleClickTimerAction;
            if (action is null) return;
            //cleanup action
            _SingleClickTimerAction = null;

            //switch to UI thread
            this.GetDispatcher().NotNull().Invoke(action);
        }

        #endregion

        #region Set Version (API)

        /// <summary>
        /// Sets the version flag for the <see cref="_IconData"/>.
        /// </summary>
        private void SetVersion()
        {
            _IconData.VersionOrTimeout = (uint)NotifyIconVersion.Vista;
            var status = WinApi.Shell_NotifyIcon(NotifyCommand.SetVersion, ref _IconData);

            if (!status)
            {
                _IconData.VersionOrTimeout = (uint)NotifyIconVersion.Win2000;
                status = TaskBarIconUtilities.WriteIconData(ref _IconData, NotifyCommand.SetVersion);
            }

            if (!status)
            {
                _IconData.VersionOrTimeout = (uint)NotifyIconVersion.Win95;
                status = TaskBarIconUtilities.WriteIconData(ref _IconData, NotifyCommand.SetVersion);
            }

            if (!status)
                Debug.Fail("Could not set version");
        }

        #endregion

        #region Create / Remove Taskbar Icon

        /// <summary>
        /// Recreates the taskbar icon if the whole taskbar was
        /// recreated (e.g. because Explorer was shut down).
        /// </summary>
        private void OnTaskbarCreated()
        {
            IsTaskbarIconCreated = false;
            CreateTaskbarIcon();
        }


        /// <summary>
        /// Creates the taskbar icon. This message is invoked during initialization,
        /// if the taskbar is restarted, and whenever the icon is displayed.
        /// </summary>
        private void CreateTaskbarIcon()
        {
            lock (this)
            {
                if (IsTaskbarIconCreated) return;
                const IconDataMembers members = IconDataMembers.Message
                                                | IconDataMembers.Icon
                                                | IconDataMembers.Tip;

                //write initial configuration
                var status = TaskBarIconUtilities.WriteIconData(ref _IconData, NotifyCommand.Add, members);
                if (!status)
                {
                    //couldn't create the icon - we can assume this is because explorer is not running (yet!)
                    //-> try a bit later again rather than throwing an exception. Typically, if the windows
                    // shell is being loaded later, this method is being reinvoked from OnTaskbarCreated
                    // (we could also retry after a delay, but that's currently YAGNI)
                    return;
                }

                //set to most recent version
                SetVersion();
                _MessageSink.Version = (NotifyIconVersion)_IconData.VersionOrTimeout;

                IsTaskbarIconCreated = true;
            }
        }

        /// <summary>
        /// Closes the taskbar icon if required.
        /// </summary>
        private void RemoveTaskbarIcon()
        {
            lock (this)
            {
                //make sure we didn't schedule a creation

                if (!IsTaskbarIconCreated) return;
                TaskBarIconUtilities.WriteIconData(ref _IconData, NotifyCommand.Delete, IconDataMembers.Message);
                IsTaskbarIconCreated = false;
            }
        }

        #endregion

        /// <summary>
        /// Recalculates OS coordinates in order to support WPFs coordinate
        /// system if OS scaling (DPIs) is not 100%.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private WinApi.Point GetDeviceCoordinates(WinApi.Point point)
        {
            if (double.IsNaN(_ScalingFactor))
            {
                //calculate scaling factor in order to support non-standard DPIs
                var presentationSource = PresentationSource.FromVisual(this);
                if (presentationSource is null)
                    _ScalingFactor = 1;
                else
                {
                    Debug.Assert(presentationSource.CompositionTarget != null, "presentationSource.CompositionTarget != null");
                    var transform = presentationSource.CompositionTarget.TransformToDevice;
                    _ScalingFactor = 1 / transform.M11;
                }
            }

            //on standard DPI settings, just return the point
            return _ScalingFactor.Equals(1d) ? point : new WinApi.Point { X = (int)(point.X * _ScalingFactor), Y = (int)(point.Y * _ScalingFactor) };
        }

        #region Dispose / Exit

        /// <summary>
        /// Set to true as soon as <c>Dispose</c> has been invoked.
        /// </summary>
        public bool IsDisposed { get; private set; }


        /// <summary>
        /// Checks if the object has been disposed and
        /// raises a <see cref="ObjectDisposedException"/> in case
        /// the <see cref="IsDisposed"/> flag is true.
        /// </summary>
        private void EnsureNotDisposed()
        {
            if (IsDisposed) throw new ObjectDisposedException(Name ?? GetType().FullName);
        }


        /// <summary>
        /// Disposes the class if the application exits.
        /// </summary>
        private void OnExit(object sender, EventArgs e) => Dispose();


        /// <summary>
        /// This destructor will run only if the <see cref="Dispose()"/>
        /// method does not get called. This gives this base class the
        /// opportunity to finalize.
        /// <para>
        /// Important: Do not provide destructors in types derived from
        /// this class.
        /// </para>
        /// </summary>
        ~TaskbarIcon() => Dispose(false);


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
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Closes the tray and releases all resources.
        /// </summary>
        /// <summary>
        /// <c>Dispose(bool disposing)</c> executes in two distinct scenarios.
        /// If disposing equals <c>true</c>, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// </summary>
        /// <param name="disposing">If disposing equals <c>false</c>, the method
        /// has been called by the runtime from inside the finalizer and you
        /// should not reference other objects. Only unmanaged resources can
        /// be disposed.</param>
        /// <remarks>Check the <see cref="IsDisposed"/> property to determine whether
        /// the method has already been called.</remarks>
        protected virtual void Dispose(bool disposing)
        {
            //don't do anything if the component is already disposed
            if (IsDisposed || !disposing) return;

            lock (this)
            {
                IsDisposed = true;

                //unregister application event listener
                if (Application.Current != null)
                    Application.Current.Exit -= OnExit;

                //stop timers
                _SingleClickTimer.Dispose();
                _BalloonCloseTimer.Dispose();

                //dispose message sink
                _MessageSink.Dispose();

                //remove icon
                RemoveTaskbarIcon();
            }
        }

        #endregion
    }

}