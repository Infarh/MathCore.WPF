using System.Windows.Input;
using MathCore.WPF.pInvoke;

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для окон, позволяющее управлять системными командами окна при нажатии левой кнопки мыши.</summary>
public class WindowSystemIconBehavior : WindowBehavior
{
    /// <summary>Вызывается при присоединении поведения к окну.</summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        // Подписываемся на событие нажатия левой кнопки мыши.
        AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
    }

    /// <summary>Вызывается при отсоединении поведения от окна.</summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        // Отписываемся от события нажатия левой кнопки мыши.
        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
    }

    /// <summary>Обработчик события нажатия левой кнопки мыши.</summary>
    /// <param name="Sender">Источник события.</param>
    /// <param name="E">Аргументы события.</param>
    protected void OnMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
    {
        // Отменяем обработку события по умолчанию.
        E.Handled = true;

        // Если нажатие произошло дважды, закрываем окно.
        if (E.ClickCount > 1)
            AssociatedWindow?.Close();
        else
        {
            // Если нажатие произошло один раз, отправляем системную команду для отображения меню окна.
            AssociatedWindow?.SendMessage(WM.SYSCOMMAND, SC.KEYMENU);
        }
    }
}