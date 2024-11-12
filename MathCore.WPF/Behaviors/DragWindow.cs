using System.Windows.Input;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для перетаскивания окна мышью</summary>
public class DragWindow : WindowBehavior
{
    /// <summary>Вызывается при подключении поведения к окну</summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        // Подписываемся на событие нажатия левой кнопки мыши
        AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
    }

    /// <summary>Вызывается при отключении поведения от окна</summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        // Отписываемся от события нажатия левой кнопки мыши
        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
    }

    /// <summary>Обработчик события нажатия левой кнопки мыши</summary>
    /// <param name="Sender">Источник события</param>
    /// <param name="E">Аргументы события</param>
    protected virtual void OnMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
    {
        // Если клик был двойным, то ничего не делаем
        if (E.ClickCount > 1) return;
        // Перетаскиваем окно
        AssociatedWindow?.DragMove();
    }
}