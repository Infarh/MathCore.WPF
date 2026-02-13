namespace MathCore.WPF.UIEvents;

public class ModelEventArgs(object? Model, object? Parameter) : EventArgs
{
    public object? Model { get; } = Model;

    public object? Parameter { get; } = Parameter;
}