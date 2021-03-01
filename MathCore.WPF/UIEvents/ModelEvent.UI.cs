using System;
using System.Collections.Generic;
using System.Windows;

namespace MathCore.WPF.UIEvents
{
    public partial class ModelEvent
    {
        #region Routed event - Invoked : EventHandler<ModelInvokedEventArgs> - Событие модели-представления

        /// <summary>Событие модели-представления</summary>
        public static readonly RoutedEvent InvokedEvent =
            EventManager.RegisterRoutedEvent(
                "Invoked",
                RoutingStrategy.Bubble,
                typeof(EventHandler<ModelInvokedEventArgs>),
                typeof(ModelEvent));

        /// <summary>Событие модели-представления</summary>
        public static void AddInvokedHandler(DependencyObject element, EventHandler<ModelInvokedEventArgs> handler) =>
            (element as UIElement)?.AddHandler(InvokedEvent, handler);

        /// <summary>Событие модели-представления</summary>
        public static void RemoveInvokedHandler(DependencyObject element, EventHandler<ModelInvokedEventArgs> handler) =>
            (element as UIElement)?.RemoveHandler(InvokedEvent, handler);

        #endregion

        #region Attached property Source : IModelEvent - Событие модели

        /// <summary>Событие модели</summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached(
                "Source",
                typeof(IModelEvent),
                typeof(ModelEvent),
                new PropertyMetadata(default(IModelEvent), OnSourceChanged));

        private class EventSourceHandler : IDisposable
        {
            private readonly WeakReference<UIElement> _Element;
            private readonly WeakReference<IModelEvent> _ModelEvent;

            public EventSourceHandler(UIElement element, IModelEvent ModelEvent)
            {
                _Element = new WeakReference<UIElement>(element);
                _ModelEvent = new WeakReference<IModelEvent>(ModelEvent);

                ModelEvent.Event += OnEventInvoked;
            }

            private void OnEventInvoked(object? Sender, ModelEventArgs E)
            {
                if(_Element.TryGetTarget(out var ui_element))
                    ui_element.RaiseEvent(new ModelInvokedEventArgs(ui_element, E));
            }

            public void Dispose()
            {
                if (_ModelEvent.TryGetTarget(out var model_event))
                    model_event.Event -= OnEventInvoked;
            }
        }

        private static readonly Dictionary<(UIElement element, IModelEvent e), EventSourceHandler> __Handlers = 
            new();

        private static void OnSourceChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
        {
            var ui_element = (UIElement)D;

            if (E.OldValue is IModelEvent old_event)
            {
                if (__Handlers.TryGetValue((ui_element, old_event), out var handler))
                    handler.Dispose();
            }

            if (E.NewValue is IModelEvent new_event)
            {
                __Handlers[(ui_element, new_event)] = new EventSourceHandler(ui_element, new_event);
            }
        }

        /// <summary>Событие модели</summary>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetSource(DependencyObject d, IModelEvent value) => d.SetValue(SourceProperty, value);

        /// <summary>Событие модели</summary>
        public static IModelEvent GetSource(DependencyObject d) => (IModelEvent)d.GetValue(SourceProperty);

        #endregion
    }
}