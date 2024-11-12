using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

namespace MathCore.WPF.Behaviors;

/// <summary>Поведение для текстового поля, которое позволяет выделить весь текст при получении фокуса.</summary>
public class TextBoxSelectAllAtGotFocus : Behavior<TextBox>
{
    /// <summary>Вызывается, когда поведение присоединяется к текстовому полю.</summary>
    protected override void OnAttached()
    {
        // Подписываемся на события получения фокуса и изменения текста
        AssociatedObject.GotFocus += OnTextBoxGotFocus;
        AssociatedObject.TextChanged += OnTextChanged;
    }

    /// <summary>Вызывается, когда поведение отсоединяется от текстового поля.</summary>
    protected override void OnDetaching()
    {
        // Отписываемся от событий получения фокуса и изменения текста
        AssociatedObject.GotFocus -= OnTextBoxGotFocus;
        AssociatedObject.TextChanged -= OnTextChanged;
    }

    /// <summary>Обработчик события получения фокуса текстового поля.</summary>
    /// <param name="Sender">Источник события.</param>
    /// <param name="E">Аргументы события.</param>
    private static void OnTextBoxGotFocus(object Sender, RoutedEventArgs E)
    {
        // Если текстовое поле не содержит текста, то ничего не делаем
        if (Sender is not TextBox { Text.Length: > 0 } text_box) return;
        // Выделяем весь текст в текстовом поле
        text_box.SelectAll();
    }

    /// <summary>Обработчик события изменения текста в текстовом поле.</summary>
    /// <param name="Sender">Источник события.</param>
    /// <param name="E">Аргументы события.</param>
    private static void OnTextChanged(object? Sender, EventArgs E)
    {
        // Если текстовое поле не содержит текста, то ничего не делаем
        if (Sender is not TextBox { Text.Length: > 0 } text_box) return;
        // Отписываемся от события изменения текста, чтобы избежать рекурсии
        text_box.TextChanged -= OnTextChanged;
        // Выделяем весь текст в текстовом поле
        text_box.SelectAll();
    }
}