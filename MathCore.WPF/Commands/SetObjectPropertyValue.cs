using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace MathCore.WPF.Commands;

public class SetObjectPropertyValue : Freezable, ICommand
{
    #region Target : object - Целевой объект

    /// <summary>Целевой объект</summary>
    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.Register(
            nameof(Target),
            typeof(object),
            typeof(SetObjectPropertyValue),
            new PropertyMetadata(default(object)));

    /// <summary>Целевой объект</summary>
    //[Category("")]
    [Description("Целевой объект")]
    public object Target { get => (object)GetValue(TargetProperty); set => SetValue(TargetProperty, value); }

    #endregion

    #region PropertyName : string - Имя свойства

    /// <summary>Имя свойства</summary>
    public static readonly DependencyProperty PropertyNameProperty =
        DependencyProperty.Register(
            nameof(PropertyName),
            typeof(string),
            typeof(SetObjectPropertyValue),
            new PropertyMetadata(default(object)));

    /// <summary>Имя свойства</summary>
    //[Category("")]
    [Description("Имя свойства")]
    public string PropertyName { get => (string)GetValue(PropertyNameProperty); set => SetValue(PropertyNameProperty, value); }

    #endregion

    #region Value : Object - Требуемое значение

    /// <summary>Требуемое значение</summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(object),
            typeof(SetObjectPropertyValue),
            new PropertyMetadata(default(object)));

    /// <summary>Требуемое значение</summary>
    //[Category("")]
    [Description("Требуемое значение")]
    public object Value { get => (object)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

    #endregion

    public enum ParameterApplyTo
    {
        None,
        Target,
        PropertyName,
        Value
    }

    public ParameterApplyTo ApplyTo { get; set; } = ParameterApplyTo.None;

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    protected override Freezable CreateInstanceCore() => new SetObjectPropertyValue
    {
        Target = Target,
        PropertyName = PropertyName,
        Value = Value
    };

    public bool CanExecute(object? parameter)
    {
        var target = ApplyTo == ParameterApplyTo.Target ? parameter : Target;
        if (target is null) return false;

        var property_name = ApplyTo == ParameterApplyTo.PropertyName ? parameter?.ToString() : PropertyName;
        if (string.IsNullOrWhiteSpace(property_name)) return false;
        //var property_info = Target.GetType().GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.Public);
        //if (property_info is null) return false;
        //var value = Value;
        //if (value is null && !property_info.PropertyType.IsByRef && property_info.PropertyType.Name != "Nullable`1") return false;
        return true;
    }

    public void Execute(object? parameter)
    {
        var target = ApplyTo == ParameterApplyTo.Target ? parameter : Target;

        var target_type = target?.GetType();
        var property_name = ApplyTo == ParameterApplyTo.PropertyName ? parameter?.ToString() : PropertyName;
        if (string.IsNullOrWhiteSpace(property_name)) return;

        var property_info = target_type?.GetProperty(property_name, BindingFlags.Instance | BindingFlags.Public);
        if (property_info is null) return;

        var value = ApplyTo == ParameterApplyTo.Value ? parameter : Value;
        var property_type = property_info.PropertyType;

        //if (value is null && !property_type.IsByRef && property_type.Name != "Nullable`1") return;

        property_info.SetValue(target, value);
    }
}