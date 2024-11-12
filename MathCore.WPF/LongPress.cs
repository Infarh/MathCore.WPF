using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using MathCore.WPF.Extensions;

namespace MathCore.WPF;

public static class LongPress
{
    private static readonly DependencyProperty __AttachedControls = DependencyProperty.RegisterAttached(nameof(__AttachedControls), typeof(DependencyObject), typeof(LongPress));

    #region Routed event - Invoked : EventHandler<ModelInvokedEventArgs> - Событие длительного нажатия

    /// <summary>Событие длительного нажатия</summary>
    public static readonly RoutedEvent ClickEvent =
        EventManager.RegisterRoutedEvent(
            "Click",
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(LongPress));

    /// <summary>Событие длительного нажатия</summary>
    public static void AddClickHandler(DependencyObject element, EventHandler handler) => (element as UIElement)?.AddHandler(ClickEvent, handler);

    /// <summary>Событие длительного нажатия</summary>
    public static void RemoveClickHandler(DependencyObject element, EventHandler handler) => (element as UIElement)?.RemoveHandler(ClickEvent, handler);

    #endregion

    #region Attached property LongPress.Command : ICommand - Команда долгого нажатия

    /// <Summary>Команда долгого нажатия</Summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(LongPress),
            new(default(ICommand), OnCommandPropertyChanged));

    private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d.GetValue(__AttachedControls) is { } attached_d)
        {
            if (ReferenceEquals(d, attached_d)) return;
            UnregisterHandlers((Control)attached_d);
        }

        if(e.NewValue is null)
        {
            UnregisterHandlers((Control)d);
            d.SetValue(__AttachedControls, null);
        }

        d.SetValue(__AttachedControls, d);

        RegisterHandlers((Control)d);
    }

    private static void RegisterHandlers(Control control)
    {
        control.MouseLeftButtonDown += OnMouseDown;
        control.MouseLeftButtonUp += OnMouseUp;
    }

    private static void UnregisterHandlers(Control control)
    {
        control.MouseLeftButtonDown -= OnMouseDown;
        control.MouseLeftButtonUp -= OnMouseUp;
    }

    private static DependencyProperty __LastClickTime = DependencyProperty.RegisterAttached(nameof(__LastClickTime), typeof(DateTime), typeof(LongPress));

    private static async void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Control control) return;

        var down_time = DateTime.Now;
        control.SetValue(__LastClickTime, down_time);
        var timeout = Math.Max(100, GetTimeout(control));

        await Task.Delay(timeout);

        if (control.GetValue(CommandProperty) is not ICommand command) return;
        if(!Equals(down_time, control.GetValue(__LastClickTime))) return;

        var parameter = control.GetValue(CommandParameterProperty);
        command.TryExecute(parameter);

        control.RaiseEvent(new(ClickEvent, control));
    }

    private static void OnMouseUp(object sender, MouseButtonEventArgs e) => ((DependencyObject)sender).ClearValue(__LastClickTime);

    /// <Summary>Команда долгого нажатия</Summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetCommand(DependencyObject D, ICommand value) => D.SetValue(CommandProperty, value);

    /// <Summary>Команда долгого нажатия</Summary>
    public static ICommand GetCommand(DependencyObject D) => (ICommand)D.GetValue(CommandProperty);

    #endregion

    #region Attached property LongPress.CommandParameter : object - Параметр команды

    /// <Summary>Параметр команды</Summary>
    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(LongPress));

    /// <Summary>Параметр команды</Summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetCommandParameter(DependencyObject D, object value) => D.SetValue(CommandParameterProperty, value);

    /// <Summary>Параметр команды</Summary>
    public static object GetCommandParameter(DependencyObject D) => (object)D.GetValue(CommandParameterProperty);

    #endregion


    #region Attached property LongPress.Timeout : int - Задержка (не меньше 100) мс

    /// <Summary>Задержка (не меньше 100) мс</Summary>
    public static readonly DependencyProperty TimeoutProperty =
        DependencyProperty.RegisterAttached(
            "Timeout",
            typeof(int),
            typeof(LongPress),
            new(3000, null, (_, v) => Math.Max(100, (int)v)));

    /// <Summary>Задержка (не меньше 100) мс</Summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetTimeout(DependencyObject D, int value) => D.SetValue(TimeoutProperty, value);

    /// <Summary>Задержка (не меньше 100) мс</Summary>
    public static int GetTimeout(DependencyObject D) => (int)D.GetValue(TimeoutProperty);

    #endregion


    #region Attached property LongPress.AnimationTimeout : int - Шаг анимации

    /// <Summary>Шаг анимации</Summary>
    public static readonly DependencyProperty AnimationTimeoutProperty =
        DependencyProperty.RegisterAttached(
            "AnimationTimeout",
            typeof(int),
            typeof(LongPress),
            new(100, null, (_, v) => Math.Max(10, (int)v)));

    /// <Summary>Шаг анимации</Summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetAnimationTimeout(DependencyObject D, int value) => D.SetValue(AnimationTimeoutProperty, value);

    /// <Summary>Шаг анимации</Summary>
    public static int GetAnimationTimeout(DependencyObject D) => (int)D.GetValue(AnimationTimeoutProperty);

    #endregion


    #region Attached property LongPress.PropName : string - Свойство

    /// <Summary>Свойство</Summary>
    private static readonly DependencyPropertyKey __PropNameProperty =
        DependencyProperty.RegisterAttachedReadOnly(
            "PropName",
            typeof(string),
            typeof(LongPress),
            new(default(string), OnPropNamePropertyChanged));

    public static readonly DependencyProperty PropNameProperty = __PropNameProperty.DependencyProperty;

    private static void OnPropNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

    }

    /// <Summary>Свойство</Summary>
    [AttachedPropertyBrowsableForType(typeof(Control))]
    public static void SetPropName(DependencyObject D, string value) => D.SetValue(__PropNameProperty, value);

    /// <Summary>Свойство</Summary>
    public static string GetPropName(DependencyObject D) => (string)D.GetValue(PropNameProperty);

    #endregion
}
