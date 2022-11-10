using System.Windows;

namespace MathCore.WPF.TrayIcon;

/// <summary>
/// Helper class used by routed events of the
/// <see cref="TaskbarIcon"/> class.
/// </summary>
public static class RoutedEventHelper
{
    #region RoutedEvent Helper Methods

    /// <summary>A static helper method to raise a routed event on a target UIElement or ContentElement.</summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    /// <param name="args">RoutedEventArgs to use when raising the event</param>
    public static void RaiseEvent(DependencyObject target, RoutedEventArgs args)
    {
        if (target is UIElement ui)
            ui.RaiseEvent(args);
        else 
            (target as ContentElement)?.RaiseEvent(args);
    }

    /// <summary>
    /// A static helper method that adds a handler for a routed event 
    /// to a target UIElement or ContentElement.
    /// </summary>
    /// <param name="element">UIElement or ContentElement that listens to the event</param>
    /// <param name="RoutedEvent">Event that will be handled</param>
    /// <param name="handler">Event handler to be added</param>
    public static void AddHandler(DependencyObject element, RoutedEvent RoutedEvent, Delegate handler)
    {
        if (element is UIElement ui)
            ui.AddHandler(RoutedEvent, handler);
        else
            (element as ContentElement)?.AddHandler(RoutedEvent, handler);
    }

    /// <summary>
    /// A static helper method that removes a handler for a routed event 
    /// from a target UIElement or ContentElement.
    /// </summary>
    /// <param name="element">UIElement or ContentElement that listens to the event</param>
    /// <param name="RoutedEvent">Event that will no longer be handled</param>
    /// <param name="handler">Event handler to be removed</param>
    public static void RemoveHandler(DependencyObject element, RoutedEvent RoutedEvent, Delegate handler)
    {
        if (element is UIElement ui)
            ui.RemoveHandler(RoutedEvent, handler);
        else
            (element as ContentElement)?.RemoveHandler(RoutedEvent, handler);
    }

    #endregion
}