using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xaml;

using MathCore.Annotations;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ParameterHidesMember

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedParameter.Global

namespace MathCore.WPF.ViewModels;

/// <summary>Визуальная объектная модель, представляющая механизм обработки событий изменения свойств</summary>
public abstract partial class ViewModel : MarkupExtension, INotifyPropertyChanged, IDisposable //,IDataErrorInfo
{
    private event PropertyChangedEventHandler? PropertyChangedEvent;

    /// <summary>Событие возникает когда изменяется значение свойства объекта</summary>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add
        {
            if (value is null) return;
            lock (_PropertiesDependenciesSyncRoot)
                PropertyChanged_AddHandler(value);
        }
        remove
        {
            if (value is null) return;
            lock (_PropertiesDependenciesSyncRoot)
                PropertyChanged_RemoveHandler(value);
        }
    }

    /// <summary>Присоединить обработчик события <see cref="PropertyChanged"/></summary>
    /// <param name="handler">Присоединяемый обработчик события <see cref="PropertyChanged"/></param>
    protected virtual void PropertyChanged_AddHandler(PropertyChangedEventHandler handler) => PropertyChangedEvent += handler;

    /// <summary>Отсоединить обработчик события <see cref="PropertyChanged"/></summary>
    /// <param name="handler">Отсоединяемый обработчик события <see cref="PropertyChanged"/></param>
    protected virtual void PropertyChanged_RemoveHandler(PropertyChangedEventHandler handler) => PropertyChangedEvent -= handler;

    /// <summary>Признак того, что мы находимся в режиме разработки под Visual Studio</summary>
    //public static bool IsDesignMode { get; set; } = LicenseManager.UsageMode == LicenseUsageMode.Designtime;
    public static bool IsDesignMode { get; set; } = DesignerProperties.GetIsInDesignMode(new());

    private readonly object _PropertiesDependenciesSyncRoot = new();

    /// <summary>Словарь графа зависимости изменений свойств</summary>
    private Dictionary<string, List<string>>? _PropertiesDependenciesDictionary;

    /// <summary>Добавить зависимости между свойствами</summary>
    /// <param name="PropertyName">Имя исходного свойства</param>
    /// <param name="Dependencies">Перечисление свойств, на которые исходное свойство имеет влияние</param>
    protected void PropertyDependence_Add(string PropertyName, params string[] Dependencies)
    {
        // Если не указано имя свойства, то это ошибка
        if (PropertyName is null) throw new ArgumentNullException(nameof(PropertyName));

        // Блокируем критическую секцию для многопоточных операций
        lock (_PropertiesDependenciesSyncRoot)
        {
            // Если словарь зависимостей не существует, то создаём новый
            Dictionary<string, List<string>> dependencies_dictionary;
            if (_PropertiesDependenciesDictionary is null)
            {
                dependencies_dictionary = [];
                _PropertiesDependenciesDictionary = dependencies_dictionary;
            }
            else dependencies_dictionary = _PropertiesDependenciesDictionary;

            // Извлекаем из словаря зависимостей список зависящих от указанного свойства свойств (если он не существует, то создаём новый
            var dependencies = dependencies_dictionary.GetValueOrAddNew(PropertyName, () => []);

            // Перебираем все зависимые свойства среди указанных исключая исходное свойство
            foreach (var dependence_property in Dependencies.Where(name => name != PropertyName))
            {
                // Если список зависимостей уже содержит зависящее свойство, то пропускаем его
                if (dependencies.Contains(dependence_property)) continue;
                // Проверяем возможные циклы зависимостей
                var invoke_queue = IsLoopDependency(PropertyName, dependence_property);
                if (invoke_queue != null) // Если цикл найден, то это ошибка
                    throw new InvalidOperationException($"Попытка добавить зависимость между свойством {PropertyName} и (->) {dependence_property} вызывающую петлю зависимости [{string.Join(">", invoke_queue)}]");

                // Добавляем свойство в список зависимостей
                dependencies.Add(dependence_property);

                foreach (var other_property in dependencies_dictionary.Keys.Where(name => name != PropertyName))
                {
                    var d = dependencies_dictionary[other_property];
                    if (!d.Contains(PropertyName)) continue;
                    invoke_queue = IsLoopDependency(other_property, dependence_property);
                    if (invoke_queue != null) // Если цикл найден, то это ошибка
                        throw new InvalidOperationException($"Попытка добавить зависимость между свойством {other_property} и (->) {dependence_property} вызывающую петлю зависимости [{string.Join(">", invoke_queue)}]");

                    d.Add(dependence_property);
                }
            }
        }
    }

    /// <summary>Проверка модели на циклические зависимости между свойствами</summary>
    /// <param name="property">Проверяемое свойство</param>
    /// <param name="dependence">Имя свойства зависимости</param>
    /// <param name="NextProperty">Следующее свойство в цепочке зависимости</param>
    /// <param name="InvokeStack">Стек вызова</param>
    /// <returns>Истина, если найден цикл</returns>
    private Queue<string>? IsLoopDependency(string property, string dependence, string? NextProperty = null, Stack<string>? InvokeStack = null)
    {
        InvokeStack ??= [property];
        if (string.Equals(property, NextProperty))
            return InvokeStack.ToQueueReverse().AddValue(property);

        var check_property = NextProperty ?? dependence;
        Debug.Assert(_PropertiesDependenciesDictionary != null, $"{nameof(_PropertiesDependenciesDictionary)} != null");
        if (!_PropertiesDependenciesDictionary.TryGetValue(check_property, out var dependence_properties))
            return null;

        foreach (var dependence_property in dependence_properties)
        {
            var invoke_queue = IsLoopDependency(property, dependence, dependence_property, InvokeStack.AddValue(check_property));
            if (invoke_queue != null)
                return invoke_queue;
        }

        InvokeStack.Pop();
        return null;
    }

    /// <summary>Удаление зависимости между свойствами</summary>
    /// <param name="PropertyName">Исходное свойство</param>
    /// <param name="Dependence">Свойство, связь с которым надо разорвать</param>
    /// <returns>Истина, если связь успешно удалена, ложь - если связь отсутствовала</returns>
    protected bool PropertyDependencies_Remove(string PropertyName, string Dependence)
    {
        lock (_PropertiesDependenciesSyncRoot)
        {
            if (_PropertiesDependenciesDictionary?.ContainsKey(PropertyName) != true)
                return false;

            var dependences = _PropertiesDependenciesDictionary[PropertyName];
            var result = dependences.Remove(Dependence);
            if (dependences.Count == 0)
                _PropertiesDependenciesDictionary.Remove(PropertyName);
            return result;
        }
    }

    /// <summary>Очистить граф зависимостей между свойствами для указанного свойства</summary>
    /// <param name="PropertyName">Название свойства, связи которого нао удалить</param>
    protected bool PropertyDependencies_Clear(string PropertyName)
    {
        lock (_PropertiesDependenciesSyncRoot)
        {
            if (!(_PropertiesDependenciesDictionary?.Remove(PropertyName) ?? false))
                return false;

            if (_PropertiesDependenciesDictionary.Count == 0)
                _PropertiesDependenciesDictionary = null;

            return true;
        }
    }

    private Dictionary<string, Action>? _PropertyChangedHandlers;

    protected void PropertyChanged_AddHandler(string PropertyName, Action handler)
    {
        lock (_PropertiesDependenciesSyncRoot)
        {
            var handlers = _PropertyChangedHandlers ??= [];
            if (handlers.ContainsKey(PropertyName))
                handlers[PropertyName] += handler;
            else
                handlers.Add(PropertyName, handler);
        }
    }

    protected bool PropertyChanged_RemoveHandler(string PropertyName, Action handler)
    {
        lock (_PropertiesDependenciesSyncRoot)
        {
            if (_PropertyChangedHandlers is null || _PropertyChangedHandlers.Count == 0 || !_PropertyChangedHandlers.TryGetValue(PropertyName, out var handlers))
                return false;
            // ReSharper disable once DelegateSubtraction
            handlers -= handler;
            if (handlers is null)
                _PropertyChangedHandlers.Remove(PropertyName);

            return true;
        }
    }

    protected bool PropertyChanged_ClearHandlers(string PropertyName)
    {
        lock (_PropertiesDependenciesSyncRoot)
            return _PropertyChangedHandlers is { Count: > 0 } && _PropertyChangedHandlers.Remove(PropertyName);
    }

    protected virtual bool PropertyChanged_ClearHandlers()
    {
        lock (_PropertiesDependenciesSyncRoot)
        {
            if (_PropertyChangedHandlers is null || _PropertyChangedHandlers.Count == 0)
                return false;

            _PropertyChangedHandlers.Clear();
            return true;
        }
    }

    /// <summary>Генерация события изменения значения свойства</summary>
    /// <param name="PropertyName">Имя изменившегося свойства</param>
    /// <param name="UpdateCommandsState">Обновить состояния <see cref="ICommand"/></param>
    /// <param name="ThreadSafe">Генерировать события безопасным для потоков способом</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null!, bool UpdateCommandsState = false, bool ThreadSafe = true)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (PropertyName is null)
            return; // Если имя свойства не указано, то выход

        if (_PropertyChangedEventsSuppressor != null)
        {
            _PropertyChangedEventsSuppressor.RegisterEvent(PropertyName);
            return;
        }

        // Извлекаем всех подписчиков события
        var handlers = PropertyChangedEvent;
        if (ThreadSafe)
            handlers?.ThreadSafeInvoke(this, PropertyName);
        else
            handlers?.Invoke(this, new(PropertyName));

        string[]? dependencies = null;
        var properties_dependencies_dictionary = _PropertiesDependenciesDictionary;
        if (properties_dependencies_dictionary != null)
            lock (properties_dependencies_dictionary)
                if (properties_dependencies_dictionary.TryGetValue(PropertyName, out var value))
                    dependencies = value.Where(name => name != PropertyName).ToArray();

        var dependency_handlers = _PropertyChangedHandlers;
        if (dependency_handlers != null && dependency_handlers.TryGetValue(PropertyName, out var handler))
            handler();

        if (dependencies is { Length: > 0 })
        {
            handlers?.ThreadSafeInvoke(this, dependencies.ToArray());
            if (dependency_handlers != null)
                foreach (var dependence in dependencies)
                    if (dependency_handlers.TryGetValue(dependence, out handler))
                        handler();
        }
        if (UpdateCommandsState)
            CommandManager.InvalidateRequerySuggested();
    }

    /// <summary>Словарь, хранящий время последней генерации события изменения указанного свойства в асинхронном режиме</summary>
    private readonly Dictionary<string, DateTime> _PropertyAsyncInvokeTime = [];

    /// <summary>Асинхронная генерация события изменения свойства с возможностью указания таймаута ожидания повторных изменений</summary>
    /// <param name="PropertyName">Имя свойства</param>
    /// <param name="Timeout">Таймаут ожидания повторных изменений, прежде чем событие будет регенерировано</param>
    /// <param name="OnChanging">Метод, выполняемый до генерации события</param>
    /// <param name="OnChanged">Метод, выполняемый после генерации события</param>
    protected async void OnPropertyChangedAsync(string PropertyName, int Timeout = 0, Action? OnChanging = null, Action? OnChanged = null)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (PropertyName is null)
            return; // Если имя свойства не указано, то выход

        if (Timeout == 0)
        {
            OnPropertyChanged(PropertyName);
            return;
        }

        if (_PropertyChangedEventsSuppressor != null)
        {
            _PropertyChangedEventsSuppressor.RegisterEvent(PropertyName);
            return;
        }

        var now                    = DateTime.Now;
        var properties_invoke_time = _PropertyAsyncInvokeTime;
        if (properties_invoke_time.TryGetValue(PropertyName, out var last_call_time) && (now - last_call_time).TotalMilliseconds < Timeout)
        {
            properties_invoke_time[PropertyName] = now;
            return;
        }

        properties_invoke_time[PropertyName] = now;
        var delta = Timeout - (DateTime.Now - properties_invoke_time[PropertyName]).TotalMilliseconds;
        while (delta > 0)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(delta)).ConfigureAwait(true);
            delta = Timeout - (DateTime.Now - properties_invoke_time[PropertyName]).TotalMilliseconds;
        }
        OnChanging?.Invoke();
        OnPropertyChanged(PropertyName);
        OnChanged?.Invoke();
    }

    /// <summary>Инициализация новой view-модели</summary><param name="CheckDependencies">Создавать карту зависимостей на основе атрибутов</param>
    protected ViewModel(bool CheckDependencies = true)
    {
        if (!CheckDependencies) return;
        var type = GetType();
        foreach (var property in type.GetProperties())
        {
            foreach (var depends_on_attribute in property.GetCustomAttributes(typeof(DependsOnAttribute), true).Cast<DependsOnAttribute>())
                PropertyDependence_Add(depends_on_attribute.Name, property.Name);

            foreach (var depends_on_attribute in property.GetCustomAttributes(typeof(DependencyOnAttribute), true).OfType<DependencyOnAttribute>())
                PropertyDependence_Add(depends_on_attribute.Name, property.Name);

            foreach (var affects_the_attribute in property.GetCustomAttributes(typeof(AffectsTheAttribute), true).OfType<AffectsTheAttribute>())
                PropertyDependence_Add(property.Name, affects_the_attribute.Name);

            foreach (var changed_handler_attribute in property.GetCustomAttributes(typeof(ChangedHandlerAttribute), true).OfType<ChangedHandlerAttribute>().Where(a => !string.IsNullOrWhiteSpace(a.MethodName)))
            {
                var handler = type.GetMethod(changed_handler_attribute.MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (handler is null) throw new InvalidOperationException(
                    $"Для свойства {property.Name} определён атрибут {nameof(ChangedHandlerAttribute)}, но в классе {type.Name} отсутствует " +
                    $"указанный в атрибуте метод реакции на изменение значения свойства {changed_handler_attribute.MethodName}");
                PropertyChanged_AddHandler(property.Name, (Action)Delegate.CreateDelegate(typeof(Action), this, handler));
            }
        }
    }

    private readonly Dictionary<string, object?> _ModelPropertyValues = [];

    protected bool Set<T>(
        T? value,
        [CallerMemberName] string Property = null!,
        bool UpdateCommandsState = false)
    {
        if (_ModelPropertyValues.TryGetValue(Property ?? throw new ArgumentNullException(nameof(Property), "Имя свойства не задано"), out var old_value) && Equals(old_value, value))
            return false;
        _ModelPropertyValues[Property] = value;
        OnPropertyChanged(Property, UpdateCommandsState);
        return true;
    }

    protected T? Get<T>([CallerMemberName] string Property = null!) =>
        _ModelPropertyValues.TryGetValue(Property ?? throw new ArgumentNullException(nameof(Property)), out var value)
            ? (T?)value
            : default;

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="Sender">Объект-источник события</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <param name="OnPropertyChanged">Метод уведомления об изменении значения свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    public static bool Set<T>(
        ref T? field,
        T? value,
        PropertyChangedEventHandler OnPropertyChanged,
        object? Sender,
        [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(Sender, new(PropertyName));
        return true;
    }

    public static bool Set<T>(
        ref T? field,
        T? value,
        Action<object?, string> OnPropertyChanged,
        object? Sender, [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(Sender, PropertyName);
        return true;
    }

    public static bool Set<T>(
        ref T? field,
        T? value,
        Action<string> OnPropertyChanged,
        [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(PropertyName);
        return true;
    }

    public static bool Set<T>(
        ref T? field,
        T? value,
        Func<T?, bool> ValueChecker,
        PropertyChangedEventHandler OnPropertyChanged,
        object? Sender,
        [CallerMemberName] string PropertyName = null!) =>
        ValueChecker(value) && Set(ref field, value, OnPropertyChanged, Sender, PropertyName);

    public SetStaticValueResult<T> SetValue<T>(
        ref T? field,
        T? value,
        Action<string> OnPropertyChanged,
        [CallerMemberName] string PropertyName = null!)
    {
        if (OnPropertyChanged is null) throw new ArgumentNullException(nameof(OnPropertyChanged));
        if (Equals(field, value)) return new(false, field, field, OnPropertyChanged);
        var old_value = field;
        field = value;
        OnPropertyChanged(PropertyName);
        return new(true, old_value, value, OnPropertyChanged);
    }

    public static SetStaticValueResult<T> SetValue<T>(
        [Attributes.NotNullIfNotNull(nameof(value))] ref T? field, 
        T? value,
        Func<T?, bool> ValueChecker,
        Action<string> OnPropertyChanged,
        [CallerMemberName] string PropertyName = null!)
    {
        if (OnPropertyChanged is null) throw new ArgumentNullException(nameof(OnPropertyChanged));
        if (Equals(field, value) || !ValueChecker(value)) return new(false, field, value, OnPropertyChanged);
        var old_value = field;
        field = value;
        OnPropertyChanged(PropertyName);
        return new(true, old_value, value, OnPropertyChanged);
    }

    public static string? CheckDesignModeFilePath(
        string? RelativeFileName,
        [CallerFilePath] string? SourceFilePath = null) =>
        !IsDesignMode
        || SourceFilePath is null
        || RelativeFileName is null
        || RelativeFileName.Length > 3
        && RelativeFileName[1] == ':'
            ? RelativeFileName
            : Path.Combine(Path.GetDirectoryName(SourceFilePath) ?? "", RelativeFileName);

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool Set<T>(
        [Attributes.NotNullIfNotNull(nameof(value))] ref T? field, 
        T? value, 
        [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(PropertyName);
        return true;
    }

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="OldValue">Предыдущее значение</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool Set<T>([Attributes.NotNullIfNotNull(nameof(value))] ref T? field, T? value, out T? OldValue, [CallerMemberName] string PropertyName = null!)
    {
        OldValue = field;
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(PropertyName);
        return true;
    }

    [NotifyPropertyChangedInvocator]
    protected virtual SetValueResult<T> SetValue<T>([Attributes.NotNullIfNotNull(nameof(value))] ref T? field, T? value, [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value)) return new(false, field, field, this);
        var old_value = field;
        field = value;
        OnPropertyChanged(PropertyName);
        return new(true, old_value, value, this);
    }

    [NotifyPropertyChangedInvocator]
    protected virtual SetValueResult<T> SetValue<T>([Attributes.NotNullIfNotNull(nameof(value))] ref T? field, T? value, Func<T?, bool> value_checker, [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value) || !value_checker(value))
            return new(false, field, value, this);
        var old_value = field;
        field = value;
        OnPropertyChanged(PropertyName);
        return new(true, old_value, value, this);
    }

    public readonly ref struct SetValueResult<T>
    {
        private readonly ViewModel _Model;
        private readonly bool _Result;
        private readonly T? _OldValue;
        private readonly T? _NewValue;

        public bool Result => _Result;
        public T? OldValue => _OldValue;
        public T? NewValue => _NewValue;

        internal SetValueResult(bool Result, T? OldValue, ViewModel model) : this(Result, OldValue, OldValue, model) { }
        internal SetValueResult(bool Result, T? OldValue, T? NewValue, ViewModel model)
        {
            _Result = Result;
            _OldValue = OldValue;
            _NewValue = NewValue;
            _Model = model;
        }

        public bool Then(Action execute)
        {
            if (_Result) execute();
            return _Result;
        }

        public bool Then(Action<object?> execute)
        {
            if (_Result) execute(NewValue);
            return _Result;
        }

        public bool Then(Action<T?> execute)
        {
            if (_Result) execute(NewValue);
            return _Result;
        }

        public bool ThenAsync(Action execute)
        {
            if (_Result) Task.Run(execute);
            return _Result;
        }

        public bool ThenAsync(Action<T?> execute)
        {
            if (_Result) NewValue.Async(execute);
            return _Result;
        }

        public bool ThenIf(Func<T?, bool> predicate, Action<T?> execute)
        {
            if (_Result && predicate(NewValue)) execute(NewValue);
            return _Result;
        }

        public bool ThenIfAsync(Func<T?, bool> predicate, Action<T?> execute)
        {
            if (_Result && predicate(NewValue)) NewValue.Async(execute);
            return _Result;
        }

        public SetValueResult<T> ThenSet(Action<T?> SetAction)
        {
            if (_Result) SetAction(NewValue);
            return this;
        }

        public SetValueResult<T> ThenSetAsync(Action<T?> SetAction)
        {
            if (_Result) NewValue.Async(SetAction);
            return this;
        }

        public bool Then(Action<T?, T?> execute)
        {
            if (_Result) execute(OldValue, NewValue);
            return _Result;
        }

        public bool ThenAsync(Action<T?, T?> execute)
        {
            if (_Result) OldValue.Async(NewValue, execute);
            return _Result;
        }

        public SetValueResult<T> ThenUpdate(string PropertyName, bool UpdateCommands = false)
        {
            if (_Result) _Model.OnPropertyChanged(PropertyName, UpdateCommands);
            return this;
        }

        public SetValueResult<T> ThenUpdate(params string[] PropertyNames)
        {
            if (!_Result) return this;
            foreach (var property in PropertyNames)
                _Model.OnPropertyChanged(property);
            return this;
        }

        public SetValueResult<T> ThenUpdate(bool UpdateCommands, params string[] PropertyNames)
        {
            if (!_Result) return this;
            foreach (var property in PropertyNames)
                _Model.OnPropertyChanged(property, UpdateCommands);
            return this;
        }

        public SetValueResult<T> Update(string PropertyName, bool UpdateCommands = false)
        {
            _Model.OnPropertyChanged(PropertyName, UpdateCommands);
            return this;
        }

        public SetValueResult<T> Update(params string[] PropertyName)
        {
            foreach (var name in PropertyName) _Model.OnPropertyChanged(name);
            return this;
        }

        public bool AnywayThen(Action execute)
        {
            execute();
            return _Result;
        }

        public bool AnywayThen(Action<bool> execute)
        {
            execute(_Result);
            return _Result;
        }

        public bool AnywayThen(Action<T?> execute)
        {
            execute(NewValue);
            return _Result;
        }

        public bool AnywayThen(Action<T?, bool> execute)
        {
            execute(NewValue, _Result);
            return _Result;
        }

        public bool AnywayThen(Action<T?, T?> execute)
        {
            execute(OldValue, NewValue);
            return _Result;
        }

        public bool AnywayThen(Action<T?, T?, bool> execute)
        {
            execute(OldValue, NewValue, _Result);
            return _Result;
        }

        public static implicit operator bool(SetValueResult<T> result) => result._Result;
    }

    public readonly ref struct SetStaticValueResult<T>
    {
        private readonly bool _Result;
        private readonly T? _OldValue;
        private readonly T? _NewValue;
        private readonly Action<string> _OnPropertyChanged;

        internal SetStaticValueResult(bool Result, T? OldValue, Action<string> OnPropertyChanged) : this(Result, OldValue, OldValue, OnPropertyChanged) { }
        internal SetStaticValueResult(bool Result, T? OldValue, T? NewValue, Action<string> OnPropertyChanged)
        {
            _Result = Result;
            _OldValue = OldValue;
            _NewValue = NewValue;
            _OnPropertyChanged = OnPropertyChanged;
        }

        public bool Then(Action execute)
        {
            if (_Result) execute();
            return _Result;
        }

        public bool Then(Action<T?> execute)
        {
            if (_Result) execute(_NewValue);
            return _Result;
        }

        public bool Then(Action<T?, T?> execute)
        {
            if (_Result) execute(_OldValue, _NewValue);
            return _Result;
        }

        public SetStaticValueResult<T> Update(string PropertyName)
        {
            _OnPropertyChanged(PropertyName);
            return this;
        }

        public SetStaticValueResult<T> Update(params string[] PropertyName)
        {
            foreach (var name in PropertyName) _OnPropertyChanged(name);
            return this;
        }

        public bool AnywayThen(Action execute)
        {
            execute();
            return _Result;
        }
        public bool AnywayThen(Action<bool> execute)
        {
            execute(_Result);
            return _Result;
        }
        public bool AnywayThen(Action<T?> execute)
        {
            execute(_NewValue);
            return _Result;
        }
        public bool AnywayThen(Action<T?, bool> execute)
        {
            execute(_NewValue, _Result);
            return _Result;
        }
        public bool AnywayThen(Action<T?, T?> execute)
        {
            execute(_OldValue, _NewValue);
            return _Result;
        }
        public bool AnywayThen(Action<T?, T?, bool> execute)
        {
            execute(_OldValue, _NewValue, _Result);
            return _Result;
        }
        public static implicit operator bool(SetStaticValueResult<T> result) => result._Result;
    }

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="TField">Тип значения свойства</typeparam>
    /// <typeparam name="TValue">Тип значения, устанавливаемого для свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="converter">Метод преобразования значения</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool Set<TField, TValue>(
        [Attributes.NotNullIfNotNull(nameof(value))] ref TField? field,
        TValue? value,
        Func<TValue?, TField?> converter,
        [CallerMemberName] string PropertyName = null!) =>
        Set(ref field, converter(value), PropertyName);

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="ValueChecker">Метод проверки правильности устанавливаемого значения</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    protected virtual bool Set<T>(
        [Attributes.NotNullIfNotNull(nameof(value))] ref T? field,
        T? value,
        Func<T?, bool> ValueChecker,
        [CallerMemberName] string PropertyName = null!) =>
        ValueChecker(value) && Set(ref field, value, PropertyName);

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="TField">Тип значения свойства</typeparam>
    /// <typeparam name="TValue">Тип значения, получаемого из свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="converter">Метод преобразования значения</param>
    /// <param name="ValueChecker">Метод проверки правильности устанавливаемого значения</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    [NotifyPropertyChangedInvocator]
    protected virtual bool Set<TField, TValue>(
        [Attributes.NotNullIfNotNull(nameof(value))] ref TField? field,
        TValue? value,
        Func<TValue?, TField?> converter,
        Func<TField?, bool> ValueChecker,
        [CallerMemberName] string PropertyName = null!) =>
        Set(ref field, converter(value), ValueChecker, PropertyName);

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
    /// <param name="value">Значение свойства, которое надо установить</param>
    /// <param name="UpdateCommandsState">Обновить состояния команд</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    protected virtual bool Set<T>(
        [Attributes.NotNullIfNotNull(nameof(value))] ref T? field,
        T? value,
        bool UpdateCommandsState,
        [CallerMemberName] string PropertyName = null!)
    {
        var result = Set(ref field, value, PropertyName);
        if (result && UpdateCommandsState)
            CommandManager.InvalidateRequerySuggested();
        return result;
    }

    /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="Value">Новое значение свойства</param>
    /// <param name="OldValue">Старое значение свойства</param>
    /// <param name="Setter">Метод установки нового значения свойства</param>
    /// <param name="ValueValidator">Метод проверки возможности установки нового значения свойства</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Истина, если значение свойства установлено успешно</returns>
    protected virtual bool Set<T>(
        T? Value,
        T? OldValue,
        Action<T?> Setter,
        Func<T?, bool>? ValueValidator,
        [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(Value, OldValue)) return false;
        if (ValueValidator is not null && !ValueValidator(Value)) return false;
        Setter(Value);
        OnPropertyChanged(PropertyName);
        return true;
    }

    /// <summary>Асинхронный метод изменения значения свойства</summary>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <param name="field">Поле, хранящее значение свойства</param>
    /// <param name="value">Новое значение свойства</param>
    /// <param name="PropertyName">Имя свойства</param>
    /// <returns>Задача, возвращающая истину, если свойство изменило своё значение</returns>
    protected virtual ValueTask<bool> SetAsync<T>([Attributes.NotNullIfNotNull(nameof(value))] ref T? field, T? value, [CallerMemberName] string PropertyName = null!)
    {
        if (Equals(field, value)) return new(false);
        field = value;

        return SetPropertyAsync(PropertyName);
        async ValueTask<bool> SetPropertyAsync(string property)
        {
            await Task.Factory.StartNew(s => OnPropertyChanged((string)s!), property);
            return true;
        }
    }

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider Service)
    {
        var value_target_service = Service.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        var root_object_service = Service.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
        OnInitialized(value_target_service?.TargetObject, value_target_service?.TargetProperty, root_object_service?.RootObject);
        return this;
    }

    protected virtual void OnInitialized(object? target, object? property, object? root)
    {

    }

    #region IDisposable

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        //GC.SuppressFinalize(this);
    }

    /// <summary>Признак того, что объект уже уничтожен</summary>
    private bool _Disposed;

    /// <summary>Освобождение ресурсов</summary>
    /// <param name="disposing">Если истина, то требуется освободить управляемые объекты. Освободить неуправляемые объекты в любом случае</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_Disposed) return;
        if (disposing) DisposeManagedObject();
        DisposeUnmanagedObject();
        _Disposed = true;
    }

    /// <summary>Освободить управляемые объекты</summary>
    protected virtual void DisposeManagedObject() { }
    /// <summary>Освободить неуправляемые объекты</summary>
    protected virtual void DisposeUnmanagedObject() { }

    #endregion

    //#region IDataErrorInfo

    //string IDataErrorInfo.this[string PropertyName] => throw new NotImplementedException();

    //string IDataErrorInfo.Error => throw new NotImplementedException();

    //#endregion
}