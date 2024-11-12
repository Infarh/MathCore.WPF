using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для кнопки закрытия приложения</summary>
public class CloseButtonBehavior : Behavior<Button>
{
    /// <summary>Вызывается при подключении поведения к кнопке</summary>
    protected override void OnAttached()
    {
        // Подписка на событие клика по кнопке
        AssociatedObject.Click += OnClick;
    }

    /// <summary>Вызывается при отключении поведения от кнопки</summary>
    protected override void OnDetaching()
    {
        // Отмена подписки на событие клика по кнопке
        AssociatedObject.Click -= OnClick;
    }

    /// <summary>Обработчик события клика по кнопке</summary>
    /// <param name="sender">Источник события</param>
    /// <param name="e">Аргументы события</param>
    private static void OnClick(object sender, RoutedEventArgs e)
    {
        // Завершение работы приложения
        Application.Current.Shutdown();
    }
}