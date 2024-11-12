using System.Linq.Expressions;
using System.Windows;

namespace MathCore.WPF;

/// <summary>Класс для регистрации свойств зависимостей.</summary>
public static class DependencyPropertyBuilder
{
    /// <summary>Регистрирует свойство зависимости для типа <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">Тип, для которого регистрируется свойство зависимости.</typeparam>
    /// <returns>Экземпляр <see cref="DependencyPropertyClassBuilder{T}"/>, который позволяет продолжить процесс регистрации свойства.</returns>
    public static DependencyPropertyClassBuilder<T> Register<T>() where T : DependencyObject => new();

    /// <summary>Регистрирует свойство зависимости для типа <typeparamref name="T"/> с указанным селектором свойства.</summary>
    /// <typeparam name="T">Тип, для которого регистрируется свойство зависимости.</typeparam>
    /// <typeparam name="TValue">Тип значения свойства.</typeparam>
    /// <param name="PropertySelector">Селектор свойства, который будет использоваться для регистрации свойства.</param>
    /// <returns>Экземпляр <see cref="DependencyPropertyBuilder{T, TValue}"/>, который позволяет продолжить процесс регистрации свойства.</returns>
    public static DependencyPropertyBuilder<T, TValue> Register<T, TValue>(Expression<Func<T, TValue>> PropertySelector)
        where T : DependencyObject => Register<T>().Property(PropertySelector);
}

/// <summary>Строитель класса свойств зависимости для типа <typeparamref name="T"/>.</summary>
/// <typeparam name="T">Тип объекта, для которого строится класс свойств зависимости.</typeparam>
public readonly ref struct DependencyPropertyClassBuilder<T> where T : DependencyObject
{
    /// <summary>Создаёт строитель свойства зависимости для типа <typeparamref name="TValue"/>.</summary>
    /// <typeparam name="TValue">Тип значения свойства зависимости.</typeparam>
    /// <param name="PropertySelector">Выражение, выбирающее свойство объекта.</param>
    /// <returns>Строитель свойства зависимости.</returns>
    /// <exception cref="InvalidOperationException">Если селектор не выбирает свойство объекта.</exception>
    public DependencyPropertyBuilder<T, TValue> Property<TValue>(Expression<Func<T, TValue>> PropertySelector)
    {
        // Проверяем, что селектор выбирает свойство объекта
        if (PropertySelector is not { Body: MemberExpression { Member.Name: var name } })
            throw new InvalidOperationException("Селектор должен выбирать свойство объекта");

        // Создаём строитель свойства зависимости
        return new(name);
    }
}

public readonly ref struct DependencyPropertyBuilder<T, TValue>(string Name) where T : DependencyObject
{
    public delegate void PropertyChangedHandler(T Sender, TValue? OldValue, TValue? NewValue);

    public string Name { get; init; } = Name;

    public PropertyMetadata? Metadata { get; init; } = new();

    public TValue? DefaultValue { get; init; } = default;

    public PropertyChangedCallback? PropertyChangedCallback { get; init; } = null;

    public CoerceValueCallback? CoerceValueCallback { get; init; } = null;

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

    public static implicit operator DependencyProperty(DependencyPropertyBuilder<T, TValue> builder) => builder.Build();
}
