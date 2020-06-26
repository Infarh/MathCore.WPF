using System;

namespace MathCore.WPF.UIEvents
{
    public interface IModelEvent
    {
        event EventHandler<ModelEventArgs> Event;

        void Invoke(object? parameter = null);
    }
}