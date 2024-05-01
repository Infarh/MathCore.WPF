using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable CollectionNeverUpdated.Global

namespace MathCore.WPF.Converters;

// <summary>
// Трансформатор, который преобразует входные значения в выходные значения на основе списка дочерних элементов переключателя.
// Это не является строго выражением 'switch' в стиле C, поскольку случаи не гарантированно уникальны.
// </summary>
[ContentProperty("Cases")]
public class SwitchConverter : DependencyObject, IValueConverter
{
    public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register(
        nameof(DefaultValue), 
        typeof(object), 
        typeof(SwitchConverter));

    public object DefaultValue
    {
        get => GetValue(DefaultValueProperty);
        set => SetValue(DefaultValueProperty, value);
    }

    public List<SwitchCase> Cases { get; } = [];

    // IValueConverter implementation
    public object? Convert(object? v, Type t, object? p, CultureInfo c)
    {
        foreach (var switch_case in Cases)
            if (Equals(switch_case.In, v))
                return switch_case.Out;

        return DefaultValue;
    }

    public object? ConvertBack(object? v, Type t, object? p, CultureInfo c)
    {
        if (Equals(v, DefaultValue))
            return null;

        foreach (var switch_case in Cases)
            if (Equals(switch_case.Out, v))
                return switch_case.In;

        return null;
    }
}

public class SwitchCase : DependencyObject
{
    public static readonly DependencyProperty InProperty = DependencyProperty.Register(
        nameof(In), 
        typeof(object),
        typeof(SwitchCase));

    public object In
    {
        get => GetValue(InProperty);
        set => SetValue(InProperty, value);
    }

    public static readonly DependencyProperty OutProperty = DependencyProperty.Register(
        nameof(Out), 
        typeof(object),
        typeof(SwitchCase));

    public object Out
    {
        get => GetValue(OutProperty);
        set => SetValue(OutProperty, value);
    }
}

public sealed class TypeReference : DependencyObject
{
    public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
        nameof(Type), 
        typeof(Type),
        typeof(TypeReference));

    public Type Type
    {
        get => (Type)GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }
}
