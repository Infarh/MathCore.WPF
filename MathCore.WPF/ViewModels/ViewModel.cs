using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xaml;
using MathCore.Annotations;
using SuppressMessageAttribute = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedParameter.Global

namespace MathCore.WPF.ViewModels
{
    /// <summary>Визуальная объектная модель, представляющая механизм обработки событий изменения свойств</summary>
    public abstract partial class ViewModel : MarkupExtension, INotifyPropertyChanged, IDisposable //,IDataErrorInfo
    {
        private event PropertyChangedEventHandler? PropertyChangedEvent;

        /// <summary>Событие возникает когда изменяется значение свойства объекта</summary>
        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                if(value is null) return;
                lock (_PropertiesDependenciesSyncRoot)
                    PropertyChanged_AddHandler(value);
            }
            remove
            {
                if(value is null) return;
                lock (_PropertiesDependenciesSyncRoot)
                    PropertyChanged_RemoveHandler(value);
            }
        }

        /// <summary>Присоединить обработчик события <see cref="PropertyChanged"/></summary>
        /// <param name="handler">Присоединяемый обработчик события <see cref="PropertyChanged"/></param>
        protected virtual void PropertyChanged_AddHandler([NotNull] PropertyChangedEventHandler handler) => PropertyChangedEvent += handler;

        /// <summary>Отсоединить обработчик события <see cref="PropertyChanged"/></summary>
        /// <param name="handler">Отсоединяемый обработчик события <see cref="PropertyChanged"/></param>
        protected virtual void PropertyChanged_RemoveHandler([NotNull] PropertyChangedEventHandler handler) => PropertyChangedEvent -= handler;

        /// <summary>Признак того, что мы находимся в режиме разработки под Visual Studio</summary>
        public static bool IsDesignMode => LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        [NotNull] private readonly object _PropertiesDependenciesSyncRoot = new object();

        /// <summary>Словарь графа зависимости изменений свойств</summary>
        private Dictionary<string, List<string>>? _PropertiesDependenciesDictionary;

        /// <summary>Добавить зависимости между свойствами</summary>
        /// <param name="PropertyName">Имя исходного свойства</param>
        /// <param name="Dependences">Перечисление свойств, на которые исходное свойство имеет влияние</param>
        protected void PropertyDependence_Add([NotNull] string PropertyName, [NotNull, ItemNotNull] params string[] Dependences)
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
                    dependencies_dictionary = new Dictionary<string, List<string>>();
                    _PropertiesDependenciesDictionary = dependencies_dictionary;
                }
                else dependencies_dictionary = _PropertiesDependenciesDictionary;

                // Извлекаем из словаря зависимостей список зависящих от указанного свойства свойств (если он не существует, то создаём новый
                var dependencies = dependencies_dictionary.GetValueOrAddNew(PropertyName, () => new List<string>());

                // Перебираем все зависимые свойства среди указанных исключая исходное свойство
                foreach (var dependence_property in Dependences.Where(name => name != PropertyName))
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
        /// <param name="next_property">Следующее свойство в цепочке зависимости</param>
        /// <param name="invoke_stack">Стек вызова</param>
        /// <returns>Истина, если найден цикл</returns>
        [ItemNotNull, CanBeNull]
        private Queue<string>? IsLoopDependency([NotNull] string property, [NotNull] string dependence, [CanBeNull] string next_property = null, [CanBeNull, ItemNotNull] Stack<string> invoke_stack = null)
        {
            if (invoke_stack is null) invoke_stack = new Stack<string> { property };
            if (string.Equals(property, next_property)) return invoke_stack.ToQueueReverse().AddValue(property);
            var check_property = next_property ?? dependence;
            Debug.Assert(_PropertiesDependenciesDictionary != null, $"{nameof(_PropertiesDependenciesDictionary)} != null");
            if (!_PropertiesDependenciesDictionary.TryGetValue(check_property, out var dependence_properties)) return null;
            foreach (var dependence_property in dependence_properties)
            {
                var invoke_queue = IsLoopDependency(property, dependence, dependence_property, invoke_stack.AddValue(check_property));
                if (invoke_queue != null) return invoke_queue;
            }
            invoke_stack.Pop();
            return null;
        }

        /// <summary>Удаление зависимости между свойствами</summary>
        /// <param name="PropertyName">Исходное свойство</param>
        /// <param name="Dependence">Свойство, связь с которым надо разорвать</param>
        /// <returns>Истина, если связь успешно удалена, ложь - если связь отсутствовала</returns>
        protected bool PropertyDependencies_Remove([NotNull] string PropertyName, [NotNull] string Dependence)
        {
            lock (_PropertiesDependenciesSyncRoot)
            {
                if (_PropertiesDependenciesDictionary?.ContainsKey(PropertyName) != true) return false;
                var dependences = _PropertiesDependenciesDictionary[PropertyName];
                var result = dependences.Remove(Dependence);
                if (dependences.Count == 0) _PropertiesDependenciesDictionary.Remove(PropertyName);
                return result;
            }
        }

        /// <summary>Очистить граф зависимостей между свойствами для указанного свойства</summary>
        /// <param name="PropertyName">Название свойства, связи которого нао удалить</param>
        protected bool PropertyDependencies_Clear([NotNull] string PropertyName)
        {
            lock (_PropertiesDependenciesSyncRoot)
            {
                if (!(_PropertiesDependenciesDictionary?.Remove(PropertyName) ?? false)) return false;
                if (_PropertiesDependenciesDictionary.Count == 0) _PropertiesDependenciesDictionary = null;
                return true;
            }
        }

        private Dictionary<string, Action>? _PropertyChangedHandlers;

        protected void PropertyChanged_AddHandler([NotNull] string PropertyName, [NotNull] Action handler)
        {
            lock (_PropertiesDependenciesSyncRoot)
            {
                var handlers = _PropertyChangedHandlers ??= new Dictionary<string, Action>();
                if (handlers.ContainsKey(PropertyName)) handlers[PropertyName] += handler;
                else handlers.Add(PropertyName, handler);
            }
        }

        protected bool PropertyChanged_RemoveHandler([NotNull] string PropertyName, [NotNull] Action handler)
        {
            lock (_PropertiesDependenciesSyncRoot)
            {
                if (_PropertyChangedHandlers is null || _PropertyChangedHandlers.Count == 0 || !_PropertyChangedHandlers.TryGetValue(PropertyName, out var h)) return false;
                // ReSharper disable once DelegateSubtraction
                h -= handler;
                if (h is null) _PropertyChangedHandlers.Remove(PropertyName);
                return true;
            }
        }

        protected bool PropertyChanged_ClearHandlers([NotNull] string PropertyName)
        {
            lock (_PropertiesDependenciesSyncRoot)
                return _PropertyChangedHandlers != null && _PropertyChangedHandlers.Count > 0 &&
                       _PropertyChangedHandlers.Remove(PropertyName);
        }

        protected virtual bool PropertyChanged_ClearHandlers()
        {
            lock (_PropertiesDependenciesSyncRoot)
            {
                if (_PropertyChangedHandlers is null || _PropertyChangedHandlers.Count == 0) return false;
                _PropertyChangedHandlers.Clear();
                return true;
            }
        }

        /// <summary>Генерация события изменения значения свойства</summary>
        /// <param name="PropertyName">Имя изменившегося свойства</param>
        /// <param name="UpdateCommandsState">Обновить состояния <see cref="ICommand"/></param>
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected virtual void OnPropertyChanged([CallerMemberName, NotNull] string PropertyName = null, bool UpdateCommandsState = false)
        {
            if (PropertyName is null) return; // Если имя свойства не указано, то выход
            // Извлекаем всех подписчиков события
            var handlers = PropertyChangedEvent;
            handlers?.ThreadSafeInvoke(this, PropertyName);
            string[]? dependencies = null;
            var properties_dependencies_dictionary = _PropertiesDependenciesDictionary;
            if (properties_dependencies_dictionary != null)
                lock (properties_dependencies_dictionary)
                    if (properties_dependencies_dictionary.ContainsKey(PropertyName))
                        dependencies = properties_dependencies_dictionary[PropertyName].Where(name => name != PropertyName).ToArray();
            var dependency_handlers = _PropertyChangedHandlers;
            if (dependency_handlers != null && dependency_handlers.TryGetValue(PropertyName, out var handler)) handler?.Invoke();
            if (dependencies != null)
            {
                handlers?.ThreadSafeInvoke(this, dependencies.ToArray());
                if (dependency_handlers != null)
                    foreach (var dependence in dependencies)
                        if (dependency_handlers.TryGetValue(dependence, out handler)) handler?.Invoke();
            }
            if (UpdateCommandsState) CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>Словарь, хранящий время последней генерации события изменения указанного свойства в асинхронном режиме</summary>
        [NotNull] private readonly Dictionary<string, DateTime> _PropertyAsyncInvokeTime = new Dictionary<string, DateTime>();

        /// <summary>Асинхронная генерация события изменения свойства с возможностью указания таймаута ожидания повторных изменений</summary>
        /// <param name="PropertyName">Имя свойства</param>
        /// <param name="Timeout">Таймаут ожидания повторных изменений, прежде чем событие будет регенерировано</param>
        /// <param name="OnChanging">Метод, выполняемый до генерации события</param>
        /// <param name="OnChanged">Метод, выполняемый после генерации события</param>
        protected async void OnPropertyChangedAsync([NotNull] string PropertyName, int Timeout = 0, Action? OnChanging = null, [CanBeNull] Action OnChanged = null)
        {
            if (Timeout == 0)
            {
                OnPropertyChanged(PropertyName);
                return;
            }
            var now = DateTime.Now;
            if (_PropertyAsyncInvokeTime.TryGetValue(PropertyName, out var last_call_time) && (now - last_call_time).TotalMilliseconds < Timeout)
            {
                _PropertyAsyncInvokeTime[PropertyName] = now;
                return;
            }

            _PropertyAsyncInvokeTime[PropertyName] = now;
            var delta = Timeout - (DateTime.Now - _PropertyAsyncInvokeTime[PropertyName]).TotalMilliseconds;
            while (delta > 0)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(delta)).ConfigureAwait(true);
                delta = Timeout - (DateTime.Now - _PropertyAsyncInvokeTime[PropertyName]).TotalMilliseconds;
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
                foreach (DependsOnAttribute depends_on_attribute in property.GetCustomAttributes(typeof(DependsOnAttribute), true))
                    PropertyDependence_Add(depends_on_attribute.Name, property.Name);
                foreach (var depends_on_attribute in property.GetCustomAttributes(typeof(DependencyOnAttribute), true).OfType<DependencyOnAttribute>())
                    PropertyDependence_Add(depends_on_attribute.Name, property.Name);
                foreach (var affects_the_attribute in property.GetCustomAttributes(typeof(AffectsTheAttribute), true).OfType<AffectsTheAttribute>())
                    PropertyDependence_Add(property.Name, affects_the_attribute.Name);
                foreach (var changed_handler_attribute in property.GetCustomAttributes(typeof(ChangedHandlerAttribute), true).OfType<ChangedHandlerAttribute>().Where(a => !string.IsNullOrWhiteSpace(a.MethodName)))
                {
                    var handler = type.GetMethod(changed_handler_attribute.MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (handler is null) throw new InvalidOperationException(
                        $"Для свойства {property.Name} определён атрибут {typeof(ChangedHandlerAttribute).Name}, но в классе {type.Name} отсутствует " +
                        $"указанный в атрибуте метод реакции на изменение значения свойства {changed_handler_attribute.MethodName}");
                    PropertyChanged_AddHandler(property.Name, (Action)Delegate.CreateDelegate(typeof(Action), this, handler));
                }
            }
        }

        [NotNull] private readonly Dictionary<string, object?> _ModelPropertyValues = new Dictionary<string, object?>();

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        protected bool Set<T>([CanBeNull] T value, [CallerMemberName, NotNull] string Property = null, bool UpdateCommandsState = false)
        {
            if (_ModelPropertyValues.TryGetValue(Property ?? throw new ArgumentNullException(nameof(Property), "Имя свойства не задано"), out var old_value) && Equals(old_value, value)) 
                return false;
            _ModelPropertyValues[Property] = value;
            OnPropertyChanged(Property, UpdateCommandsState);
            return true;
        }

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull"), CanBeNull]
        protected T Get<T>([CallerMemberName, NotNull] string Property = null) => 
            _ModelPropertyValues.TryGetValue(Property ?? throw new ArgumentNullException(nameof(Property)), out var value)
#pragma warning disable CS8601 // Возможно, назначение-ссылка, допускающее значение NULL.
                ? (T)value
#pragma warning restore CS8601 // Возможно, назначение-ссылка, допускающее значение NULL.
                : default;

        /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
        /// <typeparam name="T">Тип значения свойства</typeparam>
        /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
        /// <param name="value">Значение свойства, которое надо установить</param>
        /// <param name="Sender">Объект-источник события</param>
        /// <param name="PropertyName">Имя свойства</param>
        /// <param name="OnPropertyChanged">Метод уведомления об изменении значения свойства</param>
        /// <returns>Истина, если значение свойства установлено успешно</returns>
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        public static bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] PropertyChangedEventHandler OnPropertyChanged, [CanBeNull] object Sender, [CallerMemberName, NotNull] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(Sender, new PropertyChangedEventArgs(PropertyName));
            return true;
        }

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        public static bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Action<object, string> OnPropertyChanged, [CanBeNull] object Sender, [CallerMemberName, NotNull] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(Sender, PropertyName);
            return true;
        }

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        public static bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Action<string> OnPropertyChanged, [CallerMemberName, NotNull] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        public static bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Func<T, bool> ValueChecker, [NotNull] PropertyChangedEventHandler OnPropertyChanged, [CanBeNull] object Sender, [CallerMemberName, NotNull] string PropertyName = null) => ValueChecker(value) && Set(ref field, value, OnPropertyChanged, Sender, PropertyName);

        [NotNull, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        public SetStaticValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Action<string> OnPropertyChanged, [CallerMemberName, NotNull] string PropertyName = null)
        {
            if (OnPropertyChanged is null) throw new ArgumentNullException(nameof(OnPropertyChanged));
            if (Equals(field, value)) return new SetStaticValueResult<T>(false, field, field, OnPropertyChanged);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetStaticValueResult<T>(true, old_value, value, OnPropertyChanged);
        }

        [NotNull, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        public static SetStaticValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Func<T, bool> value_checker, [NotNull] Action<string> OnPropertyChanged, [CallerMemberName, NotNull] string PropertyName = null)
        {
            if (OnPropertyChanged is null) throw new ArgumentNullException(nameof(OnPropertyChanged));
            if (Equals(field, value) || !value_checker(value)) return new SetStaticValueResult<T>(false, field, value, OnPropertyChanged);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetStaticValueResult<T>(true, old_value, value, OnPropertyChanged);
        }

        [CanBeNull]
        public static string? CheckDesignModeFilePath(
            [CanBeNull] string? RelativeFileName,
            [CallerFilePath, CanBeNull]  string? SourceFilePath = null) =>
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
        [NotifyPropertyChangedInvocator, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"),
         SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        protected virtual bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [CallerMemberName, NotNull] string PropertyName = null)
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
        [NotifyPropertyChangedInvocator, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        protected virtual bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [CanBeNull] out T OldValue, [CallerMemberName, NotNull] string PropertyName = null)
        {
            OldValue = field;
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }

        [NotifyPropertyChangedInvocator, NotNull, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        protected virtual SetValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] T value, [CallerMemberName, NotNull] string PropertyName = null)
        {
            if (Equals(field, value)) return new SetValueResult<T>(false, field, field, this);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetValueResult<T>(true, old_value, value, this);
        }

        [NotifyPropertyChangedInvocator, NotNull, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        protected virtual SetValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Func<T, bool> value_checker, [CallerMemberName, NotNull] string PropertyName = null)
        {
            if (Equals(field, value) || !value_checker(value)) return new SetValueResult<T>(false, field, value, this);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetValueResult<T>(true, old_value, value, this);
        }

        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Ожидание>")]
        public class SetValueResult<T>
        {
            private readonly bool _Result;
            [CanBeNull] private readonly T _OldValue;
            [CanBeNull] private readonly T _NewValue;
            [NotNull] private readonly ViewModel _Model;

            internal SetValueResult(bool Result, [CanBeNull] T OldValue, [NotNull] ViewModel model) : this(Result, OldValue, OldValue, model) { }
            internal SetValueResult(bool Result, [CanBeNull] T OldValue, [CanBeNull] T NewValue, [NotNull] ViewModel model)
            {
                _Result = Result;
                _OldValue = OldValue;
                _NewValue = NewValue;
                _Model = model;
            }

            public bool Then([NotNull] Action execute)
            {
                if (_Result) execute();
                return _Result;
            }

            public bool Then([NotNull] Action<object?> execute)
            {
                if (_Result) execute(_NewValue);
                return _Result;
            }

            public bool Then([NotNull] Action<T> execute)
            {
                if (_Result) execute(_NewValue);
                return _Result;
            }

            public bool ThenAsync([NotNull] Action execute)
            {
                if (_Result) Task.Run(execute);
                return _Result;
            }

            public bool ThenAsync([NotNull] Action<T> execute)
            {
                if (_Result) _NewValue.Async(execute);
                return _Result;
            }

            public bool ThenIf([NotNull] Func<T, bool> predicate, [NotNull] Action<T> execute)
            {
                if (_Result && predicate(_NewValue)) execute(_NewValue);
                return _Result;
            }

            public bool ThenIfAsync([NotNull] Func<T, bool> predicate, [NotNull] Action<T> execute)
            {
                if (_Result && predicate(_NewValue)) _NewValue.Async(execute);
                return _Result;
            }

            [NotNull]
            public SetValueResult<T> ThenSet([NotNull] Action<T> SetAction)
            {
                if (_Result) SetAction(_NewValue);
                return this;
            }

            [NotNull]
            public SetValueResult<T> ThenSetAsync([NotNull] Action<T> SetAction)
            {
                if (_Result) _NewValue.Async(SetAction);
                return this;
            }

            public bool Then([NotNull] Action<T, T> execute)
            {
                if (_Result) execute(_OldValue, _NewValue);
                return _Result;
            }

            public bool ThenAsync([NotNull] Action<T, T> execute)
            {
                if (_Result) _OldValue.Async(_NewValue, execute);
                return _Result;
            }

            [NotNull]
            public SetValueResult<T> ThenUpdate([NotNull] string PropertyName, bool UpdateCommands = false)
            {
                if (_Result) _Model.OnPropertyChanged(PropertyName, UpdateCommands);
                return this;
            }

            [NotNull]
            public SetValueResult<T> ThenUpdate([NotNull, ItemNotNull] params string[] PropertyNames)
            {
                if (!_Result) return this;
                foreach (var property in PropertyNames)
                    _Model.OnPropertyChanged(property);
                return this;
            }

            [NotNull]
            public SetValueResult<T> ThenUpdate(bool UpdateCommands, [NotNull, ItemNotNull] params string[] PropertyNames)
            {
                if (!_Result) return this;
                foreach (var property in PropertyNames)
                    _Model.OnPropertyChanged(property, UpdateCommands);
                return this;
            }

            [NotNull]
            public SetValueResult<T> Update([NotNull] string PropertyName, bool UpdateCommands = false)
            {
                _Model.OnPropertyChanged(PropertyName, UpdateCommands);
                return this;
            }

            [NotNull]
            public SetValueResult<T> Update([ItemNotNull, NotNull] params string[] PropertyName)
            {
                foreach (var name in PropertyName) _Model.OnPropertyChanged(name);
                return this;
            }

            public bool AnywayThen([NotNull] Action execute)
            {
                execute();
                return _Result;
            }

            public bool AnywayThen([NotNull] Action<bool> execute)
            {
                execute(_Result);
                return _Result;
            }

            public bool AnywayThen([NotNull] Action<T> execute)
            {
                execute(_NewValue);
                return _Result;
            }

            public bool AnywayThen([NotNull] Action<T, bool> execute)
            {
                execute(_NewValue, _Result);
                return _Result;
            }

            public bool AnywayThen([NotNull] Action<T, T> execute)
            {
                execute(_OldValue, _NewValue);
                return _Result;
            }

            public bool AnywayThen([NotNull] Action<T, T, bool> execute)
            {
                execute(_OldValue, _NewValue, _Result);
                return _Result;
            }

            public static implicit operator bool([NotNull] SetValueResult<T> result) => result._Result;
        }

        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<Ожидание>")]
        public class SetStaticValueResult<T>
        {
            private readonly bool _Result;
            [CanBeNull] private readonly T _OldValue;
            [CanBeNull] private readonly T _NewValue;
            [NotNull] private readonly Action<string> _OnPropertyChanged;

            internal SetStaticValueResult(bool Result, [CanBeNull] T OldValue, [NotNull] Action<string> OnPropertyChanged) : this(Result, OldValue, OldValue, OnPropertyChanged) { }
            internal SetStaticValueResult(bool Result, [CanBeNull] T OldValue, [CanBeNull] T NewValue, [NotNull] Action<string> OnPropertyChanged)
            {
                _Result = Result;
                _OldValue = OldValue;
                _NewValue = NewValue;
                _OnPropertyChanged = OnPropertyChanged;
            }

            public bool Then([NotNull] Action execute)
            {
                if (_Result) execute();
                return _Result;
            }

            public bool Then([NotNull] Action<T> execute)
            {
                if (_Result) execute(_NewValue);
                return _Result;
            }

            public bool Then([NotNull] Action<T, T> execute)
            {
                if (_Result) execute(_OldValue, _NewValue);
                return _Result;
            }

            [NotNull]
            public SetStaticValueResult<T> Update([NotNull] string PropertyName)
            {
                _OnPropertyChanged(PropertyName);
                return this;
            }

            [NotNull]
            public SetStaticValueResult<T> Update([NotNull, ItemNotNull] params string[] PropertyName)
            {
                foreach (var name in PropertyName) _OnPropertyChanged(name);
                return this;
            }

            public bool AnywayThen([NotNull] Action execute)
            {
                execute();
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<bool> execute)
            {
                execute(_Result);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T> execute)
            {
                execute(_NewValue);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T, bool> execute)
            {
                execute(_NewValue, _Result);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T, T> execute)
            {
                execute(_OldValue, _NewValue);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T, T, bool> execute)
            {
                execute(_OldValue, _NewValue, _Result);
                return _Result;
            }
            public static implicit operator bool([NotNull] SetStaticValueResult<T> result) => result._Result;
        }

        /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
        /// <typeparam name="TField">Тип значения свойства</typeparam>
        /// <typeparam name="TValue">Тип значения, устанавливаемого для свойства</typeparam>
        /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
        /// <param name="value">Значение свойства, которое надо установить</param>
        /// <param name="converter">Метод преобразования значения</param>
        /// <param name="PropertyName">Имя свойства</param>
        /// <returns>Истина, если значение свойства установлено успешно</returns>
        [NotifyPropertyChangedInvocator, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"),
         SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        protected virtual bool Set<TField, TValue>([CanBeNull] ref TField field, [CanBeNull] TValue value, [NotNull] Func<TValue, TField> converter, [CallerMemberName, NotNull] string PropertyName = null) => Set(ref field, converter(value), PropertyName);

        /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
        /// <typeparam name="T">Тип значения свойства</typeparam>
        /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
        /// <param name="value">Значение свойства, которое надо установить</param>
        /// <param name="ValueChecker">Метод проверки правильности устанавливаемого значения</param>
        /// <param name="PropertyName">Имя свойства</param>
        /// <returns>Истина, если значение свойства установлено успешно</returns>
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        protected virtual bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Func<T, bool> ValueChecker, [CallerMemberName, NotNull] string PropertyName = null) => ValueChecker(value) && Set(ref field, value, PropertyName);

        /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
        /// <typeparam name="TField">Тип значения свойства</typeparam>
        /// <typeparam name="TValue">Тип значения, получаемого из свойства</typeparam>
        /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
        /// <param name="value">Значение свойства, которое надо установить</param>
        /// <param name="converter">Метод преобразования значения</param>
        /// <param name="ValueChecker">Метод проверки правильности устанавливаемого значения</param>
        /// <param name="PropertyName">Имя свойства</param>
        /// <returns>Истина, если значение свойства установлено успешно</returns>
        [NotifyPropertyChangedInvocator, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"),
         SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        protected virtual bool Set<TField, TValue>([CanBeNull] ref TField field, [CanBeNull] TValue value, [NotNull] Func<TValue, TField> converter, [NotNull] Func<TField, bool> ValueChecker, [CallerMemberName, NotNull] string PropertyName = null) =>
            Set(ref field, converter(value), ValueChecker, PropertyName);

        /// <summary>Метод установки значения свойства, осуществляющий генерацию события изменения свойства</summary>
        /// <typeparam name="T">Тип значения свойства</typeparam>
        /// <param name="field">Ссылка на поле, хранящее значение свойства</param>
        /// <param name="value">Значение свойства, которое надо установить</param>
        /// <param name="UpdateCommandsState">Обновить состояния команд</param>
        /// <param name="PropertyName">Имя свойства</param>
        /// <returns>Истина, если значение свойства установлено успешно</returns>
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        protected virtual bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, bool UpdateCommandsState, [CallerMemberName, NotNull] string PropertyName = null)
        {
            var result = Set(ref field, value, PropertyName);
            if (result && UpdateCommandsState)
                CommandManager.InvalidateRequerySuggested();
            return result;
        }

        [NotNull] private static readonly Task<bool> __SetAsyncComplitedTask = Task.FromResult(false);

        /// <summary>Асинхронный метод изменения значения свойства</summary>
        /// <typeparam name="T">Тип значения свойства</typeparam>
        /// <param name="field">Поле, хранящее значение свойства</param>
        /// <param name="value">Новое значение свойства</param>
        /// <param name="PropertyName">Имя свойства</param>
        /// <returns>Задача, возвращающая истину, если свойство изменило своё значение</returns>
        [NotNull, SuppressMessage("ReSharper", "AssignNullToNotNullAttribute"), SuppressMessage("ReSharper", "NotNullOnImplicitCanBeNull")]
        protected virtual Task<bool> SetAsync<T>([CanBeNull] ref T field, [CanBeNull] T value, [CallerMemberName, NotNull] string PropertyName = null)
        {
            if (Equals(field, value)) return __SetAsyncComplitedTask;
            field = value;
            return InternalSetPropertyValueAsync(PropertyName);
        }

        [NotNull]
        private async Task<bool> InternalSetPropertyValueAsync([NotNull] string PropertyName)
        {
            var handler = new Action<string, bool>(OnPropertyChanged);
            await Task.Factory.FromAsync(handler.BeginInvoke, handler.EndInvoke, PropertyName, false, null).ConfigureAwait(false);
            return true;
        }

        /// <inheritdoc />
        [NotNull]
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
            GC.SuppressFinalize(this);
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
}