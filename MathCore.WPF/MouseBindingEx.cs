using System;
using System.Windows;
using System.Windows.Input;
using MathCore.WPF.Commands;

namespace MathCore.WPF
{
    public class MouseWheelBinding : InputBinding
    {
        /// <inheritdoc />
        public override InputGesture Gesture
        {
            get => base.Gesture;
            set
            {
                var old_gesture = (MouseWheelGesture)base.Gesture;
                var new_gesture = (MouseWheelGesture) value;
                if(Equals(old_gesture, value)) return;
                if(new_gesture == null) throw new ArgumentNullException(nameof(value));
                base.Gesture = value;
            }
        }

        public MouseWheelBinding() { }

        public MouseWheelBinding(ICommand command) :base(command, new MouseWheelGesture()) { }

        protected override Freezable CreateInstanceCore() => new MouseWheelBinding();
    }

    public class MouseWheelGesture : InputGesture
    {
        /// <inheritdoc />
        public override bool Matches(object element, InputEventArgs e) => e is MouseWheelEventArgs wheel && wheel.Delta != 0;
    }

    public class TestCommand : LambdaCommand
    {
        public TestCommand() : base(p => MessageBox.Show(p?.ToString() ?? "null", "TestCommand")) { }
    }
}
