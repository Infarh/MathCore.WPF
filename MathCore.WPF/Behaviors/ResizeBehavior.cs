using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение, позволяющее изменять размер элемента UIElement.</summary>
public class ResizeBehavior : Behavior<UIElement>
{
    // Слой адорнера, используемый для отображения адорнера изменения размера.
    private AdornerLayer? _AdornerLayer;

    // Элемент фреймворка, связанный с этим поведением.
    private FrameworkElement? _Element;

    // Элемент UI, к которому присоединено это поведение.
    private UIElement? _AttachedElement;

    /// <summary>Вызывается, когда поведение присоединяется к элементу UIElement.</summary>
    protected override void OnAttached()
    {
        // Получить присоединенный элемент и его родительский элемент.
        _AttachedElement = AssociatedObject;
        _Element = (FrameworkElement)_AttachedElement;

        // Если родительский элемент не равен null, подпишитесь на его событие Loaded.
        if (_Element?.Parent != null)
        {
            ((FrameworkElement)_Element.Parent).Loaded += ResizeBehaviorParent_Loaded;
        }
    }

    /// <summary>Вызывается, когда поведение отсоединяется от элемента UIElement.</summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        // Очистить ссылку на слой адорнера.
        _AdornerLayer = null;
    }

    /// <summary>Создает слой адорнера, когда родительский элемент текущего элемента загружается.</summary>
    /// <param name="sender">Отправитель события Loaded.</param>
    /// <param name="e">Аргументы события Loaded.</param>
    private void ResizeBehaviorParent_Loaded(object sender, RoutedEventArgs e)
    {
        // Получить слой адорнера для родительского элемента.
        _AdornerLayer ??= AdornerLayer.GetAdornerLayer(sender as Visual ?? throw new InvalidOperationException());

        // Подпишитесь на событие MouseEnter присоединенного элемента.
        _AttachedElement!.MouseEnter += AttachedElement_MouseEnter;
    }

    /// <summary>
    /// Вызывается, когда курсор мыши входит в присоединенный элемент.
    /// Создает новый адорнер изменения размера.
    /// </summary>
    /// <param name="sender">Отправитель события MouseEnter.</param>
    /// <param name="e">Аргументы события MouseEnter.</param>
    private void AttachedElement_MouseEnter(object sender, MouseEventArgs e)
    {
        // Создать новый адорнер изменения размера.
        var resizingAdorner = new ResizingAdorner(sender as UIElement ?? throw new InvalidOperationException());

        // Подпишитесь на событие MouseLeave адорнера изменения размера.
        resizingAdorner.MouseLeave += ResizingAdorner_MouseLeave;

        // Добавить адорнер изменения размера в слой адорнера.
        _AdornerLayer!.Add(resizingAdorner);
    }

    /// <summary>
    /// Вызывается, когда курсор мыши покидает адорнер изменения размера.
    /// Удаляет адорнер изменения размера из слоя адорнера.
    /// </summary>
    /// <param name="sender">Отправитель события MouseLeave.</param>
    /// <param name="e">Аргументы события MouseLeave.</param>
    private void ResizingAdorner_MouseLeave(object? sender, MouseEventArgs e)
    {
        // Если отправитель не равен null, удалите его из слоя адорнера.
        if (sender != null)
        {
            _AdornerLayer!.Remove(sender as ResizingAdorner ?? throw new InvalidOperationException());
        }
    }
}