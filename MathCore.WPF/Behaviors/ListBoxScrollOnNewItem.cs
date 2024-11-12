using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для ListBox, которое автоматически прокручивает список при добавлении нового элемента.</summary>
public class ListBoxScrollOnNewItem : Behavior<ListBox>
{
    #region Enabled : bool - Включено

    /// <summary>DependencyProperty для свойства Enabled.</summary>
    public static readonly DependencyProperty EnabledProperty =
        DependencyProperty.Register(
            nameof(Enabled),
            typeof(bool),
            typeof(ListBoxScrollOnNewItem),
            new(true));

    /// <summary>Включает или выключает поведение.</summary>
    [Description("Включено")]
    public bool Enabled { get => (bool)GetValue(EnabledProperty); set => SetValue(EnabledProperty, value); }

    #endregion

    /// <summary>Вызывается при присоединении поведения к элементу ListBox.</summary>
    protected override void OnAttached()
    {
        // Получаем ссылку на элемент ListBox
        var list_box = AssociatedObject;

        // Получаем дескриптор свойства ItemsSource
        var items_source_property_descriptor = TypeDescriptor.GetProperties(list_box)["ItemsSource"];

        // Подписываемся на изменение свойства ItemsSource
        items_source_property_descriptor.AddValueChanged(list_box, ListBox_ItemsSourceChanged);

        // Устанавливаем коллекцию для наблюдения
        SetCollection(list_box.ItemsSource as INotifyCollectionChanged);
    }

    /// <summary>Коллекция, которая наблюдается за изменениями.</summary>
    private INotifyCollectionChanged? _ListCollection;

    /// <summary>Устанавливает коллекцию для наблюдения.</summary>
    /// <param name="collection">Коллекция для наблюдения.</param>
    private void SetCollection(INotifyCollectionChanged? collection)
    {
        // Если уже есть коллекция, отписываемся от нее
        if (_ListCollection is { } old_collection)
            old_collection.CollectionChanged -= OnCollectionChanged;

        _ListCollection = collection;

        // Если новая коллекция не null, подписываемся на ее изменение
        if (collection is not null)
            collection.CollectionChanged += OnCollectionChanged;
    }

    /// <summary>Вызывается при изменении свойства ItemsSource.</summary>
    /// <param name="Sender">Источник события.</param>
    /// <param name="E">Аргументы события.</param>
    private void ListBox_ItemsSourceChanged(object? Sender, EventArgs E) => SetCollection(AssociatedObject?.ItemsSource as INotifyCollectionChanged);

    /// <summary>Вызывается при отсоединении поведения от элемента ListBox.</summary>
    protected override void OnDetaching()
    {
        // Получаем ссылку на элемент ListBox
        var list_box = AssociatedObject;

        // Получаем дескриптор свойства ItemsSource
        var items_source_property_descriptor = TypeDescriptor.GetProperties(list_box)["ItemsSource"];

        // Отписываемся от изменений свойства ItemsSource
        items_source_property_descriptor.RemoveValueChanged(list_box, ListBox_ItemsSourceChanged);

        // Удаляем коллекцию для наблюдения
        SetCollection(null);
    }

    /// <summary>Вызывается при изменении коллекции.</summary>
    /// <param name="Sender">Источник события.</param>
    /// <param name="E">Аргументы события.</param>
    private void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
    {
        // Если поведение выключено или добавлен не один элемент, выходим
        if (!Enabled || E.Action != NotifyCollectionChangedAction.Add || E.NewItems is not [ var new_item, .. ]) return;

        // Получаем ссылку на элемент ListBox
        var list = AssociatedObject;

        // Прокручиваем список до нового элемента
        list.ScrollIntoView(new_item);

        // Выбираем новый элемент
        list.SelectedItem = new_item;
    }
}