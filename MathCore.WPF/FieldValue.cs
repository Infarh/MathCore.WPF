using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace MathCore.WPF;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class FieldValue : Freezable
{
    #region Value : object - Значение поля

    /// <summary>Значение поля</summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(object),
            typeof(FieldValue),
            new(default, OnValuePropertyChanged));

    private static void OnValuePropertyChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        if (D is not FieldValue value || value._Field is null) return;
        value._Field.Value = E.NewValue;
    }

    /// <summary>Значение поля</summary>
    public object Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

    #endregion

    private Field? _Field;

    internal void Initialize(Field field) => _Field = field;

    protected override Freezable CreateInstanceCore() => new FieldValue { _Field = _Field };
}