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
            new(null, OnCommandPropertyChanged));

    private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Отписываемся от старых обработчиков если они есть
        if (d.GetValue(__AttachedControls) is Control old_control)
            UnregisterHandlers(old_control);

        // Если новое значение null - очищаем всё и выходим
        if (e.NewValue is null)
        {
            d.ClearValue(__AttachedControls);
            d.ClearValue(__CancellationTokenSource);
            return;
        }

        // Устанавливаем новую связь и регистрируем обработчики
        d.SetValue(__AttachedControls, d);
        RegisterHandlers((Control)d);
    }

    private static void RegisterHandlers(Control control)
    {
        control.MouseLeftButtonDown += OnMouseDown;
        control.MouseLeftButtonUp += OnMouseUp;
        control.MouseLeave += OnMouseLeave; // Добавляем обработку ухода мыши с элемента
    }

    private static void UnregisterHandlers(Control control)
    {
        control.MouseLeftButtonDown -= OnMouseDown;
        control.MouseLeftButtonUp -= OnMouseUp;
        control.MouseLeave -= OnMouseLeave;
        
        // Отменяем текущую задачу если она есть
        if (control.GetValue(__CancellationTokenSource) is CancellationTokenSource cts)
        {
            cts.Cancel();
            cts.Dispose();
            control.ClearValue(__CancellationTokenSource);
        }
    }

    private static readonly DependencyProperty __LastClickTime = DependencyProperty
        .RegisterAttached(
            nameof(__LastClickTime),
            typeof(DateTime),
            typeof(LongPress));

    private static readonly DependencyProperty __CancellationTokenSource = DependencyProperty
        .RegisterAttached(
            nameof(__CancellationTokenSource),
            typeof(CancellationTokenSource),
            typeof(LongPress));

    private static async void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Control control) return;

        // Отменяем предыдущую задачу если она есть
        if (control.GetValue(__CancellationTokenSource) is CancellationTokenSource old_cts)
        {
            old_cts.Cancel();
            old_cts.Dispose();
        }

        var down_time = DateTime.Now;
        control.SetValue(__LastClickTime, down_time);
        
        var cts = new CancellationTokenSource();
        control.SetValue(__CancellationTokenSource, cts);
        
        var timeout = Math.Max(100, GetTimeout(control));

        try
        {
            await Task.Delay(timeout, cts.Token);

            // Проверяем что задача не была отменена и время совпадает
            if (cts.Token.IsCancellationRequested) return;
            if (control.GetValue(CommandProperty) is not ICommand command) return;
            if (!Equals(down_time, control.GetValue(__LastClickTime))) return;

            var parameter = control.GetValue(CommandParameterProperty);
            command.TryExecute(parameter);

            control.RaiseEvent(new(ClickEvent, control));
        }
        catch (OperationCanceledException)
        {
            // Задача была отменена - это нормально
        }
        finally
        {
            // Очищаем CancellationTokenSource если он всё ещё наш
            if (ReferenceEquals(control.GetValue(__CancellationTokenSource), cts))
            {
                control.ClearValue(__CancellationTokenSource);
                cts.Dispose();
            }
        }
    }

    private static void OnMouseUp(object sender, MouseButtonEventArgs e) 
    {
        var control = (DependencyObject)sender;
        control.ClearValue(__LastClickTime);
        
        // Отменяем текущую задачу
        if (control.GetValue(__CancellationTokenSource) is CancellationTokenSource cts)
        {
            cts.Cancel();
            control.ClearValue(__CancellationTokenSource);
        }
    }

    private static void OnMouseLeave(object sender, MouseEventArgs e)
    {
        var control = (DependencyObject)sender;
        control.ClearValue(__LastClickTime);
        
        // Отменяем текущую задачу при уходе мыши с элемента
        if (control.GetValue(__CancellationTokenSource) is CancellationTokenSource cts)
        {
            cts.Cancel();
            control.ClearValue(__CancellationTokenSource);
        }
    }

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
    public static object GetCommandParameter(DependencyObject D) => D.GetValue(CommandParameterProperty);

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

}
