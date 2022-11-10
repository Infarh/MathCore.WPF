using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;

using MathCore.WPF.Commands;
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.ViewModels;

/// <summary>Генератор модели-представления интерфейса настроек</summary>
/// <remarks>
/// В первый вложенный в окно контейнер надо добавить преобразователь контекста данных
/// DataContext="{Binding Converter={u:SettingsDialogViewModelConverter}}
/// </remarks>
[MarkupExtensionReturnType(typeof(SettingsDialogViewModelConverter))]
[ValueConversion(typeof(object), typeof(SettingsDialogViewModel))]
public sealed class SettingsDialogViewModelConverter : MarkupExtension, IValueConverter
{
    /// <summary>Диалоговое окно конфигурации</summary>
    private Window? _DialogWindow;

    /// <summary>Метод генерации значения, вставляемого в разметку</summary>
    /// <returns>Возвращает сам себя</returns>
    public override object ProvideValue(IServiceProvider Service)
    {
        if (ViewModel.IsDesignMode) return this;
        var service = (IRootObjectProvider?)Service.GetService(typeof(IRootObjectProvider));
        _DialogWindow = service?.RootObject as Window;
        return _DialogWindow is null
            ? throw new InvalidOperationException("В логическом дереве визуальных компонентов не найден корень в виде объекта класса Window - самого диалогового окна")
            : this;
    }

    /// <inheritdoc />
    public object? Convert(object v, Type t, object p, CultureInfo c) => _DialogWindow is { } window ? new SettingsDialogViewModel(v, window) : null;

    /// <inheritdoc />
    public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotSupportedException();
}

/// <summary>Модель-представление диалогового окна настроек</summary>
public class SettingsDialogViewModel : ViewModel
{
    /// <summary>Флаги привязки для поиска членов объекта конфигурации</summary>
    internal const BindingFlags PropertiesBindingTypes = BindingFlags.Instance | BindingFlags.Public;

    /// <summary>Объект конфигурации, с которым работает окно диалога (контекст данных окна диалога)</summary>
    private object? _ValueObject;

    /// <summary>Перечень известных свойств объекта конфигурации, с которыми можно работать на чтение и на запись значений</summary>
    private PropertyInfo[]? _KnownProperties;

    /// <summary>Окно диалога конфигурации</summary>
    private Window? _DialogWindow;

    /// <summary>Текущий словарь значений параметров, которые устанавливаются в окне конфигурации и которые будут применены к объекту после выполнения команды <see cref="CommitCommand"/></summary>
    private readonly Dictionary<string, object?> _PropertiesDictionary = new();

    /// <summary>Текущий словарь значений параметров, которые устанавливаются в окне конфигурации и которые будут применены к объекту после выполнения команды <see cref="CommitCommand"/></summary>
    protected Dictionary<string, object?> PropertiesDictionary => _PropertiesDictionary;

    /// <summary>Окно диалога конфигурации</summary>
    protected Window? DialogWindow => _DialogWindow;

    /// <summary>Динамический объект конфигурации</summary>
    /// <remarks>
    /// Служит для динамической привязки значений в XAML-разметке к виртуальным свойствам объекта конфигурации.
    /// При первом доступе к свойству (при чтении значения) выдаёт значение исходного объекта конфигурации
    /// При записи значения в свойство, сохраняет переданное значение в словаре и при последующий чтениях выдаёт записанное в словарь значение
    /// При закрытии окна с положительным диалоговым результатом копирует значения словаря в свойства исходного конфигурируемого объекта
    /// </remarks> 
    public SettingsObjectManager? Value { get; private set; }

    /// <summary>Объект конфигурации, с которым работает окно диалога (контекст данных окна диалога)</summary>
    public object? ValueObject => _ValueObject;

    /// <summary>Признак того, что значения динамического конфигурируемого объекта и исходного объекта расходятся (имеются записи в словаре значений)</summary>
    public bool HasChanges => _PropertiesDictionary.Count > 0;

    /// <summary>Перечень известных свойств объекта конфигурации, с которыми можно работать на чтение и на запись значений</summary>
    public PropertyInfo[] KnownProperties => _KnownProperties ??= _ValueObject?.GetType().GetProperties(PropertiesBindingTypes) ?? Array.Empty<PropertyInfo>();

    /// <summary>Команда сохранения значений конфигурации и закрытия окна диалога с положительным диалоговым результатом</summary>
    public LambdaCommand<bool?> CommitCommand { get; }
    /// <summary>Команд отклонения внесённых изменений и закрытия диалогового окна с отрицательным диалоговым результатом</summary>
    public LambdaCommand<bool?> RejectCommand { get; }
    /// <summary>Команда отмены деланных изменений (очистка словаря значений свойств конфигурируемого объекта)</summary>
    public LambdaCommand RestoreCommand { get; }
    /// <summary>Команда закрытия диалогового окна с указанием диалогового результата</summary>
    public LambdaCommand<bool?> CloseCommand { get; }

    /// <summary>Инициализация новой модели-представления окна диалога конфигурации</summary>
    /// <param name="value">Объект конфигурации</param>
    /// <param name="window">Окно диалога конфигурации</param>
    public SettingsDialogViewModel(object value, Window window) : this() => Initialize(value, window);

    /// <summary>Инициализация новой модели-представления окна диалога конфигурации</summary>
    /// <remarks>Создаёт все команды, но не производит инициализацию модели устанавливая конфигурируемый объект и окно диалога</remarks>
    protected SettingsDialogViewModel()
    {
        CommitCommand  = new LambdaCommand<bool?>(OnCommitCommandExecuted, _ => _ValueObject != null);
        RejectCommand  = new LambdaCommand<bool?>(OnRejectCommandExecuted, _ => _ValueObject != null);
        RestoreCommand = new LambdaCommand(OnRestoreCommandExecuted, _ => _ValueObject != null && HasChanges);
        CloseCommand   = new LambdaCommand<bool?>(OnCloseCommandExecuted, _ => _ValueObject != null);
    }

    /// <summary>Инициализация модели-представления диалога конфигурации</summary>
    /// <param name="value">Объект конфигурации</param>
    /// <param name="window">Окно конфигурации</param>
    protected virtual void Initialize(object value, Window window)
    {
        if (IsDesignMode || Equals(_DialogWindow, window) && Equals(_ValueObject, value)) return;
        if (window is null) throw new ArgumentNullException(nameof(window));
        if (Value != null) Value.PropertyChanged -= OnValuePropertyChanged;
        Value                 =  new SettingsObjectManager(_ValueObject = value, _DialogWindow = window, _PropertiesDictionary);
        Value.PropertyChanged += OnValuePropertyChanged;
        OnPropertyChanged(nameof(Value));
        OnPropertyChanged(nameof(DialogWindow));
    }

    /// <summary>Обработчик события изменения значения свойства динамического объекта конфигурации</summary>
    /// <param name="Sender">Источник события - динамический объект конфигурации</param>
    /// <param name="E">Аргумент события, определяющий имя изменившегося свойства</param>
    protected virtual void OnValuePropertyChanged(object? Sender, PropertyChangedEventArgs? E) { }

    /// <summary>Обработчик вызова команды применения изменений в динамическом-конфигурационном объекте и закрытия диалогового окна с положительным диалоговым результатом</summary>
    /// <param name="DialogResult">Установленный диалоговый результат (по умолчанию - <see langword="true"/>)</param>
    protected virtual void OnCommitCommandExecuted(bool? DialogResult = true) => CloseDialogWindow(DialogResult ?? true);
    /// <summary>Обработчик вызова команды отклонения сделанных в динамическом-конфигурационном объекте изменений и закрытия диалогового окна с отрицательным диалоговым результатом</summary>
    /// <param name="DialogResult">Установленный диалоговый результат (по умолчанию - <see langword="false"/>)</param>
    protected virtual void OnRejectCommandExecuted(bool? DialogResult = false) => CloseDialogWindow(DialogResult ?? false);
    /// <summary>Обработчик вызова команды отклонения изменений, сделанных в динамическом объекте конфигурации без закрытия диалогового окна</summary>
    protected virtual void OnRestoreCommandExecuted()
    {
        Value!.Restore();
        OnPropertyChanged(nameof(HasChanges));
    }
    /// <summary>Обработчик вызова команды закрытия диалогового окна с нейтральным диалоговым результатом</summary>
    /// <param name="DialogResult">Установленный диалоговый результат (по умолчанию - <see langword="null"/>)</param>
    protected virtual void OnCloseCommandExecuted(bool? DialogResult = null) => CloseDialogWindow(DialogResult);

    /// <summary>Метод закрытия диалогового кона с установленным диалоговым результатом</summary>
    /// <param name="DialogResult">Установленный диалоговый результат (по умолчанию - <see langword="null"/>)</param>
    protected virtual void CloseDialogWindow(bool? DialogResult = null)
    {
        var dialog_window = _DialogWindow ?? throw new InvalidOperationException("Не определено окно диалога");
        dialog_window.DialogResult = DialogResult;
        dialog_window.Close();
    }
}

/// <summary>Динамический объект конфигурации, осуществляющий доступ к виртуальным свойствам исходного конфигурируемого объекта</summary>
public class SettingsObjectManager : DynamicViewModel
{
    /// <summary>Исходный конфигурируемый объект</summary>
    private readonly object? _Value;

    /// <summary>Словарь дескрипторов открытых свойств исходного объекта, доступных для чтения и для записи значений</summary>
    private readonly Dictionary<string, PropertyInfo> _ObjectProperties;
    /// <summary>Словарь дескрипторов открытых полей исходного объекта, доступных для чтения и для записи значений</summary>
    private readonly Dictionary<string, FieldInfo> _ObjectFields;
    /// <summary>Словарь дескрипторов открытых индексаторов исходного объекта, доступных для чтения и для записи значений</summary>
    private readonly PropertyInfo[] _ObjectIndexers;

    /// <summary>Создание нового динамического конфигурируемого объекта</summary>
    /// <param name="value">Исходный конфигурируемый объект</param>
    /// <param name="window">Окно диалога конфигурации</param>
    /// <param name="PropertiesDictionary">Словарь, в котором следует хранить временные значения устанавливаемых свойств конфигурируемого объекта</param>
    internal SettingsObjectManager(object value, Window window, Dictionary<string, object?> PropertiesDictionary)
        : base(PropertiesDictionary)
    {
        if (window is null) throw new ArgumentNullException(nameof(window));
        _Value = value;
        if (value is INotifyPropertyChanged notify_property_changed) 
            notify_property_changed.PropertyChanged += OnValuePropertyChanged;

        var type       = value.GetType();
        var properties = type.GetProperties(SettingsDialogViewModel.PropertiesBindingTypes);
        _ObjectProperties =  properties.Where(p => p.GetIndexParameters().Length == 0).ToDictionary(p => p.Name);
        _ObjectFields     =  type.GetFields(SettingsDialogViewModel.PropertiesBindingTypes).ToDictionary(p => p.Name);
        _ObjectIndexers   =  properties.Where(p => p.GetIndexParameters().Length > 0).ToArray();
        window.Closed     += OnDialogWindowClosed;
    }

    /// <summary>Обработчик события изменений свойств конфигурируемого объекта в случае если он определяет интерфейс <see cref="INotifyPropertyChanged"/></summary>
    /// <param name="Sender">Исходный конфигурируемый объект - источник события</param>
    /// <param name="E">Аргумент события, определяющий имя изменившегося свойства</param>
    private void OnValuePropertyChanged(object? Sender, PropertyChangedEventArgs E)
    {
        if(E.PropertyName is not { } property_name) return;
        if (property_name == __Items_PropertyName)
            _IndexersValues.Clear();
        else
            _PropertiesValues.Remove(property_name);
        OnPropertyChanged(property_name);
    }

    /// <summary>Обработчик события закрытия диалогового окна</summary>
    /// <remarks>В случае положительного диалогового результат переписывает значения словаря значений свойств в свойства исходного конфигурируемого объекта</remarks>
    private void OnDialogWindowClosed(object? Sender, EventArgs E)
    {
        if (Sender is not Window window) return;
        var result                                                                                            = window.DialogResult;
        if (_Value is INotifyPropertyChanged notify_property_changed) notify_property_changed.PropertyChanged -= OnValuePropertyChanged;
        if (result != true) return;
        foreach (var (key, value) in _PropertiesValues.ToArray())
            _ObjectProperties[key].SetValue(_Value, value, null);
        //foreach (var value_info in _IndexersValues)
        //    _ObjectIndexers[value_info.Key].SetValue(_Value, value_info.Value, value_info.Key);
    }

    /// <inheritdoc />
    /// <remarks>Попытка определить значение свойства: если в словаре свойств существует значение по ключу - имени свойства, то оно будет передано в качестве значения</remarks>
    protected override bool TryGetPropertyValue(string property, out object? value)
    {
        if (!base.TryGetPropertyValue(property, out value))
            value = _ObjectProperties[property].GetValue(_Value, null);
        return true;
    }

    /// <inheritdoc />
    /// <remarks>При отсутствии записи в словаре свойств производится поиск свойства исходного объекта и определение его значения</remarks>
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (_Value is null)
        {
            result = null;
            Debug.WriteLine($"Попытка получить значение свойства {binder.Name} у отсутствующего дочернего объекта View-модели.");
            return false;
        }
        result = null;
        var operation_result = _ObjectProperties.TryGetValue(binder.Name, out var info)
            && info.CanRead
            && base.TryGetMember(binder, out result);
        Debug.WriteLineIf(!operation_result, $"Попытка получить доступ к отсутствующему свойству View-модели данных\r\n{string.Join("\r\n", new StackTrace().GetFrames().Select(s => s.GetMethod()?.ToString()))}");
        return operation_result;
    }

    /// <inheritdoc />
    /// <remarks>
    /// При записи значения свойства, если в исходном конфигурируемом объекте свойство с таким именем существует, 
    /// то сохраняем переданное значение в словаре, проводя предварительно преобразование типа перданного объекта в тип, поддерживаемый свойством
    /// </remarks>
    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        if (_Value is null)
        {
            Debug.WriteLine($"Попытка установить значение свойства {binder.Name}={value ?? "null"} отсутствующему дочернему объекту View-модели.");
            return false;
        }
        if (!_ObjectProperties.TryGetValue(binder.Name, out var info) || !info.CanWrite)
        {
            Debug.WriteLine($"Попытка получить доступ к отсутствующему свойству View-модели данных\r\n{string.Join("\r\n", new StackTrace().GetFrames().Select(s => s.GetMethod()?.ToString()))}");
            return false;
        }
        var property_type = info.PropertyType;
        if (value != null && value.GetType() != property_type)
            value = Convert.ChangeType(value, property_type);
        return base.TrySetMember(binder, value);
    }

    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
    {
        foreach (var indexer in _ObjectIndexers.Where(i => i.CanRead))
        {
            var index_parameters = indexer.GetIndexParameters();
            if (index_parameters.Length != indexes.Length) continue;
            var flag = true;
            for (var j = 0; j < indexes.Length && flag; j++)
                if (indexes[j].GetType() != index_parameters[j].ParameterType) flag = false;
            if (!flag) continue;
            if (!base.TryGetIndex(binder, indexes, out result))
                result = indexer.GetValue(_Value, indexes);
            else if (result?.GetType() != indexer.PropertyType)
                result = Convert.ChangeType(result, indexer.PropertyType);
            return true;
        }
        result = null;
        return false;
    }

    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
    {
        foreach (var indexer in _ObjectIndexers.Where(i => i.CanWrite))
        {
            var index_parameters = indexer.GetIndexParameters();
            if (index_parameters.Length != indexes.Length) continue;
            var flag = true;
            for (var j = 0; j < indexes.Length && flag; j++)
                if (indexes[j].GetType() != index_parameters[j].ParameterType) flag = false;
            if (!flag) continue;
            var property_type = indexer.PropertyType;
            if (value != null && value.GetType() != property_type)
                value = Convert.ChangeType(value, property_type);
            return base.TrySetIndex(binder, indexes, value);
        }
        return false;
    }

    /// <inheritdoc />
    public override IEnumerable<string> GetDynamicMemberNames() => _ObjectProperties.Values.Select(p => p.Name);

    /// <summary>Отбросить все сделанные изменения</summary>
    /// <remarks>Очистить все значения словаря, вызвать для каждой записи событие изменения свойства</remarks>
    public void Restore()
    {
        if (_Value is null)
        {
            Debug.WriteLine($"Попытка получить доступ к отсутствующей View-модели данных\r\n{string.Join("\r\n", new StackTrace().GetFrames().Select(s => s.GetMethod()?.ToString()))}");
            return;
        }
        if (_PropertiesValues.Count > 0)
        {
            var properties = _PropertiesValues.Keys.ToArray();
            _PropertiesValues.Clear();
            foreach (var property_name in properties)
                OnPropertyChanged(property_name);
        }
        if (_IndexersValues.Count > 0)
        {
            _IndexersValues.Clear();
            OnPropertyChanged(__Items_PropertyName);
        }
    }
}