using System.Windows;
using System.Windows.Input;

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для заголовка окна, позволяющее перетаскивать окно за заголовок.</summary>
public class WindowTitleBarBehavior : DragWindow
{
    /// <summary>Обработчик события нажатия левой кнопки мыши.</summary>
    /// <param name="Sender">Источник события.</param>
    /// <param name="E">Аргументы события.</param>
    protected override void OnMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
    {
        // Получаем связанное окно
        var window = AssociatedWindow;
        if (window is null) return; // Если окно не найдено, выходим

        // Проверяем количество кликов
        if (E.ClickCount == 1)
        {
            // Если один клик, проверяем состояние окна
            switch (window.WindowState)
            {
                case WindowState.Maximized:
                    // Если окно развернуто, сворачиваем его и устанавливаем положение сверху
                    window.WindowState = WindowState.Normal;
                    window.Top = 0;
                    base.OnMouseLeftButtonDown(Sender, E); // Вызываем базовый метод
                    break;
                case WindowState.Normal:
                    // Если окно в нормальном состоянии, просто вызываем базовый метод
                    base.OnMouseLeftButtonDown(Sender, E);
                    break;
            }
        }
        else
        {
            // Если двойной клик, переключаем состояние окна между развернутым и нормальным
            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
    }
}