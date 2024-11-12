using System.Windows;

using Microsoft.Xaml.Behaviors;
// ReSharper disable MemberCanBeProtected.Global

namespace MathCore.WPF.Behaviors;

/// <summary>
/// Поведение, которое можно присоединить к UIElement.
/// Это поведение предоставляет ссылку на окно, в котором находится UIElement.
/// </summary>
public abstract class WindowBehavior : Behavior<UIElement>
{
    private WeakReference<Window>? _WindowReference;

    /// <summary>
    /// Возвращает окно, связанное с UIElement.
    /// Возвращает null, если UIElement не является частью окна.
    /// </summary>
    public Window? AssociatedWindow => _WindowReference is null
        ? null
        : _WindowReference.TryGetTarget(out var window)
            ? window
            : null;

    /// <summary>
    /// Вызывается, когда поведение присоединяется к UIElement.
    /// </summary>
    protected override void OnAttached()
    {
        // Найти окно, в котором находится UIElement.
        // Сначала проверяем, является ли UIElement окном.
        // Если нет, проверяем, является ли UIElement дочерним элементом окна.
        // Если нет, проверяем, является ли UIElement логическим дочерним элементом окна.
        var window = AssociatedObject as Window
            ?? AssociatedObject.FindVisualParent<Window>()
            ?? AssociatedObject.FindLogicalParent<Window>();

        // Если окно найдено, создаем слабую ссылку на него.
        // Это позволяет окну быть собранным мусором, если оно больше не используется.
        _WindowReference = window is null ? null : new WeakReference<Window>(window);
    }
}