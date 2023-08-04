using System.Collections.ObjectModel;

namespace MathCore.WPF;

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