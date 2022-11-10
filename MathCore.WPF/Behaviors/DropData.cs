using System.Windows;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Behaviors;

public class DropData : Behavior<UIElement>
{
    #region DropDataCommand : ICommand - Команда, вызываемая в момент получения данных

    /// <summary>Команда, вызываемая в момент получения данных</summary>
    public static readonly DependencyProperty DropDataCommandProperty =
        DependencyProperty.Register(
            nameof(DropDataCommand),
            typeof(ICommand),
            typeof(DropData),
            new PropertyMetadata(default(ICommand)));

    /// <summary>Команда, вызываемая в момент получения данных</summary>
    public ICommand? DropDataCommand
    {
        get => (ICommand)GetValue(DropDataCommandProperty);
        set => SetValue(DropDataCommandProperty, value);
    }

    #endregion

    #region DataFormat : string - Предпочитаемый формат данных

    /// <summary>Предпочитаемый формат данных</summary>
    public static readonly DependencyProperty DataFormatProperty =
        DependencyProperty.Register(
            nameof(DataFormat),
            typeof(string),
            typeof(DropData),
            new PropertyMetadata(default(string)));

    /// <summary>Предпочитаемый формат данных</summary>
    public string? DataFormat
    {
        get => (string)GetValue(DataFormatProperty);
        set => SetValue(DataFormatProperty, value);
    }

    #endregion

    #region DataFormatAutoConversation : bool - Автоматически преобразовывать данные

    /// <summary>Автоматически преобразовывать данные</summary>
    public static readonly DependencyProperty DataFormatAutoConversationProperty =
        DependencyProperty.Register(
            nameof(DataFormatAutoConversation),
            typeof(bool),
            typeof(DropData),
            new PropertyMetadata(true));

    /// <summary>Автоматически преобразовывать данные</summary>
    public bool DataFormatAutoConversation
    {
        get => (bool)GetValue(DataFormatAutoConversationProperty);
        set => SetValue(DataFormatAutoConversationProperty, value);
    }

    #endregion

    #region DataType : Type - Предпочитаемый тип данных

    /// <summary>Предпочитаемый тип данных</summary>
    public static readonly DependencyProperty DataTypeProperty =
        DependencyProperty.Register(
            nameof(DataType),
            typeof(Type),
            typeof(DropData),
            new PropertyMetadata(default(Type)));

    /// <summary>Предпочитаемый тип данных</summary>
    public Type? DataType
    {
        get => (Type)GetValue(DataTypeProperty);
        set => SetValue(DataTypeProperty, value);
    }

    #endregion

    private bool _LastAllowDropValue;
    protected override void OnAttached()
    {
        var element = AssociatedObject;
        _LastAllowDropValue =  element.AllowDrop;
        element.AllowDrop   =  true;
        element.Drop        += OnDropData;
    }

    protected override void OnDetaching()
    {
        var element = AssociatedObject;
        element.AllowDrop =  _LastAllowDropValue;
        element.Drop      -= OnDropData;
    }

    private void OnDropData(object Sender, DragEventArgs E)
    {
        var command = DropDataCommand;
        if (command is null) return;
        var data      = E.Data;
        var data_type = DataType;
        if (data_type != null)
        {
            if (!data.GetDataPresent(data_type)) return;
            var value = data.GetData(data_type);
            if (command.CanExecute(value))
                command.Execute(value);
            return;
        }

        var data_format = DataFormat;
        if (!string.IsNullOrWhiteSpace(data_format))
        {
            var auto_convertible = DataFormatAutoConversation;
            if (!data.GetDataPresent(data_format, auto_convertible)) return;
            var value = data.GetData(data_format, auto_convertible);
            if (command.CanExecute(value))
                command.Execute(value);
            return;
        }

        var str = data.GetData(DataFormats.StringFormat, true);
        if (command.CanExecute(str))
            command.Execute(str);
    }
}