using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace MathCore.WPF;

/// <summary>Расширение разметки XAML для динамической привязки событий к обработчикам в DataContext</summary>
/// <remarks>
/// Позволяет связывать события элементов WPF с методами в DataContext, автоматически обновляя привязку при изменении контекста данных.
/// Обработчик события подбирается автоматически по имени (если указано EventHandlerName) или по сигнатуре, или создаётся пустой делегат по умолчанию
/// </remarks>
/// <example>
/// <![CDATA[
/// <!-- Автоматический поиск по сигнатуре -->
/// <Button Click="{local:EventBinding}" />
/// <!-- Явное указание имени обработчика -->
/// <Button Click="{local:EventBinding EventHandlerName=OnButtonClick}" />
/// ]]>
/// </example>
public class EventBinding : MarkupExtension
{
    private EventInfo _TargetEvent;
    private Delegate? _LastEventHandler;
    private FrameworkElement _Target;
    private bool _IsSubscribed;
    
    /// <summary>Имя метода-обработчика события в DataContext</summary>
    public string EventHandlerName { get; set; }

    /// <summary>Предоставляет значение расширения разметки для привязки события</summary>
    /// <param name="sp">Поставщик служб XAML</param>
    /// <returns>Делегат-обработчик события</returns>
    /// <exception cref="InvalidOperationException">Если сервис доступа к объектам разметки не обнаружен, целевой элемент не является FrameworkElement или целевое свойство не является событием</exception>
    public override object ProvideValue(IServiceProvider sp)
    {
        var target_service = sp.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget
            ?? throw new InvalidOperationException("Сервис доступа к объектам разметки XAML не обнаружен");
        _Target = target_service.TargetObject as FrameworkElement
            ?? throw new InvalidOperationException("Элемент разметки XAML, к которому применяется расширение разметки, не определён как элемент типа FrameworkElement");
        _TargetEvent = target_service.TargetProperty as EventInfo
            ?? throw new InvalidOperationException("Свойство разметки XAML не определено как событие");

        // Избегаем множественной подписки
        if (!_IsSubscribed)
        {
            _Target.DataContextChanged += OnTargetDataContextChanged;
            _Target.Unloaded += OnTargetUnloaded; // для очистки ресурсов
            _IsSubscribed = true;
        }

        return _LastEventHandler = GetEventHandler(_Target.DataContext) ?? GetDefaultEventHandler();
    }

    /// <summary>Обрабатывает выгрузку элемента для очистки подписок и предотвращения утечек памяти</summary>
    /// <param name="Sender">Источник события</param>
    /// <param name="E">Аргументы события</param>
    private void OnTargetUnloaded(object Sender, RoutedEventArgs E)
    {
        if (_Target is null) return;

        _Target.DataContextChanged -= OnTargetDataContextChanged;
        _Target.Unloaded -= OnTargetUnloaded;
        _IsSubscribed = false;

        if (_LastEventHandler != null && _TargetEvent != null)
        {
            _TargetEvent.RemoveEventHandler(_Target, _LastEventHandler);
            _LastEventHandler = null;
        }
    }

    /// <summary>Обрабатывает изменение DataContext целевого элемента для обновления привязки обработчика события</summary>
    /// <param name="Sender">Источник события</param>
    /// <param name="E">Аргументы события изменения свойства зависимости</param>
    private void OnTargetDataContextChanged(object Sender, DependencyPropertyChangedEventArgs E)
    {
        if (_LastEventHandler != null)
        {
            _TargetEvent.RemoveEventHandler(_Target, _LastEventHandler);
            _LastEventHandler = null;
        }

        _LastEventHandler = GetEventHandler(E.NewValue);
        if (_LastEventHandler != null)
            _TargetEvent.AddEventHandler(_Target, _LastEventHandler);
    }

    /// <summary>Получает делегат-обработчик события из контекста данных по имени или сигнатуре</summary>
    /// <param name="Context">Контекст данных, содержащий потенциальный обработчик</param>
    /// <returns>Делегат-обработчик события или null, если подходящий метод не найден</returns>
    private Delegate? GetEventHandler(object? Context)
    {
        if (Context is null) return null;

        var context_type = Context.GetType();
        var event_handler_type = _TargetEvent.EventHandlerType;
        var invoke_method = event_handler_type.GetMethod(nameof(Action.Invoke), BindingFlags.Public | BindingFlags.Instance) 
            ?? throw new InvalidOperationException("Метод Invoke типа обработчика события не найден");

        // Если указано имя обработчика, ищем метод по имени
        if (!string.IsNullOrWhiteSpace(EventHandlerName))
        {
            var named_method = context_type.GetMethod(EventHandlerName, BindingFlags.Instance | BindingFlags.Public);
            if (named_method != null && IsMethodCompatibleWithDelegate(named_method, invoke_method))
                return named_method.CreateDelegate(event_handler_type, Context);
        }

        // Иначе ищем по сигнатуре
        var methods = context_type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
        var invoke_method_parameters = invoke_method.GetParameters();
        
        foreach (var method in methods.Where(m => m.ReturnType == invoke_method.ReturnType))
        {
            if (IsMethodCompatibleWithDelegate(method, invoke_method))
                return method.CreateDelegate(event_handler_type, Context);
        }

        return null;
    }

    /// <summary>Проверяет совместимость сигнатуры метода с делегатом события</summary>
    /// <param name="Method">Проверяемый метод</param>
    /// <param name="InvokeMethod">Метод Invoke делегата события</param>
    /// <returns>True, если метод совместим с делегатом</returns>
    private static bool IsMethodCompatibleWithDelegate(MethodInfo Method, MethodInfo InvokeMethod)
    {
        var method_parameters = Method.GetParameters();
        var invoke_method_parameters = InvokeMethod.GetParameters();
        
        if (method_parameters.Length != invoke_method_parameters.Length) 
            return false;

        for (var i = 0; i < method_parameters.Length; i++)
        {
            // Параметр метода должен быть того же типа или базовым для параметра делегата
            if (!invoke_method_parameters[i].ParameterType.IsAssignableFrom(method_parameters[i].ParameterType))
                return false;
        }

        return true;
    }

    /// <summary>Создаёт пустой делегат-обработчик события по умолчанию</summary>
    /// <returns>Скомпилированный пустой делегат с сигнатурой целевого события</returns>
    /// <exception cref="InvalidOperationException">Если метод Invoke типа обработчика не найден</exception>
    private Delegate GetDefaultEventHandler()
    {
        var event_handler_type = _TargetEvent.EventHandlerType;
        var invoke_method = event_handler_type.GetMethod(nameof(Action.Invoke), BindingFlags.Public | BindingFlags.Instance) 
            ?? throw new InvalidOperationException("Метод Invoke типа обработчика события не найден");
        var parameters = invoke_method
           .GetParameters()
           .Select(p => System.Linq.Expressions.Expression.Parameter(p.ParameterType))
           .ToArray();
        var @delegate = System.Linq.Expressions.Expression.Lambda(
                event_handler_type,
                System.Linq.Expressions.Expression.Empty(),
                "EmptyDelegate",
                true,
                parameters)
           .Compile();

        return @delegate;
    }
}