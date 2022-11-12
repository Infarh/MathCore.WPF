using System.Windows;

namespace MathCore.WPF.UIEvents;

public class ModelInvokedEventArgs : RoutedEventArgs
{
    private readonly ModelEventArgs _EventInfo;

    public object? Model => _EventInfo.Model;

    public object? Parameter => _EventInfo.Parameter;

    public ModelInvokedEventArgs(object Source, ModelEventArgs EventInfo)
    {
        RoutedEvent = ModelEvent.InvokedEvent;
        this.Source = Source;

        _EventInfo = EventInfo;
    }
}