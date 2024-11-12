using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для привязки выбранных элементов в ListBox.</summary>
public class ListBoxItemsSelectionBinder : Behavior<ListBox>
{
    #region SelectedItems : IList - Выбранные элементы

    /// <summary>DependencyProperty для свойства SelectedItems.</summary>
    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.Register(
            nameof(SelectedItems),
            typeof(IList),
            typeof(ListBoxItemsSelectionBinder),
            new FrameworkPropertyMetadata(
                default(IList),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedItemsChanged));

    /// <summary>Обработчик изменения свойства SelectedItems.</summary>
    /// <param name="D">Объект зависимости.</param>
    /// <param name="E">Аргументы события.</param>
    private static void OnSelectedItemsChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        // Получаем поведение и список выбранных элементов.
        var behavior = (ListBoxItemsSelectionBinder)D;
        var listSelectedItems = behavior.AssociatedObject.SelectedItems;
        var newSelectedItems = (IList)E.NewValue;

        // Если списки совпадают, то ничего не делаем.
        if (ReferenceEquals(listSelectedItems, newSelectedItems)) return;

        // Создаём список элементов, которые нужно удалить.
        var toRemove = new List<object>();
        foreach (var listItem in listSelectedItems)
            if (!newSelectedItems.Contains(listItem) && listItem is not null)
                toRemove.Add(listItem);

        // Удаляем элементы из списка выбранных элементов.
        foreach (var item in toRemove)
            listSelectedItems.Remove(item);

        // Добавляем новые элементы в список выбранных элементов.
        foreach (var newSelectedItem in newSelectedItems)
            if (!listSelectedItems.Contains(newSelectedItem))
                listSelectedItems.Add(newSelectedItem);
    }

    /// <summary>Выбранные элементы.</summary>
    [Description("Выбранные элементы")]
    public IList SelectedItems
    {
        get => (IList)GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    #endregion

    /// <summary>Вызывается при присоединении поведения к элементу.</summary>
    protected override void OnAttached()
    {
        // Подписываемся на событие изменения выбора.
        AssociatedObject.SelectionChanged += OnSelectionChanged;
    }

    /// <summary>Вызывается при отсоединении поведения от элемента.</summary>
    protected override void OnDetaching()
    {
        // Отписываемся от события изменения выбора.
        AssociatedObject.SelectionChanged -= OnSelectionChanged;
    }

    /// <summary>Обработчик события изменения выбора.</summary>
    /// <param name="sender">Источник события.</param>
    /// <param name="e">Аргументы события.</param>
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
        // Обновляем свойство SelectedItems.
        SelectedItems = AssociatedObject.SelectedItems;
}