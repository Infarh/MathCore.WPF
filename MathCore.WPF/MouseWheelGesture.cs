using System.Windows.Input;

namespace MathCore.WPF;

/// <summary>Представляет жест колесом мыши.</summary>
public class MouseWheelGesture : InputGesture
{
    /// <summary>Определяет, соответствует ли указанный элемент ввода и событие ввода этому жесту.</summary>
    /// <param name="element">Элемент ввода.</param>
    /// <param name="e">Событие ввода.</param>
    /// <returns>true, если элемент ввода и событие ввода соответствуют этому жесту; в противном случае — false.</returns>
    public override bool Matches(object element, InputEventArgs e)
    {
        // Проверяем, является ли событие ввода событием колесом мыши и не равен ли delta нулю.
        return e is MouseWheelEventArgs wheel && wheel.Delta != 0;
    }
}