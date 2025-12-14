using System.Collections;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;

// ReSharper: отключить предупреждение UnusedMember.Global

namespace MathCore.WPF;

/// <summary>Утилитный менеджер для привязки коллекции контроллеров к элементам WPF</summary>
public static class ElementManager
{
    private static readonly DependencyProperty ControllersProperty =
        DependencyProperty.RegisterAttached(
            "ShadowControllers",
            typeof(ElementControllersCollection),
            typeof(ElementManager),
            new FrameworkPropertyMetadata(OnControllersChanged));

    /// <summary>Получить или создать коллекцию контроллеров, ассоциированных с объектом</summary>
    /// <param name="obj">Целевой объект WPF</param>
    /// <returns>Коллекция контроллеров, ассоциированная с объектом</returns>
    public static ElementControllersCollection GetBehaviors(DependencyObject obj)
    {
        var collection = (ElementControllersCollection?)obj.GetValue(ControllersProperty);
        if (collection != null) return collection;
        collection = [];
        obj.SetValue(ControllersProperty, collection);
        return collection;
    }

    private static void OnControllersChanged(DependencyObject? obj, DependencyPropertyChangedEventArgs args)
    {
        var old_value = (ElementControllersCollection?)args.OldValue;
        var new_value = (ElementControllersCollection?)args.NewValue;
        if (old_value == new_value) return;
        if (old_value?.Element != null) old_value.ResetElement();
        if (new_value is null || obj is null) return;
        if (new_value.Element != null) throw new InvalidOperationException();
        new_value.SetElement(obj);
    }

}

/// <summary>Базовый абстрактный контроллер элемента на основе Animatable</summary>
public abstract class ElementController : Animatable
{
    /// <summary>Привязать контроллер к элементу</summary>
    /// <param name="element">Целевой объект WPF</param>
    public abstract void SetElement(DependencyObject element);

    /// <summary>Сбросить привязку контроллера от элемента</summary>
    public abstract void ResetElement();
}

/// <summary>Обобщённый контроллер элемента с типом целевого элемента</summary>
/// <typeparam name="TElement">Тип целевого элемента наследуемый от DependencyObject</typeparam>
public abstract class ElementController<TElement> : ElementController
    where TElement : DependencyObject
{
    /// <summary>Событие возникает при установке элемента для контроллера</summary>
    public event EventHandler<ElementController<TElement>, TElement>? ElementSet;

    /// <summary>Событие возникает при сбросе элемента у контроллера</summary>
    public event EventHandler<ElementController<TElement>, TElement>? ElementReset;

    private TElement? _Element;

    /// <summary>Текущий привязанный элемент контроллера</summary>
    public TElement? Element => _Element;

    /// <inheritdoc />
    public override void SetElement(DependencyObject? element)
    {
        if (element is null)
        {
            ResetElement();
            return;
        }

        if (element is not TElement e)
            throw new ArgumentException($"Целевой объект не является объектом типа {typeof(TElement)}");
        SetElement(e);
    }

    /// <summary>Виртуальный метод для установки элемента конкретного типа</summary>
    /// <param name="element">Элемент типа TElement</param>
    protected virtual void SetElement(TElement element)
    {
        if (ReferenceEquals(_Element, element)) return;
        if (element is null) throw new ArgumentNullException(nameof(element));
        ResetElement();
        ElementSet?.Invoke(this, _Element = element);
    }

    /// <summary>Сбросить привязанный элемент и вызвать соответствующее событие</summary>
    public override void ResetElement()
    {
        if (_Element != null)
            ElementReset?.Invoke(this, _Element);
        _Element = null;
    }
}

/// <summary>Коллекция контроллеров элемента с поддержкой IList интерфейса</summary>
public class ElementControllersCollection : IList<ElementController>
{
    private readonly List<ElementController> _Items = [];
    private DependencyObject _Element;

    /// <summary>Элемент, к которому привязана коллекция контроллеров</summary>
    public DependencyObject Element
    {
        get => _Element;
        set
        {
            if (ReferenceEquals(_Element, value)) return;
            _Element = value ?? throw new ArgumentNullException(nameof(value));
            for (var i = 0; i < _Items.Count; i++)
                _Items[i].SetElement(value);
        }
    }

    /// <summary>Количество контроллеров в коллекции</summary>
    public int Count => _Items.Count;

    /// <summary>Добавить контроллер в коллекцию и установить ему текущий элемент</summary>
    /// <param name="controller">Добавляемый контроллер</param>
    public void Add(ElementController controller)
    {
        controller.SetElement(_Element);
        _Items.Add(controller);
    }

    /// <summary>Удалить контроллер из коллекции и сбросить его элемент</summary>
    /// <param name="controller">Удаляемый контроллер</param>
    /// <returns>True если удаление выполнено</returns>
    public bool Remove(ElementController? controller)
    {
        var remove = _Items.Remove(controller);
        if (remove) controller.ResetElement();
        return remove;
    }

    /// <summary>Установить элемент для всех контроллеров коллекции</summary>
    /// <param name="element">Элемент, который нужно установить</param>
    public void SetElement(DependencyObject element) => _Items.Foreach(element, (c, e) => c.SetElement(e));

    /// <summary>Сбросить элемент у всех контроллеров коллекции</summary>
    public void ResetElement() => _Items.ForEach(c => c.ResetElement());


    /// <summary>Очистить коллекцию контроллеров и сбросить их элементы</summary>
    public void Clear()
    {
        ResetElement();
        _Items.Clear();
    }

    #region IList

    /// <summary>Признак доступности коллекции только для чтения</summary>
    bool ICollection<ElementController>.IsReadOnly => false;

    /// <summary>Индексатор доступа к элементам коллекции</summary>
    ElementController IList<ElementController>.this[int index] { get => _Items[index]; set => _Items[index] = value; }

    /// <summary>Проверить наличие контроллера в коллекции</summary>
    bool ICollection<ElementController>.Contains(ElementController? controller) => _Items.Contains(controller);

    /// <summary>Скопировать элементы коллекции в массив</summary>
    void ICollection<ElementController>.CopyTo(ElementController[] array, int arrayIndex) => _Items.CopyTo(array, arrayIndex);

    /// <summary>Получить индекс контроллера в коллекции</summary>
    int IList<ElementController>.IndexOf(ElementController controller) => _Items.IndexOf(controller);

    /// <summary>Вставить контроллер по индексу и установить ему текущий элемент</summary>
    void IList<ElementController>.Insert(int index, ElementController controller)
    {
        controller.SetElement(_Element);
        _Items.Insert(index, controller);
    }

    /// <summary>Удалить контроллер по индексу и сбросить его элемент</summary>
    void IList<ElementController>.RemoveAt(int index)
    {
        _Items[index].ResetElement();
        _Items.RemoveAt(index);
    }

    /// <summary>Получить перечислитель по коллекции контроллеров</summary>
    IEnumerator<ElementController> IEnumerable<ElementController>.GetEnumerator() => _Items.GetEnumerator();

    /// <summary>Получить неуниверсальный перечислитель по коллекции</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Items).GetEnumerator();

    #endregion
}

/// <summary>Триггер событий с дополнительной проверкой источника и поддержкой действий</summary>
[ContentProperty("Actions")]
public class ConditionalEventTrigger : FrameworkContentElement
{
    private static readonly RoutedEvent TriggerActionsEvent = EventManager
       .RegisterRoutedEvent(
            "TriggerActions",
            RoutingStrategy.Direct,
            typeof(EventHandler),
            typeof(ConditionalEventTrigger));

    /// <summary>RoutedEvent, на которое ссылается триггер</summary>
    public RoutedEvent RoutedEvent { get; set; }

    /// <summary>DependencyProperty для списка имён исключённых источников</summary>
    public static readonly DependencyProperty ExcludedSourceNamesProperty = DependencyProperty
       .Register(
            nameof(ExcludedSourceNames),
            typeof(List<string>),
            typeof(ConditionalEventTrigger),
            new(new List<string>()));

    /// <summary>Список имён источников, для которых действия не будут выполняться</summary>
    public List<string> ExcludedSourceNames
    {
        get => (List<string>)GetValue(ExcludedSourceNamesProperty);
        set => SetValue(ExcludedSourceNamesProperty, value);
    }

    /// <summary>DependencyProperty для списка действий триггера</summary>
    public static readonly DependencyProperty ActionsProperty = DependencyProperty
       .Register(
            nameof(Actions),
            typeof(List<TriggerAction>),
            typeof(ConditionalEventTrigger),
            new(new List<TriggerAction>()));

    /// <summary>Список действий, выполняемых при срабатывании триггера</summary>
    public List<TriggerAction> Actions
    {
        get => (List<TriggerAction>)GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    // Прикреплённое свойство "Triggers"
    /// <summary>Получить коллекцию триггеров, привязанную к объекту</summary>
    /// <param name="obj">Целевой объект</param>
    /// <returns>Коллекция условных триггеров</returns>
    public static ConditionalEventTriggerCollection GetTriggers(DependencyObject obj) => (ConditionalEventTriggerCollection)obj.GetValue(TriggersProperty);

    /// <summary>Установить коллекцию триггеров для объекта</summary>
    /// <param name="obj">Целевой объект</param>
    /// <param name="value">Коллекция триггеров</param>
    public static void SetTriggers(DependencyObject obj, ConditionalEventTriggerCollection value) => obj.SetValue(TriggersProperty, value);

    /// <summary>Прикреплённое свойство коллекции условных триггеров</summary>
    public static readonly DependencyProperty TriggersProperty = DependencyProperty
       .RegisterAttached(
            "Triggers",
            typeof(ConditionalEventTriggerCollection),
            typeof(ConditionalEventTrigger),
            new()
            {
                PropertyChangedCallback = (s, e) =>
                {
                    // При установке свойства "Triggers" зарегистрировать обработчики для каждого триггера в списке
                    var element = (FrameworkElement)s;
                    foreach (var trigger in (List<ConditionalEventTrigger>)e.NewValue)
                        element.AddHandler(trigger.RoutedEvent, new RoutedEventHandler((_, e2) => trigger.OnRoutedEvent(element, e2)));
                }
            });

    // Когда происходит событие, проверить условие и при выполнении запустить действия
    private void OnRoutedEvent(FrameworkElement element, RoutedEventArgs args)
    {
        if (args.OriginalSource is not FrameworkElement sender) return;
        DataContext = element.DataContext; // Разрешить привязке данных доступ к свойствам элемента
        if (ExcludedSourceNames.Any(x => x.Equals(sender.Name))) return;
        // Построить EventTrigger, содержащий действия, и затем вызвать его
        var trigger = new EventTrigger { RoutedEvent = TriggerActionsEvent };
        foreach (var action in Actions)
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

/// <summary>Коллекция условных триггеров для прикреплённого свойства</summary>
public class ConditionalEventTriggerCollection : List<ConditionalEventTrigger>;