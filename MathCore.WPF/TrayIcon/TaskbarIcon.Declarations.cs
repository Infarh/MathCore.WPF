using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
// ReSharper disable UnusedMethodReturnValue.Global

namespace MathCore.WPF.TrayIcon
{
    /// <summary>
    /// Contains declarations of WPF dependency properties
    /// and events.
    /// </summary>
    partial class TaskbarIcon
    {
        /// <summary>
        /// Category name that is set on designer properties.
        /// </summary>
        private const string CategoryName = "NotifyIcon";


        //POPUP CONTROLS

        #region TrayPopupResolved

        /// <summary>
        /// TrayPopupResolved Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey TrayPopupResolvedPropertyKey
            = DependencyProperty.RegisterReadOnly("TrayPopupResolved", typeof(Popup), typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null));


        /// <summary>
        /// A read-only dependency property that returns the <see cref="Popup"/>
        /// that is being displayed in the taskbar area based on a user action.
        /// </summary>
        public static readonly DependencyProperty TrayPopupResolvedProperty
            = TrayPopupResolvedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the TrayPopupResolved property. Returns
        /// a <see cref="Popup"/> which is either the
        /// <see cref="TrayPopup"/> control itself or a
        /// <see cref="Popup"/> control that contains the
        /// <see cref="TrayPopup"/>.
        /// </summary>
        [Category(CategoryName)]
        public Popup TrayPopupResolved => (Popup)GetValue(TrayPopupResolvedProperty);

        /// <summary>
        /// Provides a secure method for setting the TrayPopupResolved property.  
        /// This dependency property indicates ....
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        protected void SetTrayPopupResolved(Popup value) => SetValue(TrayPopupResolvedPropertyKey, value);

        #endregion

        #region TrayToolTipResolved

        /// <summary>
        /// TrayToolTipResolved Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey TrayToolTipResolvedPropertyKey
            = DependencyProperty.RegisterReadOnly("TrayToolTipResolved", typeof(ToolTip), typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null));


        /// <summary>
        /// A read-only dependency property that returns the <see cref="ToolTip"/>
        /// that is being displayed.
        /// </summary>
        public static readonly DependencyProperty TrayToolTipResolvedProperty
            = TrayToolTipResolvedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the TrayToolTipResolved property. Returns 
        /// a <see cref="ToolTip"/> control that was created
        /// in order to display either <see cref="TrayToolTip"/>
        /// or <see cref="ToolTipText"/>.
        /// </summary>
        [Category(CategoryName)]
        [Browsable(true)]
        [Bindable(true)]
        public ToolTip TrayToolTipResolved => (ToolTip)GetValue(TrayToolTipResolvedProperty);

        /// <summary>
        /// Provides a secure method for setting the <see cref="TrayToolTipResolved"/>
        /// property.  
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        protected void SetTrayToolTipResolved(ToolTip value) => SetValue(TrayToolTipResolvedPropertyKey, value);

        #endregion

        #region CustomBalloon

        /// <summary>
        /// CustomBalloon Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey CustomBalloonPropertyKey
            = DependencyProperty.RegisterReadOnly("CustomBalloon", typeof(Popup), typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Maintains a currently displayed custom balloon.
        /// </summary>
        public static readonly DependencyProperty CustomBalloonProperty
            = CustomBalloonPropertyKey.DependencyProperty;

        /// <summary>
        /// A custom popup that is being displayed in the tray area in order
        /// to display messages to the user.
        /// </summary>
        public Popup CustomBalloon => (Popup)GetValue(CustomBalloonProperty);

        /// <summary>
        /// Provides a secure method for setting the <see cref="CustomBalloon"/> property.  
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        protected void SetCustomBalloon(Popup value) => SetValue(CustomBalloonPropertyKey, value);

        #endregion

        //DEPENDENCY PROPERTIES

        #region Icon property / IconSource dependency property

        private Icon icon;

        /// <summary>
        /// Gets or sets the icon to be displayed. This is not a
        /// dependency property - if you want to assign the property
        /// through XAML, please use the <see cref="IconSource"/>
        /// dependency property.
        /// </summary>
        [Browsable(false)]
        public Icon Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                iconData.IconHandle = value is null ? IntPtr.Zero : icon.Handle;

                TaskBarIconUtilities.WriteIconData(ref iconData, NotifyCommand.Modify, IconDataMembers.Icon);
            }
        }


        /// <summary>
        /// Resolves an image source and updates the <see cref="Icon" /> property accordingly.
        /// </summary>
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource",
                typeof(ImageSource),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null, IconSourcePropertyChanged));

        /// <summary>
        /// A property wrapper for the <see cref="IconSourceProperty"/>
        /// dependency property:<br/>
        /// Resolves an image source and updates the <see cref="Icon" /> property accordingly.
        /// </summary>
        [Category(CategoryName)]
        [Description("Sets the displayed taskbar icon.")]
        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }


        /// <summary>
        /// A static callback listener which is being invoked if the
        /// <see cref="IconSourceProperty"/> dependency property has
        /// been changed. Invokes the <see cref="OnIconSourcePropertyChanged"/>
        /// instance method of the changed instance.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void IconSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TaskbarIcon)d).OnIconSourcePropertyChanged(e);


        /// <summary>
        /// Handles changes of the <see cref="IconSourceProperty"/> dependency property. As
        /// WPF privately uses the dependency property system and bypasses the
        /// <see cref="IconSource"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        private void OnIconSourcePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            //resolving the ImageSource at design time is unlikely to work
            if(!TaskBarIconUtilities.IsDesignMode) Icon = ((ImageSource)e.NewValue).ToIcon();
        }

        #endregion

        #region ToolTipText dependency property

        /// <summary>
        /// A tooltip text that is being displayed if no custom <see cref="ToolTip"/>
        /// was set or if custom tooltips are not supported.
        /// </summary>
        public static readonly DependencyProperty ToolTipTextProperty =
            DependencyProperty.Register("ToolTipText",
                typeof(string),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(string.Empty, ToolTipTextPropertyChanged));


        /// <summary>
        /// A property wrapper for the <see cref="ToolTipTextProperty"/>
        /// dependency property:<br/>
        /// A tooltip text that is being displayed if no custom <see cref="ToolTip"/>
        /// was set or if custom tooltips are not supported.
        /// </summary>
        [Category(CategoryName)]
        [Description("Alternative to a fully blown ToolTip, which is only displayed on Vista and above.")]
        public string ToolTipText
        {
            get { return (string)GetValue(ToolTipTextProperty); }
            set { SetValue(ToolTipTextProperty, value); }
        }


        /// <summary>
        /// A static callback listener which is being invoked if the
        /// <see cref="ToolTipTextProperty"/> dependency property has
        /// been changed. Invokes the <see cref="OnToolTipTextPropertyChanged"/>
        /// instance method of the changed instance.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void ToolTipTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TaskbarIcon)d).OnToolTipTextPropertyChanged(e);


        /// <summary>
        /// Handles changes of the <see cref="ToolTipTextProperty"/> dependency property. As
        /// WPF privately uses the dependency property system and bypasses the
        /// <see cref="ToolTipText"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        private void OnToolTipTextPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            //do not touch tooltips if we have a custom tooltip element
            if(TrayToolTip is null)
            {
                var currentToolTip = TrayToolTipResolved;
                if(currentToolTip is null) //if we don't have a wrapper tooltip for the tooltip text, create it now
                    CreateCustomToolTip();
                else //if we have a wrapper tooltip that shows the old tooltip text, just update content
                    currentToolTip.Content = e.NewValue;
            }

            WriteToolTipSettings();
        }

        #endregion

        #region TrayToolTip dependency property

        /// <summary>
        /// A custom UI element that is displayed as a tooltip if the user hovers over the taskbar icon.
        /// Works only with Vista and above. Accordingly, you should make sure that
        /// the <see cref="ToolTipText"/> property is set as well.
        /// </summary>
        public static readonly DependencyProperty TrayToolTipProperty =
            DependencyProperty.Register("TrayToolTip",
                typeof(UIElement),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null, TrayToolTipPropertyChanged));

        /// <summary>
        /// A property wrapper for the <see cref="TrayToolTipProperty"/>
        /// dependency property:<br/>
        /// A custom UI element that is displayed as a tooltip if the user hovers over the taskbar icon.
        /// Works only with Vista and above. Accordingly, you should make sure that
        /// the <see cref="ToolTipText"/> property is set as well.
        /// </summary>
        [Category(CategoryName)]
        [Description("Custom UI element that is displayed as a tooltip. Only on Vista and above")]
        public UIElement TrayToolTip
        {
            get { return (UIElement)GetValue(TrayToolTipProperty); }
            set { SetValue(TrayToolTipProperty, value); }
        }


        /// <summary>
        /// A static callback listener which is being invoked if the
        /// <see cref="TrayToolTipProperty"/> dependency property has
        /// been changed. Invokes the <see cref="OnTrayToolTipPropertyChanged"/>
        /// instance method of the changed instance.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void TrayToolTipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TaskbarIcon)d).OnTrayToolTipPropertyChanged(e);


        /// <summary>
        /// Handles changes of the <see cref="TrayToolTipProperty"/> dependency property. As
        /// WPF privately uses the dependency property system and bypasses the
        /// <see cref="TrayToolTip"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        private void OnTrayToolTipPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            //recreate tooltip control
            CreateCustomToolTip();

            if(e.OldValue != null)
                //remove the taskbar icon reference from the previously used element
                SetParentTaskbarIcon((DependencyObject)e.OldValue, null);

            if(e.NewValue != null)
                //set this taskbar icon as a reference to the new tooltip element
                SetParentTaskbarIcon((DependencyObject)e.NewValue, this);

            //update tooltip settings - needed to make sure a string is set, even
            //if the ToolTipText property is not set. Otherwise, the event that
            //triggers tooltip display is never fired.
            WriteToolTipSettings();
        }

        #endregion

        #region TrayPopup dependency property

        /// <summary>
        /// A control that is displayed as a popup when the taskbar icon is clicked.
        /// </summary>
        public static readonly DependencyProperty TrayPopupProperty =
            DependencyProperty.Register("TrayPopup",
                typeof(UIElement),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null, TrayPopupPropertyChanged));

        /// <summary>
        /// A property wrapper for the <see cref="TrayPopupProperty"/>
        /// dependency property:<br/>
        /// A control that is displayed as a popup when the taskbar icon is clicked.
        /// </summary>
        [Category(CategoryName)]
        [Description("Displayed as a Popup if the user clicks on the taskbar icon.")]
        public UIElement TrayPopup
        {
            get { return (UIElement)GetValue(TrayPopupProperty); }
            set { SetValue(TrayPopupProperty, value); }
        }


        /// <summary>
        /// A static callback listener which is being invoked if the
        /// <see cref="TrayPopupProperty"/> dependency property has
        /// been changed. Invokes the <see cref="OnTrayPopupPropertyChanged"/>
        /// instance method of the changed instance.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void TrayPopupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TaskbarIcon)d).OnTrayPopupPropertyChanged(e);


        /// <summary>
        /// Handles changes of the <see cref="TrayPopupProperty"/> dependency property. As
        /// WPF privately uses the dependency property system and bypasses the
        /// <see cref="TrayPopup"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        private void OnTrayPopupPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue != null)
                //remove the taskbar icon reference from the previously used element
                SetParentTaskbarIcon((DependencyObject)e.OldValue, null);


            if(e.NewValue != null)
                //set this taskbar icon as a reference to the new tooltip element
                SetParentTaskbarIcon((DependencyObject)e.NewValue, this);

            //create a pop
            CreatePopup();
        }

        #endregion

        #region MenuActivation dependency property

        /// <summary>
        /// Defines what mouse events display the context menu.
        /// Defaults to <see cref="PopupActivationMode.RightClick"/>.
        /// </summary>
        public static readonly DependencyProperty MenuActivationProperty =
            DependencyProperty.Register("MenuActivation",
                typeof(PopupActivationMode),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(PopupActivationMode.RightClick));

        /// <summary>
        /// A property wrapper for the <see cref="MenuActivationProperty"/>
        /// dependency property:<br/>
        /// Defines what mouse events display the context menu.
        /// Defaults to <see cref="PopupActivationMode.RightClick"/>.
        /// </summary>
        [Category(CategoryName)]
        [Description("Defines what mouse events display the context menu.")]
        public PopupActivationMode MenuActivation
        {
            get { return (PopupActivationMode)GetValue(MenuActivationProperty); }
            set { SetValue(MenuActivationProperty, value); }
        }

        #endregion

        #region PopupActivation dependency property

        /// <summary>
        /// Defines what mouse events trigger the <see cref="TrayPopup" />.
        /// Default is <see cref="PopupActivationMode.LeftClick" />.
        /// </summary>
        public static readonly DependencyProperty PopupActivationProperty =
            DependencyProperty.Register("PopupActivation",
                typeof(PopupActivationMode),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(PopupActivationMode.LeftClick));

        /// <summary>
        /// A property wrapper for the <see cref="PopupActivationProperty"/>
        /// dependency property:<br/>
        /// Defines what mouse events trigger the <see cref="TrayPopup" />.
        /// Default is <see cref="PopupActivationMode.LeftClick" />.
        /// </summary>
        [Category(CategoryName)]
        [Description("Defines what mouse events display the TaskbarIconPopup.")]
        public PopupActivationMode PopupActivation
        {
            get { return (PopupActivationMode)GetValue(PopupActivationProperty); }
            set { SetValue(PopupActivationProperty, value); }
        }

        #endregion

        #region Visibility dependency property override

        /// <summary>
        /// A static callback listener which is being invoked if the
        /// <see cref="UIElement.VisibilityProperty"/> dependency property has
        /// been changed. Invokes the <see cref="OnVisibilityPropertyChanged"/>
        /// instance method of the changed instance.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void VisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TaskbarIcon)d).OnVisibilityPropertyChanged(e);


        /// <summary>
        /// Handles changes of the <see cref="UIElement.VisibilityProperty"/> dependency property. As
        /// WPF privately uses the dependency property system and bypasses the
        /// <see cref="Visibility"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        private void OnVisibilityPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if((Visibility)e.NewValue == Visibility.Visible)
                CreateTaskbarIcon();
            else
                RemoveTaskbarIcon();
        }

        #endregion

        #region DataContext dependency property override / target update

        /// <summary>
        /// Updates the <see cref="FrameworkElement.DataContextProperty"/> of a given
        /// <see cref="FrameworkElement"/>. This method only updates target elements
        /// that do not already have a data context of their own, and either assigns
        /// the <see cref="FrameworkElement.DataContext"/> of the NotifyIcon, or the
        /// NotifyIcon itself, if no data context was assigned at all.
        /// </summary>
        private void UpdateDataContext(FrameworkElement target, object oldDataContextValue, object newDataContextValue)
        {
            //if there is no target or it's data context is determined through a binding
            //of its own, keep it
            if(target is null || target.IsDataContextDataBound()) return;

            //if the target's data context is the NotifyIcon's old DataContext or the NotifyIcon itself,
            //update it
            if(ReferenceEquals(this, target.DataContext) || Equals(oldDataContextValue, target.DataContext))
                //assign own data context, if available. If there is no data
                //context at all, assign NotifyIcon itself.
                target.DataContext = newDataContextValue ?? this;
        }

        /// <summary>
        /// A static callback listener which is being invoked if the
        /// <see cref="FrameworkElement.DataContextProperty"/> dependency property has
        /// been changed. Invokes the <see cref="OnDataContextPropertyChanged"/>
        /// instance method of the changed instance.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void DataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TaskbarIcon)d).OnDataContextPropertyChanged(e);


        /// <summary>
        /// Handles changes of the <see cref="FrameworkElement.DataContextProperty"/> dependency property. As
        /// WPF privately uses the dependency property system and bypasses the
        /// <see cref="FrameworkElement.DataContext"/> property wrapper, updates of the property's value
        /// should be handled here.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        private void OnDataContextPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var new_value = e.NewValue;
            var old_value = e.OldValue;

            //replace custom data context for ToolTips, Popup, and
            //ContextMenu
            UpdateDataContext(TrayPopupResolved, old_value, new_value);
            UpdateDataContext(TrayToolTipResolved, old_value, new_value);
            UpdateDataContext(ContextMenu, old_value, new_value);
        }

        #endregion

        #region ContextMenu dependency property override

        /// <summary>
        /// A static callback listener which is being invoked if the
        /// <see cref="FrameworkElement.ContextMenuProperty"/> dependency property has
        /// been changed. Invokes the <see cref="OnContextMenuPropertyChanged"/>
        /// instance method of the changed instance.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void ContextMenuPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TaskbarIcon)d).OnContextMenuPropertyChanged(e);


        /// <summary>
        /// Releases the old and updates the new <see cref="ContextMenu"/> property
        /// in order to reflect both the NotifyIcon's <see cref="FrameworkElement.DataContext"/>
        /// property and have the <see cref="ParentTaskbarIconProperty"/> assigned.
        /// </summary>
        /// <param name="e">Provides information about the updated property.</param>
        private void OnContextMenuPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue != null)
                //remove the taskbar icon reference from the previously used element
                SetParentTaskbarIcon((DependencyObject)e.OldValue, null);

            if(e.NewValue != null)
                //set this taskbar icon as a reference to the new tooltip element
                SetParentTaskbarIcon((DependencyObject)e.NewValue, this);

            UpdateDataContext((ContextMenu)e.NewValue, null, DataContext);
        }

        #endregion

        #region DoubleClickCommand dependency property

        /// <summary>
        /// Associates a command that is being executed if the tray icon is being
        /// double clicked.
        /// </summary>
        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.Register("DoubleClickCommand",
                typeof(ICommand),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// A property wrapper for the <see cref="DoubleClickCommandProperty"/>
        /// dependency property:<br/>
        /// Associates a command that is being executed if the tray icon is being
        /// double clicked.
        /// </summary>
        [Category(CategoryName)]
        [Description("A command that is being executed if the tray icon is being double-clicked.")]
        public ICommand DoubleClickCommand
        {
            get { return (ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }

        #endregion

        #region DoubleClickCommandParameter dependency property

        /// <summary>
        /// Command parameter for the <see cref="DoubleClickCommand"/>.
        /// </summary>
        public static readonly DependencyProperty DoubleClickCommandParameterProperty =
            DependencyProperty.Register("DoubleClickCommandParameter",
                typeof(object),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// A property wrapper for the <see cref="DoubleClickCommandParameterProperty"/>
        /// dependency property:<br/>
        /// Command parameter for the <see cref="DoubleClickCommand"/>.
        /// </summary>
        [Category(CategoryName)]
        [Description("Parameter to submit to the DoubleClickCommand when the user double clicks on the NotifyIcon.")]
        public object DoubleClickCommandParameter
        {
            get { return GetValue(DoubleClickCommandParameterProperty); }
            set { SetValue(DoubleClickCommandParameterProperty, value); }
        }

        #endregion

        #region DoubleClickCommandTarget dependency property

        /// <summary>
        /// The target of the command that is fired if the notify icon is double clicked.
        /// </summary>
        public static readonly DependencyProperty DoubleClickCommandTargetProperty =
            DependencyProperty.Register("DoubleClickCommandTarget",
                typeof(IInputElement),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// A property wrapper for the <see cref="DoubleClickCommandTargetProperty"/>
        /// dependency property:<br/>
        /// The target of the command that is fired if the notify icon is double clicked.
        /// </summary>
        [Category(CategoryName)]
        [Description("The target of the command that is fired if the notify icon is double clicked.")]
        public IInputElement DoubleClickCommandTarget
        {
            get { return (IInputElement)GetValue(DoubleClickCommandTargetProperty); }
            set { SetValue(DoubleClickCommandTargetProperty, value); }
        }

        #endregion

        #region LeftClickCommand dependency property

        /// <summary>
        /// Associates a command that is being executed if the tray icon is being
        /// double clicked.
        /// </summary>
        public static readonly DependencyProperty LeftClickCommandProperty =
            DependencyProperty.Register("LeftClickCommand",
                typeof(ICommand),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// A property wrapper for the <see cref="LeftClickCommandProperty"/>
        /// dependency property:<br/>
        /// Associates a command that is being executed if the tray icon is being
        /// left-clicked.
        /// </summary>
        [Category(CategoryName)]
        [Description("A command that is being executed if the tray icon is being left-clicked.")]
        public ICommand LeftClickCommand
        {
            get { return (ICommand)GetValue(LeftClickCommandProperty); }
            set { SetValue(LeftClickCommandProperty, value); }
        }

        #endregion

        #region LeftClickCommandParameter dependency property

        /// <summary>
        /// Command parameter for the <see cref="LeftClickCommand"/>.
        /// </summary>
        public static readonly DependencyProperty LeftClickCommandParameterProperty =
            DependencyProperty.Register("LeftClickCommandParameter",
                typeof(object),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// A property wrapper for the <see cref="LeftClickCommandParameterProperty"/>
        /// dependency property:<br/>
        /// Command parameter for the <see cref="LeftClickCommand"/>.
        /// </summary>
        [Category(CategoryName)]
        [Description("The target of the command that is fired if the notify icon is clicked with the left mouse button."
            )]
        public object LeftClickCommandParameter
        {
            get { return GetValue(LeftClickCommandParameterProperty); }
            set { SetValue(LeftClickCommandParameterProperty, value); }
        }

        #endregion

        #region LeftClickCommandTarget dependency property

        /// <summary>
        /// The target of the command that is fired if the notify icon is clicked.
        /// </summary>
        public static readonly DependencyProperty LeftClickCommandTargetProperty =
            DependencyProperty.Register("LeftClickCommandTarget",
                typeof(IInputElement),
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// A property wrapper for the <see cref="LeftClickCommandTargetProperty"/>
        /// dependency property:<br/>
        /// The target of the command that is fired if the notify icon is clicked.
        /// </summary>
        [Category(CategoryName)]
        [Description("The target of the command that is fired if the notify icon is clicked with the left mouse button.")]
        public IInputElement LeftClickCommandTarget
        {
            get { return (IInputElement)GetValue(LeftClickCommandTargetProperty); }
            set { SetValue(LeftClickCommandTargetProperty, value); }
        }

        #endregion

        //EVENTS

        #region TrayLeftMouseDown

        /// <summary>
        /// TrayLeftMouseDown Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayLeftMouseDownEvent =
            EventManager.RegisterRoutedEvent("TrayLeftMouseDown",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Occurs when the user presses the left mouse button.
        /// </summary>
        [Category(CategoryName)]
        public event RoutedEventHandler TrayLeftMouseDown
        {
            add { AddHandler(TrayLeftMouseDownEvent, value); }
            remove { RemoveHandler(TrayLeftMouseDownEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the TrayLeftMouseDown event.
        /// </summary>
        protected RoutedEventArgs RaiseTrayLeftMouseDownEvent()
        {
            var args = RaiseTrayLeftMouseDownEvent(this);
            return args;
        }

        /// <summary>
        /// A static helper method to raise the TrayLeftMouseDown event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayLeftMouseDownEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayLeftMouseDownEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayRightMouseDown

        /// <summary>
        /// TrayRightMouseDown Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayRightMouseDownEvent =
            EventManager.RegisterRoutedEvent("TrayRightMouseDown",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Occurs when the presses the right mouse button.
        /// </summary>
        public event RoutedEventHandler TrayRightMouseDown
        {
            add { AddHandler(TrayRightMouseDownEvent, value); }
            remove { RemoveHandler(TrayRightMouseDownEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the TrayRightMouseDown event.
        /// </summary>
        protected RoutedEventArgs RaiseTrayRightMouseDownEvent() => RaiseTrayRightMouseDownEvent(this);

        /// <summary>
        /// A static helper method to raise the TrayRightMouseDown event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayRightMouseDownEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayRightMouseDownEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayMiddleMouseDown

        /// <summary>
        /// TrayMiddleMouseDown Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayMiddleMouseDownEvent =
            EventManager.RegisterRoutedEvent("TrayMiddleMouseDown",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Occurs when the user presses the middle mouse button.
        /// </summary>
        public event RoutedEventHandler TrayMiddleMouseDown
        {
            add { AddHandler(TrayMiddleMouseDownEvent, value); }
            remove { RemoveHandler(TrayMiddleMouseDownEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the TrayMiddleMouseDown event.
        /// </summary>
        protected RoutedEventArgs RaiseTrayMiddleMouseDownEvent() => RaiseTrayMiddleMouseDownEvent(this);

        /// <summary>
        /// A static helper method to raise the TrayMiddleMouseDown event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayMiddleMouseDownEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayMiddleMouseDownEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayLeftMouseUp

        /// <summary>
        /// TrayLeftMouseUp Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayLeftMouseUpEvent =
            EventManager.RegisterRoutedEvent("TrayLeftMouseUp",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Occurs when the user releases the left mouse button.
        /// </summary>
        public event RoutedEventHandler TrayLeftMouseUp
        {
            add { AddHandler(TrayLeftMouseUpEvent, value); }
            remove { RemoveHandler(TrayLeftMouseUpEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the TrayLeftMouseUp event.
        /// </summary>
        protected RoutedEventArgs RaiseTrayLeftMouseUpEvent() => RaiseTrayLeftMouseUpEvent(this);

        /// <summary>
        /// A static helper method to raise the TrayLeftMouseUp event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayLeftMouseUpEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayLeftMouseUpEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayRightMouseUp

        /// <summary>
        /// TrayRightMouseUp Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayRightMouseUpEvent =
            EventManager.RegisterRoutedEvent("TrayRightMouseUp",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Occurs when the user releases the right mouse button.
        /// </summary>
        public event RoutedEventHandler TrayRightMouseUp
        {
            add { AddHandler(TrayRightMouseUpEvent, value); }
            remove { RemoveHandler(TrayRightMouseUpEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the TrayRightMouseUp event.
        /// </summary>
        protected RoutedEventArgs RaiseTrayRightMouseUpEvent() => RaiseTrayRightMouseUpEvent(this);

        /// <summary>
        /// A static helper method to raise the TrayRightMouseUp event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayRightMouseUpEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayRightMouseUpEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayMiddleMouseUp

        /// <summary>
        /// TrayMiddleMouseUp Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayMiddleMouseUpEvent =
            EventManager.RegisterRoutedEvent("TrayMiddleMouseUp",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Occurs when the user releases the middle mouse button.
        /// </summary>
        public event RoutedEventHandler TrayMiddleMouseUp
        {
            add { AddHandler(TrayMiddleMouseUpEvent, value); }
            remove { RemoveHandler(TrayMiddleMouseUpEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the TrayMiddleMouseUp event.
        /// </summary>
        protected RoutedEventArgs RaiseTrayMiddleMouseUpEvent() => RaiseTrayMiddleMouseUpEvent(this);

        /// <summary>
        /// A static helper method to raise the TrayMiddleMouseUp event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayMiddleMouseUpEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayMiddleMouseUpEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayMouseClick

        /// <summary>TrayMouseClick Routed Event</summary>
        public static readonly RoutedEvent TrayMouseClickEvent =
            EventManager.RegisterRoutedEvent("TrayMouseClick",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>Возникает при щелчке мышью по иконке</summary>
        public event RoutedEventHandler TrayMouseClick
        {
            add { AddHandler(TrayMouseClickEvent, value); }
            remove { RemoveHandler(TrayMouseClickEvent, value); }
        }

        /// <summary>Метод генерации события TrayMouseClick</summary>
        protected RoutedEventArgs RaiseTrayMouseClickEvent()
        {
            var args = RaiseTrayMouseClickEvent(this);
            DoubleClickCommand.ExecuteIfEnabled(DoubleClickCommandParameter, DoubleClickCommandTarget ?? this);
            return args;
        }

        /// <summary>
        /// A static helper method to raise the TrayMouseClick event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayMouseClickEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayMouseClickEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayMouseDoubleClick

        /// <summary>TrayMouseDoubleClick Routed Event</summary>
        public static readonly RoutedEvent TrayMouseDoubleClickEvent =
            EventManager.RegisterRoutedEvent("TrayMouseDoubleClick",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>Возникает при двойном щелчке мышью по иконке</summary>
        public event RoutedEventHandler TrayMouseDoubleClick
        {
            add { AddHandler(TrayMouseDoubleClickEvent, value); }
            remove { RemoveHandler(TrayMouseDoubleClickEvent, value); }
        }

        /// <summary>Метод генерации события TrayMouseDoubleClick</summary>
        protected RoutedEventArgs RaiseTrayMouseDoubleClickEvent()
        {
            var args = RaiseTrayMouseDoubleClickEvent(this);
            DoubleClickCommand.ExecuteIfEnabled(DoubleClickCommandParameter, DoubleClickCommandTarget ?? this);
            return args;
        }

        /// <summary>
        /// A static helper method to raise the TrayMouseDoubleClick event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayMouseDoubleClickEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayMouseDoubleClickEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayMouseMove

        /// <summary>
        /// TrayMouseMove Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayMouseMoveEvent = EventManager.RegisterRoutedEvent("TrayMouseMove",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TaskbarIcon));

        /// <summary>
        /// Occurs when the user moves the mouse over the taskbar icon.
        /// </summary>
        public event RoutedEventHandler TrayMouseMove
        {
            add { AddHandler(TrayMouseMoveEvent, value); }
            remove { RemoveHandler(TrayMouseMoveEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the TrayMouseMove event.
        /// </summary>
        protected RoutedEventArgs RaiseTrayMouseMoveEvent() => RaiseTrayMouseMoveEvent(this);

        /// <summary>
        /// A static helper method to raise the TrayMouseMove event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayMouseMoveEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayMouseMoveEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayBalloonTipShown

        /// <summary>
        /// TrayBalloonTipShown Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayBalloonTipShownEvent =
            EventManager.RegisterRoutedEvent("TrayBalloonTipShown",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Occurs when a balloon ToolTip is displayed.
        /// </summary>
        public event RoutedEventHandler TrayBalloonTipShown
        {
            add { AddHandler(TrayBalloonTipShownEvent, value); }
            remove { RemoveHandler(TrayBalloonTipShownEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the TrayBalloonTipShown event.
        /// </summary>
        protected RoutedEventArgs RaiseTrayBalloonTipShownEvent() => RaiseTrayBalloonTipShownEvent(this);

        /// <summary>
        /// A static helper method to raise the TrayBalloonTipShown event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayBalloonTipShownEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayBalloonTipShownEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayBalloonTipClosed

        /// <summary>
        /// TrayBalloonTipClosed Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayBalloonTipClosedEvent =
            EventManager.RegisterRoutedEvent("TrayBalloonTipClosed",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Occurs when a balloon ToolTip was closed.
        /// </summary>
        public event RoutedEventHandler TrayBalloonTipClosed
        {
            add { AddHandler(TrayBalloonTipClosedEvent, value); }
            remove { RemoveHandler(TrayBalloonTipClosedEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the TrayBalloonTipClosed event.
        /// </summary>
        protected RoutedEventArgs RaiseTrayBalloonTipClosedEvent() => RaiseTrayBalloonTipClosedEvent(this);

        /// <summary>
        /// A static helper method to raise the TrayBalloonTipClosed event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayBalloonTipClosedEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayBalloonTipClosedEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayBalloonTipClicked

        /// <summary>
        /// TrayBalloonTipClicked Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayBalloonTipClickedEvent =
            EventManager.RegisterRoutedEvent("TrayBalloonTipClicked",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Occurs when the user clicks on a balloon ToolTip.
        /// </summary>
        public event RoutedEventHandler TrayBalloonTipClicked
        {
            add { AddHandler(TrayBalloonTipClickedEvent, value); }
            remove { RemoveHandler(TrayBalloonTipClickedEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the TrayBalloonTipClicked event.
        /// </summary>
        protected RoutedEventArgs RaiseTrayBalloonTipClickedEvent() => RaiseTrayBalloonTipClickedEvent(this);

        /// <summary>
        /// A static helper method to raise the TrayBalloonTipClicked event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayBalloonTipClickedEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayBalloonTipClickedEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayContextMenuOpen (and PreviewTrayContextMenuOpen)

        /// <summary>
        /// TrayContextMenuOpen Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayContextMenuOpenEvent =
            EventManager.RegisterRoutedEvent("TrayContextMenuOpen",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Bubbled event that occurs when the context menu of the taskbar icon is being displayed.
        /// </summary>
        public event RoutedEventHandler TrayContextMenuOpen
        {
            add { AddHandler(TrayContextMenuOpenEvent, value); }
            remove { RemoveHandler(TrayContextMenuOpenEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the TrayContextMenuOpen event.
        /// </summary>
        protected RoutedEventArgs RaiseTrayContextMenuOpenEvent() => RaiseTrayContextMenuOpenEvent(this);

        /// <summary>
        /// A static helper method to raise the TrayContextMenuOpen event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayContextMenuOpenEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayContextMenuOpenEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        /// <summary>
        /// PreviewTrayContextMenuOpen Routed Event
        /// </summary>
        public static readonly RoutedEvent PreviewTrayContextMenuOpenEvent =
            EventManager.RegisterRoutedEvent("PreviewTrayContextMenuOpen",
                RoutingStrategy.Tunnel,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Tunneled event that occurs when the context menu of the taskbar icon is being displayed.
        /// </summary>
        public event RoutedEventHandler PreviewTrayContextMenuOpen
        {
            add { AddHandler(PreviewTrayContextMenuOpenEvent, value); }
            remove { RemoveHandler(PreviewTrayContextMenuOpenEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the PreviewTrayContextMenuOpen event.
        /// </summary>
        protected RoutedEventArgs RaisePreviewTrayContextMenuOpenEvent() => RaisePreviewTrayContextMenuOpenEvent(this);

        /// <summary>
        /// A static helper method to raise the PreviewTrayContextMenuOpen event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaisePreviewTrayContextMenuOpenEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = PreviewTrayContextMenuOpenEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayPopupOpen (and PreviewTrayPopupOpen)

        /// <summary>
        /// TrayPopupOpen Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayPopupOpenEvent =
            EventManager.RegisterRoutedEvent("TrayPopupOpen",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Bubbled event that occurs when the custom popup is being opened.
        /// </summary>
        public event RoutedEventHandler TrayPopupOpen
        {
            add { AddHandler(TrayPopupOpenEvent, value); }
            remove { RemoveHandler(TrayPopupOpenEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the TrayPopupOpen event.
        /// </summary>
        protected RoutedEventArgs RaiseTrayPopupOpenEvent() => RaiseTrayPopupOpenEvent(this);

        /// <summary>
        /// A static helper method to raise the TrayPopupOpen event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayPopupOpenEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayPopupOpenEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        /// <summary>
        /// PreviewTrayPopupOpen Routed Event
        /// </summary>
        public static readonly RoutedEvent PreviewTrayPopupOpenEvent =
            EventManager.RegisterRoutedEvent("PreviewTrayPopupOpen",
                RoutingStrategy.Tunnel,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Tunneled event that occurs when the custom popup is being opened.
        /// </summary>
        public event RoutedEventHandler PreviewTrayPopupOpen
        {
            add { AddHandler(PreviewTrayPopupOpenEvent, value); }
            remove { RemoveHandler(PreviewTrayPopupOpenEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the PreviewTrayPopupOpen event.
        /// </summary>
        protected RoutedEventArgs RaisePreviewTrayPopupOpenEvent() => RaisePreviewTrayPopupOpenEvent(this);

        /// <summary>
        /// A static helper method to raise the PreviewTrayPopupOpen event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaisePreviewTrayPopupOpenEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = PreviewTrayPopupOpenEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayToolTipOpen (and PreviewTrayToolTipOpen)

        /// <summary>
        /// TrayToolTipOpen Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayToolTipOpenEvent =
            EventManager.RegisterRoutedEvent("TrayToolTipOpen",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Bubbled event that occurs when the custom ToolTip is being displayed.
        /// </summary>
        public event RoutedEventHandler TrayToolTipOpen
        {
            add { AddHandler(TrayToolTipOpenEvent, value); }
            remove { RemoveHandler(TrayToolTipOpenEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the TrayToolTipOpen event.
        /// </summary>
        protected RoutedEventArgs RaiseTrayToolTipOpenEvent() => RaiseTrayToolTipOpenEvent(this);

        /// <summary>
        /// A static helper method to raise the TrayToolTipOpen event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayToolTipOpenEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayToolTipOpenEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        /// <summary>
        /// PreviewTrayToolTipOpen Routed Event
        /// </summary>
        public static readonly RoutedEvent PreviewTrayToolTipOpenEvent =
            EventManager.RegisterRoutedEvent("PreviewTrayToolTipOpen",
                RoutingStrategy.Tunnel,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Tunneled event that occurs when the custom ToolTip is being displayed.
        /// </summary>
        public event RoutedEventHandler PreviewTrayToolTipOpen
        {
            add { AddHandler(PreviewTrayToolTipOpenEvent, value); }
            remove { RemoveHandler(PreviewTrayToolTipOpenEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the PreviewTrayToolTipOpen event.
        /// </summary>
        protected RoutedEventArgs RaisePreviewTrayToolTipOpenEvent() => RaisePreviewTrayToolTipOpenEvent(this);

        /// <summary>
        /// A static helper method to raise the PreviewTrayToolTipOpen event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaisePreviewTrayToolTipOpenEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = PreviewTrayToolTipOpenEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region TrayToolTipClose (and PreviewTrayToolTipClose)

        /// <summary>
        /// TrayToolTipClose Routed Event
        /// </summary>
        public static readonly RoutedEvent TrayToolTipCloseEvent =
            EventManager.RegisterRoutedEvent("TrayToolTipClose",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Bubbled event that occurs when a custom tooltip is being closed.
        /// </summary>
        public event RoutedEventHandler TrayToolTipClose
        {
            add { AddHandler(TrayToolTipCloseEvent, value); }
            remove { RemoveHandler(TrayToolTipCloseEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the TrayToolTipClose event.
        /// </summary>
        protected RoutedEventArgs RaiseTrayToolTipCloseEvent() => RaiseTrayToolTipCloseEvent(this);

        /// <summary>
        /// A static helper method to raise the TrayToolTipClose event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseTrayToolTipCloseEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = TrayToolTipCloseEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        /// <summary>
        /// PreviewTrayToolTipClose Routed Event
        /// </summary>
        public static readonly RoutedEvent PreviewTrayToolTipCloseEvent =
            EventManager.RegisterRoutedEvent("PreviewTrayToolTipClose",
                RoutingStrategy.Tunnel,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Tunneled event that occurs when a custom tooltip is being closed.
        /// </summary>
        public event RoutedEventHandler PreviewTrayToolTipClose
        {
            add { AddHandler(PreviewTrayToolTipCloseEvent, value); }
            remove { RemoveHandler(PreviewTrayToolTipCloseEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the PreviewTrayToolTipClose event.
        /// </summary>
        protected RoutedEventArgs RaisePreviewTrayToolTipCloseEvent() => RaisePreviewTrayToolTipCloseEvent(this);

        /// <summary>
        /// A static helper method to raise the PreviewTrayToolTipClose event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaisePreviewTrayToolTipCloseEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = PreviewTrayToolTipCloseEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        //ATTACHED EVENTS

        #region PopupOpened

        /// <summary>
        /// PopupOpened Attached Routed Event
        /// </summary>
        public static readonly RoutedEvent PopupOpenedEvent =
            EventManager.RegisterRoutedEvent("PopupOpened",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Adds a handler for the PopupOpened attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be added</param>
        public static void AddPopupOpenedHandler(DependencyObject element, RoutedEventHandler handler) => RoutedEventHelper.AddHandler(element, PopupOpenedEvent, handler);

        /// <summary>
        /// Removes a handler for the PopupOpened attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be removed</param>
        public static void RemovePopupOpenedHandler(DependencyObject element, RoutedEventHandler handler) => RoutedEventHelper.RemoveHandler(element, PopupOpenedEvent, handler);

        /// <summary>
        /// A static helper method to raise the PopupOpened event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaisePopupOpenedEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = PopupOpenedEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region ToolTipOpened

        /// <summary>
        /// ToolTipOpened Attached Routed Event
        /// </summary>
        public static readonly RoutedEvent ToolTipOpenedEvent =
            EventManager.RegisterRoutedEvent("ToolTipOpened",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Adds a handler for the ToolTipOpened attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be added</param>
        public static void AddToolTipOpenedHandler(DependencyObject element, RoutedEventHandler handler) => RoutedEventHelper.AddHandler(element, ToolTipOpenedEvent, handler);

        /// <summary>
        /// Removes a handler for the ToolTipOpened attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be removed</param>
        public static void RemoveToolTipOpenedHandler(DependencyObject element, RoutedEventHandler handler) => RoutedEventHelper.RemoveHandler(element, ToolTipOpenedEvent, handler);

        /// <summary>
        /// A static helper method to raise the ToolTipOpened event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseToolTipOpenedEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = ToolTipOpenedEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region ToolTipClose

        /// <summary>
        /// ToolTipClose Attached Routed Event
        /// </summary>
        public static readonly RoutedEvent ToolTipCloseEvent =
            EventManager.RegisterRoutedEvent("ToolTipClose",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Adds a handler for the ToolTipClose attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be added</param>
        public static void AddToolTipCloseHandler(DependencyObject element, RoutedEventHandler handler) => RoutedEventHelper.AddHandler(element, ToolTipCloseEvent, handler);

        /// <summary>
        /// Removes a handler for the ToolTipClose attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be removed</param>
        public static void RemoveToolTipCloseHandler(DependencyObject element, RoutedEventHandler handler) => RoutedEventHelper.RemoveHandler(element, ToolTipCloseEvent, handler);

        /// <summary>
        /// A static helper method to raise the ToolTipClose event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        private static RoutedEventArgs RaiseToolTipCloseEvent(DependencyObject target)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs { RoutedEvent = ToolTipCloseEvent };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region BalloonShowing

        /// <summary>
        /// BalloonShowing Attached Routed Event
        /// </summary>
        public static readonly RoutedEvent BalloonShowingEvent =
            EventManager.RegisterRoutedEvent("BalloonShowing",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Adds a handler for the BalloonShowing attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be added</param>
        public static void AddBalloonShowingHandler(DependencyObject element, RoutedEventHandler handler) => RoutedEventHelper.AddHandler(element, BalloonShowingEvent, handler);

        /// <summary>
        /// Removes a handler for the BalloonShowing attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be removed</param>
        public static void RemoveBalloonShowingHandler(DependencyObject element, RoutedEventHandler handler) => RoutedEventHelper.RemoveHandler(element, BalloonShowingEvent, handler);

        /// <summary>
        /// A static helper method to raise the BalloonShowing event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        /// <param name="source">The <see cref="TaskbarIcon"/> instance that manages the balloon.</param>
        private static RoutedEventArgs RaiseBalloonShowingEvent(DependencyObject target, TaskbarIcon source)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs(BalloonShowingEvent, source);
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #region BalloonClosing

        /// <summary>
        /// BalloonClosing Attached Routed Event
        /// </summary>
        public static readonly RoutedEvent BalloonClosingEvent =
            EventManager.RegisterRoutedEvent("BalloonClosing",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TaskbarIcon));

        /// <summary>
        /// Adds a handler for the BalloonClosing attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be added</param>
        public static void AddBalloonClosingHandler(DependencyObject element, RoutedEventHandler handler) => RoutedEventHelper.AddHandler(element, BalloonClosingEvent, handler);

        /// <summary>
        /// Removes a handler for the BalloonClosing attached event
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="handler">Event handler to be removed</param>
        public static void RemoveBalloonClosingHandler(DependencyObject element, RoutedEventHandler handler) => RoutedEventHelper.RemoveHandler(element, BalloonClosingEvent, handler);

        /// <summary>
        /// A static helper method to raise the BalloonClosing event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        /// <param name="source">The <see cref="TaskbarIcon"/> instance that manages the balloon.</param>
        private static RoutedEventArgs RaiseBalloonClosingEvent(DependencyObject target, TaskbarIcon source)
        {
            if(target is null) return null;

            var args = new RoutedEventArgs(BalloonClosingEvent, source);
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        //ATTACHED PROPERTIES

        #region ParentTaskbarIcon

        /// <summary>
        /// An attached property that is assigned to displayed UI elements (balloos, tooltips, context menus), and
        /// that can be used to bind to this control. The attached property is being derived, so binding is
        /// quite straightforward:
        /// <code>
        /// <TextBlock Text="{Binding RelativeSource={RelativeSource Self}, Path=(tb:TaskbarIcon.ParentTaskbarIcon).ToolTipText}" />
        /// </code>
        /// </summary>  
        public static readonly DependencyProperty ParentTaskbarIconProperty =
            DependencyProperty.RegisterAttached(
                "ParentTaskbarIcon", 
                typeof(TaskbarIcon), 
                typeof(TaskbarIcon),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the ParentTaskbarIcon property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static TaskbarIcon GetParentTaskbarIcon(DependencyObject d) => (TaskbarIcon)d.GetValue(ParentTaskbarIconProperty);

        /// <summary>
        /// Sets the ParentTaskbarIcon property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetParentTaskbarIcon(DependencyObject d, TaskbarIcon value) => d.SetValue(ParentTaskbarIconProperty, value);

        #endregion

        //BASE CLASS PROPERTY OVERRIDES

        /// <summary>
        /// Registers properties.
        /// </summary>
        static TaskbarIcon()
        {
            //register change listener for the Visibility property
            var md = new PropertyMetadata(Visibility.Visible, VisibilityPropertyChanged);
            VisibilityProperty.OverrideMetadata(typeof(TaskbarIcon), md);

            //register change listener for the DataContext property
            md = new FrameworkPropertyMetadata(DataContextPropertyChanged);
            DataContextProperty.OverrideMetadata(typeof(TaskbarIcon), md);

            //register change listener for the ContextMenu property
            md = new FrameworkPropertyMetadata(ContextMenuPropertyChanged);
            ContextMenuProperty.OverrideMetadata(typeof(TaskbarIcon), md);
        }
    }

    /// <summary>
    /// Util and extension methods.
    /// </summary>
    internal static class TaskBarIconUtilities
    {
        private static readonly object SyncRoot = new object();

        #region IsDesignMode

        private static readonly bool isDesignMode;

        /// <summary>
        /// Checks whether the application is currently in design mode.
        /// </summary>
        public static bool IsDesignMode => isDesignMode;

        #endregion

        #region construction

        static TaskBarIconUtilities()
        {
            isDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(
                DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement))
                .Metadata.DefaultValue;
        }
        #endregion

        #region WriteIconData

        /// <summary>
        /// Updates the taskbar icons with data provided by a given
        /// <see cref="NotifyIconData"/> instance.
        /// </summary>
        /// <param name="data">Configuration settings for the NotifyIcon.</param>
        /// <param name="command">Operation on the icon (e.g. delete the icon).</param>
        /// <returns>True if the data was successfully written.</returns>
        /// <remarks>See Shell_NotifyIcon documentation on MSDN for details.</remarks>
        public static bool WriteIconData(ref NotifyIconData data, NotifyCommand command) => WriteIconData(ref data, command, data.ValidMembers);


        /// <summary>
        /// Updates the taskbar icons with data provided by a given
        /// <see cref="NotifyIconData"/> instance.
        /// </summary>
        /// <param name="data">Configuration settings for the NotifyIcon.</param>
        /// <param name="command">Operation on the icon (e.g. delete the icon).</param>
        /// <param name="flags">Defines which members of the <paramref name="data"/>
        /// structure are set.</param>
        /// <returns>True if the data was successfully written.</returns>
        /// <remarks>See Shell_NotifyIcon documentation on MSDN for details.</remarks>
        public static bool WriteIconData(ref NotifyIconData data, NotifyCommand command, IconDataMembers flags)
        {
            //do nothing if in design mode
            if(IsDesignMode) return true;

            data.ValidMembers = flags;
            lock(SyncRoot)
                return WinApi.Shell_NotifyIcon(command, ref data);
        }

        #endregion

        #region GetBalloonFlag

        /// <summary>
        /// Gets a <see cref="BalloonFlags"/> enum value that
        /// matches a given <see cref="BalloonIcon"/>.
        /// </summary>
        public static BalloonFlags GetBalloonFlag(this BalloonIcon icon)
        {
            switch(icon)
            {
                case BalloonIcon.None:
                    return BalloonFlags.None;
                case BalloonIcon.Info:
                    return BalloonFlags.Info;
                case BalloonIcon.Warning:
                    return BalloonFlags.Warning;
                case BalloonIcon.Error:
                    return BalloonFlags.Error;
                default:
                    throw new ArgumentOutOfRangeException(nameof(icon));
            }
        }

        #endregion

        #region ImageSource to Icon

        /// <summary>
        /// Reads a given image resource into a WinForms icon.
        /// </summary>
        /// <param name="imageSource">Image source pointing to
        /// an icon file (*.ico).</param>
        /// <returns>An icon object that can be used with the
        /// taskbar area.</returns>
        public static Icon ToIcon(this ImageSource imageSource)
        {
            if(imageSource is null) return null;

            var uri = new Uri(imageSource.ToString());
            var streamInfo = Application.GetResourceStream(uri);

            if(streamInfo != null) return new Icon(streamInfo.Stream);
            var msg = "The supplied image source '{0}' could not be resolved.";
            msg = string.Format(msg, imageSource);
            throw new ArgumentException(msg);
        }

        #endregion

        #region evaluate listings

        /// <summary>
        /// Checks a list of candidates for equality to a given
        /// reference value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The evaluated value.</param>
        /// <param name="candidates">A liste of possible values that are
        /// regarded valid.</param>
        /// <returns>True if one of the submitted <paramref name="candidates"/>
        /// matches the evaluated value. If the <paramref name="candidates"/>
        /// parameter itself is null, too, the method returns false as well,
        /// which allows to check with null values, too.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="candidates"/>
        /// is a null reference.</exception>
        public static bool Is<T>(this T value, params T[] candidates) => candidates != null && candidates.Contains(value);

        #endregion

        #region match MouseEvent to PopupActivation

        /// <summary>
        /// Checks if a given <see cref="PopupActivationMode"/> is a match for
        /// an effectively pressed mouse button.
        /// </summary>
        public static bool IsMatch(this MouseEvent me, PopupActivationMode activationMode)
        {
            switch(activationMode)
            {
                case PopupActivationMode.LeftClick:
                    return me == MouseEvent.IconLeftMouseUp;
                case PopupActivationMode.RightClick:
                    return me == MouseEvent.IconRightMouseUp;
                case PopupActivationMode.LeftOrRightClick:
                    return me.Is(MouseEvent.IconLeftMouseUp, MouseEvent.IconRightMouseUp);
                case PopupActivationMode.LeftOrDoubleClick:
                    return me.Is(MouseEvent.IconLeftMouseUp, MouseEvent.IconDoubleClick);
                case PopupActivationMode.DoubleClick:
                    return me.Is(MouseEvent.IconDoubleClick);
                case PopupActivationMode.MiddleClick:
                    return me == MouseEvent.IconMiddleMouseUp;
                case PopupActivationMode.All:
                    //return true for everything except mouse movements
                    return me != MouseEvent.MouseMove;
                default:
                    throw new ArgumentOutOfRangeException(nameof(activationMode));
            }
        }

        #endregion

        #region execute command

        /// <summary>
        /// Executes a given command if its <see cref="ICommand.CanExecute"/> method
        /// indicates it can run.
        /// </summary>
        /// <param name="command">The command to be executed, or a null reference.</param>
        /// <param name="commandParameter">An optional parameter that is associated with
        /// the command.</param>
        /// <param name="target">The target element on which to raise the command.</param>
        public static void ExecuteIfEnabled(this ICommand command, object commandParameter, IInputElement target)
        {
            if(command is null) return;

            var rc = command as RoutedCommand;
            if(rc != null) //routed commands work on a target
            {
                if(rc.CanExecute(commandParameter, target)) rc.Execute(commandParameter, target);
            }
            else if(command.CanExecute(commandParameter))
                command.Execute(commandParameter);
        }

        #endregion

        /// <summary>
        /// Returns a dispatcher for multi-threaded scenarios
        /// </summary>
        /// <returns></returns>
        public static Dispatcher GetDispatcher(this DispatcherObject source)
        {
            //use the application's dispatcher by default
            //fallback for WinForms environments
            //ultimatively use the thread's dispatcher
            return Application.Current != null
                        ? Application.Current.Dispatcher
                        : (source.Dispatcher ?? Dispatcher.CurrentDispatcher);


        }


        /// <summary>
        /// Checks whether the <see cref="FrameworkElement.DataContextProperty"/>
        ///  is bound or not.
        /// </summary>
        /// <param name="element">The element to be checked.</param>
        /// <returns>True if the data context property is being managed by a
        /// binding expression.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="element"/>
        /// is a null reference.</exception>
        public static bool IsDataContextDataBound(this FrameworkElement element)
        {
            if(element is null) throw new ArgumentNullException(nameof(element));
            return element.GetBindingExpression(FrameworkElement.DataContextProperty) != null;
        }
    }

    /// <summary>
    /// Helper class used by routed events of the
    /// <see cref="TaskbarIcon"/> class.
    /// </summary>
    public static class RoutedEventHelper
    {
        #region RoutedEvent Helper Methods

        /// <summary>
        /// A static helper method to raise a routed event on a target UIElement or ContentElement.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        /// <param name="args">RoutedEventArgs to use when raising the event</param>
        public static void RaiseEvent(DependencyObject target, RoutedEventArgs args)
        {
            var ui = target as UIElement;
            if (ui != null)
                ui.RaiseEvent(args);
            else (target as ContentElement)?.RaiseEvent(args);
        }

        /// <summary>
        /// A static helper method that adds a handler for a routed event 
        /// to a target UIElement or ContentElement.
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="routedEvent">Event that will be handled</param>
        /// <param name="handler">Event handler to be added</param>
        public static void AddHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
        {
            var uie = element as UIElement;
            if (uie != null)
                uie.AddHandler(routedEvent, handler);
            else
                (element as ContentElement)?.AddHandler(routedEvent, handler);
        }

        /// <summary>
        /// A static helper method that removes a handler for a routed event 
        /// from a target UIElement or ContentElement.
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="routedEvent">Event that will no longer be handled</param>
        /// <param name="handler">Event handler to be removed</param>
        public static void RemoveHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
        {
            var uie = element as UIElement;
            if (uie != null)
                uie.RemoveHandler(routedEvent, handler);
            else
                (element as ContentElement)?.RemoveHandler(routedEvent, handler);
        }

        #endregion
    }

    /// <summary>
    /// Defines flags that define when a popup
    /// is being displyed.
    /// </summary>
    public enum PopupActivationMode
    {
        /// <summary>
        /// The item is displayed if the user clicks the
        /// tray icon with the left mouse button.
        /// </summary>
        LeftClick,

        /// <summary>
        /// The item is displayed if the user clicks the
        /// tray icon with the right mouse button.
        /// </summary>
        RightClick,

        /// <summary>
        /// The item is displayed if the user double-clicks the
        /// tray icon.
        /// </summary>
        DoubleClick,

        /// <summary>
        /// The item is displayed if the user clicks the
        /// tray icon with the left or the right mouse button.
        /// </summary>
        LeftOrRightClick,

        /// <summary>
        /// The item is displayed if the user clicks the
        /// tray icon with the left mouse button or if a
        /// double-click is being performed.
        /// </summary>
        LeftOrDoubleClick,

        /// <summary>
        /// The item is displayed if the user clicks the
        /// tray icon with the middle mouse button.
        /// </summary>
        MiddleClick,

        /// <summary>
        /// The item is displayed whenever a click occurs.
        /// </summary>
        All
    }

    ///<summary>
    /// Supported icons for the tray's balloon messages.
    ///</summary>
    public enum BalloonIcon
    {
        /// <summary>
        /// The balloon message is displayed without an icon.
        /// </summary>
        None,

        /// <summary>
        /// An information is displayed.
        /// </summary>
        Info,

        /// <summary>
        /// A warning is displayed.
        /// </summary>
        Warning,

        /// <summary>
        /// An error is displayed.
        /// </summary>
        Error
    }

    /// <summary>
    /// Main operations performed on the
    /// <see cref="WinApi.Shell_NotifyIcon"/> function.
    /// </summary>
    public enum NotifyCommand
    {
        /// <summary>
        /// The taskbar icon is being created.
        /// </summary>
        Add = 0x00,

        /// <summary>
        /// The settings of the taskbar icon are being updated.
        /// </summary>
        Modify = 0x01,

        /// <summary>
        /// The taskbar icon is deleted.
        /// </summary>
        Delete = 0x02,

        /// <summary>
        /// Focus is returned to the taskbar icon. Currently not in use.
        /// </summary>
        SetFocus = 0x03,

        /// <summary>
        /// Shell32.dll version 5.0 and later only. Instructs the taskbar
        /// to behave according to the version number specified in the 
        /// uVersion member of the structure pointed to by lpdata.
        /// This message allows you to specify whether you want the version
        /// 5.0 behavior found on Microsoft Windows 2000 systems, or the
        /// behavior found on earlier Shell versions. The default value for
        /// uVersion is zero, indicating that the original Windows 95 notify
        /// icon behavior should be used.
        /// </summary>
        SetVersion = 0x04
    }

    /// <summary>
    /// Resolves the current tray position.
    /// </summary>
    public static class TrayInfo
    {
        /// <summary>
        /// Gets the position of the system tray.
        /// </summary>
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
}
