using System;

namespace MathCore.WPF.UIEvents
{
    public partial class ModelEvent : IModelEvent
    {
        public event EventHandler<ModelEventArgs>? Event;

        protected virtual void OnEvent(ModelEventArgs e) => Event?.Invoke(this, e);

        protected virtual void OnEvent(object? Model, object? Parameter) => OnEvent(new ModelEventArgs(Model, Parameter));

        protected virtual void OnEvent(object? Parameter) => OnEvent(_Model, Parameter);

        private readonly object? _Model;

        public ModelEvent() { }

        public ModelEvent(object Model) => _Model = Model;

        public virtual void Invoke(object? Parameter = null) => OnEvent(Parameter);
    }
}