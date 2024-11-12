using System.Windows;
using System.Windows.Controls;

namespace MathCore.WPF;

/// <summary> Класс, наследующий RowDefinition, добавляющий свойство видимости.</summary>
public class RowDefinitionCollapsable : RowDefinition
{
    /// <summary> Статический конструктор, переопределяющий метаданные свойств Height и MinHeight.</summary>
    static RowDefinitionCollapsable()
    {
        // Переопределение метаданных свойства Height, чтобы оно зависело от видимости.
        HeightProperty.OverrideMetadata(
            typeof(RowDefinitionCollapsable),
            new FrameworkPropertyMetadata(
                new GridLength(1, GridUnitType.Star),
                null,
                (d, v) => ((RowDefinitionCollapsable)d).Visible ? v : new GridLength(0)));

        // Переопределение метаданных свойства MinHeight, чтобы оно зависело от видимости.
        MinHeightProperty.OverrideMetadata(
            typeof(RowDefinitionCollapsable),
            new FrameworkPropertyMetadata(0d, null, (d, v) => ((RowDefinitionCollapsable)d).Visible ? v : 0d));
    }

    #region Visible : bool - Видимость

    /// <summary> DependencyProperty для свойства видимости.</summary>
    public static readonly DependencyProperty VisibleProperty =
        DependencyProperty.Register(
            nameof(Visible),
            typeof(bool),
            typeof(RowDefinitionCollapsable),
            new(true, OnVisibleChanged));

    /// <summary> Метод, вызываемый при изменении свойства видимости.</summary>
    /// <param name="d">Объект, свойство которого изменилось.</param>
    /// <param name="e">Аргументы события изменения свойства.</param>
    private static void OnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Принудительное обновление свойств Height и MinHeight после изменения видимости.
        d.CoerceValue(HeightProperty);
        d.CoerceValue(MinHeightProperty);
    }

    /// <summary> Свойство видимости.</summary>
    public bool Visible
    {
        get => (bool)GetValue(VisibleProperty);
        set => SetValue(VisibleProperty, value);
    }

    #endregion
}