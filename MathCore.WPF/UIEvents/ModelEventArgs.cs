using System;

namespace MathCore.WPF.UIEvents
{
    public class ModelEventArgs : EventArgs
    {
        public object? Model { get; }

        public object? Parameter { get; }

        public ModelEventArgs(object? Model, object? Parameter)
        {
            this.Model = Model;
            this.Parameter = Parameter;
        }
    }
}
