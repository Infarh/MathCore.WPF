using System.Linq.Expressions;
using System.Windows;

namespace MathCore.WPF;

public static class DependencyPropertyBuilder
{
    public static DependencyPropertyClassBuilder<T> Register<T>() where T : DependencyObject => new();
}

public readonly ref struct DependencyPropertyClassBuilder<T> where T : DependencyObject
{
    public DependencyPropertyBuilder<T, TValue> Property<TValue>(Expression<Func<T, TValue>> PropertySelector)
    {
        if (PropertySelector is not { Body: MemberExpression { Member.Name: var name } })
            throw new InvalidOperationException("Селектор должен выбирать свойство объекта");
        return new(name);
    }
}

public readonly ref struct DependencyPropertyBuilder<T, TValue> where T : DependencyObject
{
    public delegate void PropertyChangedHandler(T Sender, TValue? OldValue, TValue? NewValue);

    public string Name { get; init; }

    public PropertyMetadata? Metadata { get; init; } = new();

    public TValue? DefaultValue { get; init; } = default;

    public PropertyChangedCallback? PropertyChangedCallback { get; init; } = null;
    public CoerceValueCallback? CoerceValueCallback { get; init; } = null;

    public DependencyPropertyBuilder(string Name) => this.Name = Name;

    public DependencyPropertyBuilder<T, TValue> WithDefaultValue(TValue value) => this with { DefaultValue = value };

    public DependencyPropertyBuilder<T, TValue> OnChanged(PropertyChangedCallback? ChangedCallback) =>
        ChangedCallback is null ? this with { PropertyChangedCallback = null }
            : PropertyChangedCallback is null
                ? this with { PropertyChangedCallback = ChangedCallback }
                : this with { PropertyChangedCallback = PropertyChangedCallback + ChangedCallback };

    public DependencyPropertyBuilder<T, TValue> OnChanged(PropertyChangedHandler? ChangedCallback) =>
        ChangedCallback is null ? this with { PropertyChangedCallback = null } 
            : PropertyChangedCallback is null
                ? this with { PropertyChangedCallback = (d, e) => ChangedCallback((T)d, (TValue?)e.OldValue, (TValue?)e.NewValue) }
                : this with { PropertyChangedCallback = PropertyChangedCallback + ((d, e) => ChangedCallback((T)d, (TValue?)e.OldValue, (TValue?)e.NewValue)) };

    public DependencyPropertyBuilder<T, TValue> CoerceValue(CoerceValueCallback? Coerce) => this with { CoerceValueCallback = Coerce };

    public DependencyProperty Build()
    {
        Metadata.DefaultValue = DefaultValue;
        Metadata.PropertyChangedCallback = PropertyChangedCallback;
        Metadata.CoerceValueCallback = CoerceValueCallback;

        switch (Metadata)
        {
            case FrameworkPropertyMetadata framework_metadata:

                break;

            case UIPropertyMetadata ui_metadata:
                //ui_metadata.
                break;
        }

        var property = DependencyProperty
           .Register(
                Name,
                typeof(T),
                typeof(TValue),
                Metadata);

        //PropertyMetadata p = new PropertyMetadata(DefaultValue, );
        //p.DefaultValue = null;

        return property;
    }
}
