using System.ComponentModel;
using System.Windows;

using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Triggers;

public class DebugActionTrigger : TriggerAction<DependencyObject>
{
    public static event Action<string, object, DependencyObject, object> TriggerEvent;

    #region Message : string - Сообщение

    /// <summary>Сообщение</summary>
    //[Category("")]
    [Description("Сообщение")]
    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>Сообщение</summary>
    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register(
            nameof(Message),
            typeof(string),
            typeof(DebugActionTrigger),
            new PropertyMetadata(default(string)));

    #endregion

    #region MessageParameter : object - Параметр сообщения

    /// <summary>Параметр сообщения</summary>
    //[Category("")]
    [Description("Параметр сообщения")]
    public object MessageParameter
    {
        get => (object)GetValue(MessageParameterProperty);
        set => SetValue(MessageParameterProperty, value);
    }

    /// <summary>Параметр сообщения</summary>
    public static readonly DependencyProperty MessageParameterProperty =
        DependencyProperty.Register(
            nameof(MessageParameter),
            typeof(object),
            typeof(DebugActionTrigger),
            new PropertyMetadata(default));

    #endregion

    protected override void Invoke(object parameter) => TriggerEvent?.Invoke(Message, MessageParameter, AssociatedObject, parameter);
}
