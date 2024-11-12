using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для установки положения курсора в конце текста в TextBox при получении фокуса.</summary>
public class TextBoxEndCaretAtGotFocus : Behavior<TextBox>
{
    /// <summary>Вызывается при подключении поведения к элементу.</summary>
    protected override void OnAttached()
    {
        // Подписываемся на событие получения фокуса элементом TextBox.
        AssociatedObject.GotFocus += OnTextBoxGotFocus;
    }

    /// <summary>Вызывается при отключении поведения от элемента.</summary>
    protected override void OnDetaching()
    {
        // Отписываемся от события получения фокуса элементом TextBox.
        AssociatedObject.GotFocus -= OnTextBoxGotFocus;
    }

    /// <summary>Обработчик события получения фокуса элементом TextBox.</summary>
    /// <param name="Sender">Источник события.</param>
    /// <param name="E">Аргументы события.</param>
    private static void OnTextBoxGotFocus(object Sender, RoutedEventArgs E)
    {
        // Проверяем, что источник события является элементом TextBox.
        if (Sender is not TextBox edit) return;

        // Устанавливаем положение курсора в конце текста.
        edit.CaretIndex = edit.Text.Length;
    }
}