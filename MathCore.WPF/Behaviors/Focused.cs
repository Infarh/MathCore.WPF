using System.Windows;
using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для установки фокуса на элемент при его инициализации.</summary>
public class Focused : Behavior<FrameworkElement>
{
    /// <summary>Вызывается после присоединения поведения к элементу.</summary>
    protected override void OnAttached()
    {
        // Вызов базового метода для выполнения стандартных действий при присоединении поведения.
        base.OnAttached();

        // Подписка на событие Initialized элемента для установки фокуса при его инициализации.
        AssociatedObject.Initialized += (s, _) =>
        {
            // Установка фокуса на элемент, если он не равен null.
            (s as FrameworkElement)?.Focus();
        };

        // Установка фокуса на элемент сразу после присоединения поведения.
        AssociatedObject.Focus();
    }
}