using System.Windows.Input;

namespace MathCore.WPF;

public class MouseWheelGesture : InputGesture
{
    /// <inheritdoc />
    public override bool Matches(object element, InputEventArgs e) => e is MouseWheelEventArgs wheel && wheel.Delta != 0;
}