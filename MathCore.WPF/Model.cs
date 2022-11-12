using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;

using MathCore.Annotations;

// ReSharper disable UnusedMember.Global

namespace MathCore.WPF;

/// <summary>Модель конструируемого динамического объекта для передачи в контекст данных представления</summary>
[ContentProperty(nameof(Properties))]
public class Model : Freezable, IAddChild
{
    /// <summary>Коллекция свойств объекта</summary>
    private ModelPropertiesCollection? _Properties;

    /// <summary>Динамический объект для доступа к свойствам</summary>
    private readonly ModelObject _ModelObject;

    /// <summary>Динамический объект для доступа к свойствам</summary>
    public ModelObject Object => _ModelObject;

    /// <summary>Коллекция свойств объекта</summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ModelPropertiesCollection Properties
    {
        get
        {
            VerifyAccess();
            if (_Properties != null) return _Properties;

            _Properties                 =  new ModelPropertiesCollection();
            _Properties.PropertyChanged += OnPropertiesChanged;
            return _Properties;
        }
    }

    /// <summary>Перехват событий свойств, возникающих при изменении их значений</summary>
    /// <param name="sender">Свойство - источник события</param>
    /// <param name="e">Аргумент события</param>
    private void OnPropertiesChanged(object sender, EventArgs e) => _ModelObject.PropertyChangeValue(((ModelProperty)sender).Name);

    /// <summary>Индексатор по имени свойства</summary>
    /// <param name="Name">Имя требуемого свойства</param>
    /// <returns>Свойство объекта с указанным именем</returns>
    public ModelProperty this[string Name]
    {
        get
        {
            var property = GetProperty(Name);
            if (property != null) return property;

            property = new ModelProperty(Name);
            Properties.Add(property);
            return property;
        }
    }

    /// <summary>Инициализация новой динамической модели объекта</summary>
    public Model() => _ModelObject = new ModelObject(this);

    /// <summary>Получить свойство объекта по указанному имени</summary>
    /// <param name="Name">Имя свойства объекта</param>
    /// <returns>Свойство объекта с указанным именем, либо пустота, если свойство отсутствует</returns>
    private ModelProperty? GetProperty(string Name) => _Properties?.FirstOrDefault(p => p.Name == Name);

    /// <summary>Попытаться установить значение свойства по указанному имени</summary>
    /// <param name="PropertyName">Имя устанавливаемого свойства</param>
    /// <param name="Value">Устанавливаемое значение свойства</param>
    /// <param name="CreateNewProperty">Создать новое свойство, если свойство с указанным именем не было найдено</param>
    /// <returns>Истина, если удалось установить значение свойства, ложь - если свойство отсутствует и не было создано</returns>
    public bool TrySetValue(string PropertyName, object Value, bool CreateNewProperty = false)
    {
        var property = GetProperty(PropertyName);
        if (property != null) property.Value = Value;
        else if (!CreateNewProperty) return false;
        else
        {
            property = new ModelProperty(PropertyName, Value);
            Properties.Add(property);
        }

        return true;
    }

    /// <summary>Попытаться получить значение свойства</summary>
    /// <param name="PropertyName">Имя свойства, значение которого требуется получить</param>
    /// <param name="Value">Получаемое значение свойства</param>
    /// <returns>Истина, если свойство было найдено и значение было получпено</returns>
    public bool TryGetValue(string PropertyName, out object Value)
    {
        var property = GetProperty(PropertyName);
        Value = property?.Value;
        return property != null;
    }

    /// <summary>Проверка - существует ли свойство в модели</summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    public bool ContainsProperty(string Name) => _Properties?.Contains(p => p.Name == Name) ?? false;

    /// <inheritdoc />
    public void AddChild(object value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));

        if (value is string name) value = new ModelProperty(name);
        if (value is ModelProperty property) _Properties.Add(property);
        else throw new NotSupportedException($"Тип объектов {value.GetType()} не поддерживается");
    }

    /// <summary>Передача события изменения свойства в динамический объект доступа</summary>
    /// <param name="Sender">Источник события - изменившееся свойство</param>
    /// <param name="E">Аргумент события и именем свойства в изменившемся объекте-свойстве модели</param>
    private void OnPropertyChanged(object Sender, PropertyChangedEventArgs E)
    {
        var property = (ModelProperty)Sender;
        _ModelObject.PropertyChangeValue(property.Name);
    }

    /// <inheritdoc />
    public void AddText(string text) => throw new NotSupportedException();

    /// <inheritdoc />
    protected override Freezable CreateInstanceCore() => new Model();
}

/// <summary>Колекция свойств модели</summary>
public sealed class ModelPropertiesCollection : Collection<ModelProperty>
{
    /// <summary>Событие возникает, когда одно из свойств коллекции меняет свой значение</summary>
    public event EventHandler? PropertyChanged;

    /// <summary>Генерация события изменения значения свойства коллекции</summary>
    /// <param name="sender">Источник события - одно из свойств коллекции </param>
    /// <param name="e">Аргумент события</param>
    private void OnPropertyChanged(object sender, EventArgs e) => PropertyChanged?.Invoke(sender, e);

    /// <summary>Словарь свойств по имени</summary>
    private readonly Dictionary<string, ModelProperty> _Properties = new();

    /// <summary>Определение свойства по имени</summary>
    /// <param name="Name">Имя требуемого свойства</param>
    /// <returns>Свойство с указанным именем</returns>
    public ModelProperty this[string Name] => _Properties[Name];

    /// <summary>Определение - содержится ли свойство с указанным именем в коллеции</summary>
    /// <param name="PropertyName">Имя проверяемого свойства</param>
    /// <returns>Истина, если свойство с указанным именем содержится в коллекции</returns>
    public bool Contains(string PropertyName) => _Properties.ContainsKey(PropertyName);

    /// <summary>Попытаться получить свойство из коллекции по указанному имени</summary>
    /// <param name="PropertyName">Имя свойства, которое требуется получить из коллекции</param>
    /// <param name="property">Свойство с указанным именем</param>
    /// <returns>Истина, если свойство было получено успешно</returns>
    public bool TryGetValue(string PropertyName, out ModelProperty property) => _Properties.TryGetValue(PropertyName, out property);

    /// <inheritdoc />
    protected override void ClearItems()
    {
        _Properties.Clear();
        base.ClearItems();
    }

    /// <inheritdoc />
    protected override void SetItem(int index, ModelProperty property)
    {
        var old_item                                                    = this[index];
        if (_Properties.Remove(old_item.Name)) old_item.PropertyChanged -= OnPropertyChanged;
        _Properties.Add(property.Name, property);
        base.SetItem(index, property);
        property.PropertyChanged += OnPropertyChanged;
    }

    /// <inheritdoc />
    protected override void RemoveItem(int index)
    {
        var item = this[index];
        _Properties.Remove(item.Name);
        base.RemoveItem(index);
    }

    /// <inheritdoc />
    protected override void InsertItem(int index, ModelProperty property)
    {
        _Properties.Add(property.Name, property);
        base.InsertItem(index, property);
        property.PropertyChanged += OnPropertyChanged;
    }
}

/// <summary>Свойство модели</summary>
public sealed class ModelProperty : Freezable, INotifyPropertyChanged
{
    /// <summary>Значение свойства</summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(object),
            typeof(ModelProperty),
            new PropertyMetadata(default(object), (d, _) => ((ModelProperty)d).OnPropertyChanged(nameof(Value))));

    /// <summary>Событие, возникает когда значение свойства модели меняется</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Генерация события изменения значения свойства</summary>
    /// <param name="PropertyName">Имя изменившегося свойства</param>
    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string PropertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

    /// <summary>Значение свойства</summary>
    public object Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>Имя свойства модели</summary>
    public string Name { get; set; }

    /// <summary>Инициализация нового свойства модели</summary>
    public ModelProperty() => Name = "noname";

    /// <summary>Инициализация нового свойства модели</summary>
    /// <param name="Name">Имя свойства модели</param>
    public ModelProperty(string Name) => this.Name = Name ?? throw new ArgumentNullException(nameof(Name));

    /// <summary>Инициализация нового свойства модели</summary>
    /// <param name="Name">Имя свойства модели</param>
    /// <param name="Value">Значение свойства модели</param>
    public ModelProperty(string Name, object? Value) : this(Name) => this.Value = Value;

    /// <inheritdoc />
    protected override Freezable CreateInstanceCore() => new ModelProperty(Name);
}

/// <summary>Динамический объект доступа к свойствам модели</summary>
public sealed class ModelObject : DynamicObject, INotifyPropertyChanged
{
    /// <summary>Событие возникает в момент изменения значения свойства модели</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Метод генерации события изменения свойства модели</summary>
    /// <param name="PropertyName"></param>
    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string PropertyName = null!) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

    /// <summary>Модель, доступ к свойствам которой осуществляет динамический объект</summary>
    private readonly Model _Model;

    /// <summary>Инициализация нового динамического объекта доступа к свойствам модели</summary>
    /// <param name="Model"></param>
    public ModelObject(Model Model) => _Model = Model ?? throw new ArgumentNullException(nameof(Model));

    /// <summary>Уведомление динамического объекта о том, что его свойство изменилось</summary>
    /// <param name="PropertyName">Имя изменившегося свойства</param>
    internal void PropertyChangeValue(string PropertyName) => OnPropertyChanged(PropertyName);

    /// <inheritdoc />
    public override bool TryGetMember(GetMemberBinder binder, out object result) =>
        _Model.TryGetValue(binder.Name, out result) || base.TryGetMember(binder, out result);

    /// <inheritdoc />
    public override bool TrySetMember(SetMemberBinder binder, object value) => _Model.TrySetValue(binder.Name, value) || base.TrySetMember(binder, value);
}