using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

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

            _Properties                 =  [];
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

            property = new(Name);
            Properties.Add(property);
            return property;
        }
    }

    /// <summary>Инициализация новой динамической модели объекта</summary>
    public Model() => _ModelObject = new(this);

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
            property = new(PropertyName, Value);
            Properties.Add(property);
        }

        return true;
    }

    /// <summary>Попытаться получить значение свойства</summary>
    /// <param name="PropertyName">Имя свойства, значение которого требуется получить</param>
    /// <param name="Value">Получаемое значение свойства</param>
    /// <returns>Истина, если свойство было найдено и значение было получено</returns>
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