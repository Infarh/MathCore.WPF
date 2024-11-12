using System.Windows;
using System.Windows.Controls;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF;

/// <summary>Класс, наследующий ColumnDefinition, добавляющий свойство видимости.</summary>
public class ColumnDefinitionCollapsible : ColumnDefinition
{
    /// <summary>Статический конструктор, инициализирующий метаданные свойств.</summary>
    static ColumnDefinitionCollapsible()
    {
        // OverrideMetadata для свойства Width, чтобы установить значение по умолчанию и коэрцитивный callback
        WidthProperty.OverrideMetadata(
            typeof(ColumnDefinitionCollapsible),
            new FrameworkPropertyMetadata(
                new GridLength(1, GridUnitType.Star), // Значение по умолчанию
                null, // PropertyChangedCallback
                (d, v) => ((ColumnDefinitionCollapsible)d).Visible ? v : new GridLength(0))); // CoerceValueCallback

        // OverrideMetadata для свойства MinWidth, чтобы установить значение по умолчанию и коэрцитивный callback
        MinWidthProperty.OverrideMetadata(
            typeof(ColumnDefinitionCollapsible),
            new FrameworkPropertyMetadata(0d, null, (d, v) => ((ColumnDefinitionCollapsible)d).Visible ? v : 0d));
    }

    #region Visible : bool - Видимость

    /// <summary>DependencyProperty для свойства видимости.</summary>
    public static readonly DependencyProperty VisibleProperty =
        DependencyProperty.Register(
            nameof(Visible), // Имя свойства
            typeof(bool), // Тип свойства
            typeof(ColumnDefinitionCollapsible), // Тип владельца свойства
            new(true, OnVisibleChanged)); // Метаданные свойства

    /// <summary>Callback, вызываемый при изменении свойства видимости.</summary>
    /// <param name="d">Объект, владеющий свойством.</param>
    /// <param name="e">Аргументы события изменения свойства.</param>
    private static void OnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Вызов коэрцитивных callback для свойств Width и MinWidth
        d.CoerceValue(WidthProperty);
        d.CoerceValue(MinWidthProperty);
    }

    /// <summary>Свойство видимости.</summary>
    public bool Visible
    {
        get => (bool)GetValue(VisibleProperty); // Геттер
        set => SetValue(VisibleProperty, value); // Сеттер
    }

    #endregion
}