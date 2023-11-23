using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

using MathCore.Annotations;

namespace MathCore.WPF;

/// <summary>Свойство модели</summary>
/// <remarks>Инициализация нового свойства модели</remarks>
/// <param name="Name">Имя свойства модели</param>
public sealed class ModelProperty(string Name) : Freezable, INotifyPropertyChanged
{
    /// <summary>Инициализация нового свойства модели</summary>
    public ModelProperty() : this("noname") { }

    /// <summary>Значение свойства</summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(object),
            typeof(ModelProperty),
            new PropertyMetadata(default, (d, _) => ((ModelProperty)d).OnPropertyChanged(nameof(Value))));

    /// <summary>Событие, возникает когда значение свойства модели меняется</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Генерация события изменения значения свойства</summary>
    /// <param name="PropertyName">Имя изменившегося свойства</param>
    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string PropertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

    /// <summary>Значение свойства</summary>
    public object Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>Имя свойства модели</summary>
    public string Name { get; set; } = Name.NotNull();

    /// <summary>Инициализация нового свойства модели</summary>
    /// <param name="Name">Имя свойства модели</param>
    /// <param name="Value">Значение свойства модели</param>
    public ModelProperty(string Name, object? Value) : this(Name) => this.Value = Value;

    /// <inheritdoc />
    protected override Freezable CreateInstanceCore() => new ModelProperty(Name);
}