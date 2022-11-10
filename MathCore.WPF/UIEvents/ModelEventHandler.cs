namespace MathCore.WPF.UIEvents;

public class ModelEventHandler
{
    //#region Event : IModelEvent - Событие модели

    ///// <summary>Событие модели</summary>
    //public static readonly DependencyProperty EventProperty =
    //    DependencyProperty.Register(
    //        nameof(Event),
    //        typeof(IModelEvent),
    //        typeof(ModelEventHandler),
    //        new PropertyMetadata(default(IModelEvent), OnEventChanged));

    //private static void OnEventChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    //{
    //    var handler = (ModelEventHandler)D;
  
    //    if (E.OldValue is IModelEvent old_event)
    //        old_event.Event -= handler.OnEvent;

    //    if(E.NewValue is IModelEvent new_event)
    //        new_event.Event += handler.OnEvent;
    //}

    //private void OnEvent(object? Sender, ModelEventArgs E)
    //{
            
    //}

    ///// <summary>Событие модели</summary>
    ////[Category("")]
    //[Description("Событие модели")]
    //public IModelEvent Event { get => (IModelEvent)GetValue(EventProperty); set => SetValue(EventProperty, value); }

    //#endregion
}