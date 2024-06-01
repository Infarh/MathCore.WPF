using System.Collections;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;

// ReSharper disable UnusedMember.Global

namespace MathCore.WPF;

public static class ElementManager
{
    private static readonly DependencyProperty ControllersProperty =
        DependencyProperty.RegisterAttached(
            "ShadowControllers",
            typeof(ElementControllersCollection),
            typeof(ElementManager),
            new FrameworkPropertyMetadata(OnControllersChanged));

    public static ElementControllersCollection GetBehaviors(DependencyObject obj)
    {
        var collection = (ElementControllersCollection?)obj.GetValue(ControllersProperty);
        if(collection != null) return collection;
        collection = [];
        obj.SetValue(ControllersProperty, collection);
        return collection;
    }

    private static void OnControllersChanged(DependencyObject? obj, DependencyPropertyChangedEventArgs args)
    {
        var old_value = (ElementControllersCollection?)args.OldValue;
        var new_value = (ElementControllersCollection?)args.NewValue;
        if(old_value == new_value) return;
        if(old_value?.Element != null) old_value.ResetElement();
        if(new_value is null || obj is null) return;
        if(new_value.Element != null) throw new InvalidOperationException();
        new_value.SetElement(obj);
    }

}

public abstract class ElementController : Animatable
{
    public abstract void SetElement(DependencyObject element);
    public abstract void ResetElement();
}

public abstract class ElementController<TElement> : ElementController
    where TElement : DependencyObject
{
    public event EventHandler<ElementController<TElement>, TElement>? ElementSet;
    public event EventHandler<ElementController<TElement>, TElement>? ElementReset;

    private TElement? _Element;

    public TElement? Element => _Element;

    /// <inheritdoc />
    public override void SetElement(DependencyObject? element)
    {
        if(element is null)
        {
            ResetElement();
            return;
        }

        if(element is not TElement e)
            throw new ArgumentException($"Целевой объект не является объектом типа {typeof(TElement)}");
        SetElement(e);
    }

    protected virtual void SetElement(TElement element)
    {
        if(ReferenceEquals(_Element, element)) return;
        if(element is null) throw new ArgumentNullException(nameof(element));
        ResetElement();
        ElementSet?.Invoke(this, _Element = element);
    }

    public override void ResetElement()
    {
        if(_Element != null)
            ElementReset?.Invoke(this, _Element);
        _Element = null;
    }
}

public class ElementControllersCollection : IList<ElementController>
{
    private readonly List<ElementController> _Items = [];
    private DependencyObject _Element;

    public DependencyObject Element
    {
        get => _Element;
        set
        {
            if(ReferenceEquals(_Element, value)) return;
            _Element = value ?? throw new ArgumentNullException(nameof(value));
            for(var i = 0; i < _Items.Count; i++)
                _Items[i].SetElement(value);
        }
    }

    /// <inheritdoc />
    public int Count => _Items.Count;

    /// <inheritdoc />
    public void Add(ElementController controller)
    {
        controller.SetElement(_Element);
        _Items.Add(controller);
    }

    /// <inheritdoc />
    public bool Remove(ElementController? controller)
    {
        var remove = _Items.Remove(controller);
        if(remove) controller.ResetElement();
        return remove;
    }

    public void SetElement(DependencyObject element) => _Items.Foreach(element, (c, e) => c.SetElement(e));

    public void ResetElement() => _Items.ForEach(c => c.ResetElement());


    /// <inheritdoc />
    public void Clear()
    {
        ResetElement();
        _Items.Clear();
    }

    #region IList

    /// <inheritdoc />
    bool ICollection<ElementController>.IsReadOnly => false;

    /// <inheritdoc />
    ElementController IList<ElementController>.this[int index] { get => _Items[index]; set => _Items[index] = value; }

    /// <inheritdoc />
    bool ICollection<ElementController>.Contains(ElementController? controller) => _Items.Contains(controller);

    /// <inheritdoc />
    void ICollection<ElementController>.CopyTo(ElementController[] array, int arrayIndex) => _Items.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    int IList<ElementController>.IndexOf(ElementController controller) => _Items.IndexOf(controller);

    /// <inheritdoc />
    void IList<ElementController>.Insert(int index, ElementController controller)
    {
        controller.SetElement(_Element);
        _Items.Insert(index, controller);
    }

    /// <inheritdoc />
    void IList<ElementController>.RemoveAt(int index)
    {
        _Items[index].ResetElement();
        _Items.RemoveAt(index);
    }

    /// <inheritdoc />
    IEnumerator<ElementController> IEnumerable<ElementController>.GetEnumerator() => _Items.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Items).GetEnumerator();

    #endregion
}

[ContentProperty("Actions")]
public class ConditionalEventTrigger : FrameworkContentElement
{
    private static readonly RoutedEvent TriggerActionsEvent = EventManager
       .RegisterRoutedEvent(
            "TriggerActions", 
            RoutingStrategy.Direct, 
            typeof(EventHandler), 
            typeof(ConditionalEventTrigger));
    public RoutedEvent RoutedEvent { get; set; }

    public static readonly DependencyProperty ExcludedSourceNamesProperty = DependencyProperty
       .Register(
            nameof(ExcludedSourceNames), 
            typeof(List<string>), 
            typeof(ConditionalEventTrigger), 
            new(new List<string>()));

    public List<string> ExcludedSourceNames
    {
        get => (List<string>)GetValue(ExcludedSourceNamesProperty);
        set => SetValue(ExcludedSourceNamesProperty, value);
    }

    public static readonly DependencyProperty ActionsProperty = DependencyProperty
       .Register(
            nameof(Actions),
            typeof(List<TriggerAction>),
            typeof(ConditionalEventTrigger),
            new(new List<TriggerAction>()));

    public List<TriggerAction> Actions
    {
        get => (List<TriggerAction>)GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    // "Triggers" attached property
    public static ConditionalEventTriggerCollection GetTriggers(DependencyObject obj) => (ConditionalEventTriggerCollection)obj.GetValue(TriggersProperty);
    public static void SetTriggers(DependencyObject obj, ConditionalEventTriggerCollection value) => obj.SetValue(TriggersProperty, value);

    public static readonly DependencyProperty TriggersProperty = DependencyProperty
       .RegisterAttached(
            "Triggers",
            typeof(ConditionalEventTriggerCollection),
            typeof(ConditionalEventTrigger),
            new()
            {
                PropertyChangedCallback = (s, e) =>
                {
                    // When "Triggers" is set, register handlers for each trigger in the list 
                    var element = (FrameworkElement)s;
                    foreach(var trigger in (List<ConditionalEventTrigger>)e.NewValue)
                        element.AddHandler(trigger.RoutedEvent, new RoutedEventHandler((_, e2) => trigger.OnRoutedEvent(element, e2)));
                }
            });

    // When an event fires, check the condition and if it is true fire the actions 
    private void OnRoutedEvent(FrameworkElement element, RoutedEventArgs args)
    {
        if(args.OriginalSource is not FrameworkElement sender) return;
        DataContext = element.DataContext; // Allow data binding to access element properties
        if(ExcludedSourceNames.Any(x => x.Equals(sender.Name))) return;
        // Construct an EventTrigger containing the actions, then trigger it 
        var trigger = new EventTrigger { RoutedEvent = TriggerActionsEvent };
        foreach(var action in Actions)
            trigger.Actions.Add(action);

        element.Triggers.Add(trigger);
        try
        {
            element.RaiseEvent(new(TriggerActionsEvent));
        }
        finally
        {
            element.Triggers.Remove(trigger);
        }
    }
}

public class ConditionalEventTriggerCollection : List<ConditionalEventTrigger>;