using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MathCore.Annotations;

namespace MathCore.WPF.TrayIcon
{
    /// <summary>
    /// Util and extension methods.
    /// </summary>
    internal static class TaskBarIconUtilities
    {
        private static readonly object __SyncRoot = new object();

        #region IsDesignMode

        /// <summary>
        /// Checks whether the application is currently in design mode.
        /// </summary>
        public static bool IsDesignMode { get; } = 
            (bool)DependencyPropertyDescriptor
               .FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement))
               .Metadata
               .DefaultValue;

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
            lock(__SyncRoot)
                return WinApi.Shell_NotifyIcon(command, ref data);
        }

        #endregion

        #region GetBalloonFlag

        /// <summary>
        /// Gets a <see cref="BalloonFlags"/> enum value that
        /// matches a given <see cref="BalloonIcon"/>.
        /// </summary>
        public static BalloonFlags GetBalloonFlag(this BalloonIcon icon) =>
            icon switch
            {
                BalloonIcon.None => BalloonFlags.None,
                BalloonIcon.Info => BalloonFlags.Info,
                BalloonIcon.Warning => BalloonFlags.Warning,
                BalloonIcon.Error => BalloonFlags.Error,
                _ => throw new ArgumentOutOfRangeException(nameof(icon))
            };

        #endregion

        #region ImageSource to Icon

        /// <summary>
        /// Reads a given image resource into a WinForms icon.
        /// </summary>
        /// <param name="imageSource">Image source pointing to
        /// an icon file (*.ico).</param>
        /// <returns>An icon object that can be used with the
        /// taskbar area.</returns>
        public static Icon? ToIcon([CanBeNull] this ImageSource imageSource)
        {
            if(imageSource is null) return null;

            var uri = new Uri(imageSource.ToString());
            var stream_info = Application.GetResourceStream(uri);

            if(stream_info != null) return new Icon(stream_info.Stream);
            throw new ArgumentException($"The supplied image source '{imageSource}' could not be resolved.");
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
        public static bool Is<T>(this T value, [CanBeNull] params T[] candidates) => candidates != null && candidates.Contains(value);

        #endregion

        #region match MouseEvent to PopupActivation

        /// <summary>
        /// Checks if a given <see cref="PopupActivationMode"/> is a match for
        /// an effectively pressed mouse button.
        /// </summary>
        public static bool IsMatch(this MouseEvent me, PopupActivationMode activationMode) =>
            activationMode switch
            {
                PopupActivationMode.LeftClick => (me == MouseEvent.IconLeftMouseUp),
                PopupActivationMode.RightClick => (me == MouseEvent.IconRightMouseUp),
                PopupActivationMode.LeftOrRightClick => me.Is(MouseEvent.IconLeftMouseUp, MouseEvent.IconRightMouseUp),
                PopupActivationMode.LeftOrDoubleClick => me.Is(MouseEvent.IconLeftMouseUp, MouseEvent.IconDoubleClick),
                PopupActivationMode.DoubleClick => me.Is(MouseEvent.IconDoubleClick),
                PopupActivationMode.MiddleClick => (me == MouseEvent.IconMiddleMouseUp),
                //return true for everything except mouse movements
                PopupActivationMode.All => (me != MouseEvent.MouseMove),
                _ => throw new ArgumentOutOfRangeException(nameof(activationMode))
            };

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
        public static void ExecuteIfEnabled([CanBeNull] this ICommand command, object commandParameter, IInputElement target)
        {
            switch (command)
            {
                case null: return;
                case RoutedCommand rc:
                {
                    if(rc.CanExecute(commandParameter, target)) rc.Execute(commandParameter, target);
                    break;
                }
                default:
                {
                    if(command.CanExecute(commandParameter))
                        command.Execute(commandParameter);
                    break;
                }
            }
        }

        #endregion

        /// <summary>
        /// Returns a dispatcher for multi-threaded scenarios
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public static Dispatcher GetDispatcher(this DispatcherObject source) =>
            //use the application's dispatcher by default
            //fallback for WinForms environments
            //ultimately use the thread's dispatcher
            Application.Current != null
                ? Application.Current.Dispatcher
                : (source.Dispatcher ?? Dispatcher.CurrentDispatcher);


        /// <summary>
        /// Checks whether the <see cref="FrameworkElement.DataContextProperty"/>
        ///  is bound or not.
        /// </summary>
        /// <param name="element">The element to be checked.</param>
        /// <returns>True if the data context property is being managed by a
        /// binding expression.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="element"/>
        /// is a null reference.</exception>
        public static bool IsDataContextDataBound([NotNull] this FrameworkElement element) => 
            (element ?? throw new ArgumentNullException(nameof(element))).GetBindingExpression(FrameworkElement.DataContextProperty) != null;
    }
}