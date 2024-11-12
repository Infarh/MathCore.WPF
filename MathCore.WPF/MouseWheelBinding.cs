using System.Windows;
using System.Windows.Input;

namespace MathCore.WPF;

/// <summary>Представляет связь между жестом колеса мыши и командой.</summary>
public class MouseWheelBinding : InputBinding
{
    /// <summary>Возвращает или задает жест, связанный с этим связыванием.</summary>
    /// <value>Жест.</value>
    /// <exception cref="ArgumentNullException">Выбрасывается, когда значение равно null.</exception>
    public override InputGesture Gesture
    {
        get => base.Gesture;
        set
        {
            // Проверяем, отличается ли новый жест от старого
            var old_gesture = (MouseWheelGesture)base.Gesture;
            var new_gesture = (MouseWheelGesture)value;
            if (Equals(old_gesture, value)) return;

            // Проверяем на null значение
            if (new_gesture is null) throw new ArgumentNullException(nameof(value));

            // Обновляем базовый жест
            base.Gesture = value;
        }
    }

    /// <summary>Инициализирует новый экземпляр класса <see cref="MouseWheelBinding"/>.</summary>
    public MouseWheelBinding() { }

    /// <summary>Инициализирует новый экземпляр класса <see cref="MouseWheelBinding"/> с указанной командой.</summary>
    /// <param name="command">Команда, которую необходимо связать с этим связыванием.</param>
    public MouseWheelBinding(ICommand command) : base(command, new MouseWheelGesture()) { }

    /// <summary>Создает новый экземпляр класса <see cref="MouseWheelBinding"/>.</summary>
    /// <returns>Новый экземпляр класса <see cref="MouseWheelBinding"/>.</returns>
    protected override Freezable CreateInstanceCore() => new MouseWheelBinding();
}