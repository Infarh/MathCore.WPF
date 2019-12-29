using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace MathCore.WPF
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class EventBinding : MarkupExtension
    {
        private EventInfo _TargetEvent;
        private Delegate _LastEventHandler;
        private FrameworkElement _Target;
        public string EventHandlerName { get; set; }

        public override object ProvideValue(IServiceProvider sp)
        {
            var target_service = sp.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget
                                 ?? throw new InvalidOperationException("Сервис доступа к объектам разметки XAML не обнаружен");
            _Target = target_service.TargetObject as FrameworkElement
                       ?? throw new InvalidOperationException("Элемент разметки XAML, к которому применяется расширение разметки, не определён как элемент типа FrameworkElement");
            _TargetEvent = target_service.TargetProperty as EventInfo
                            ?? throw new InvalidOperationException("Свойство разметки XAML не определено как событие");

            _Target.DataContextChanged += OnTargetDatacontextChanged;

            return _LastEventHandler = GetEventHandler(_Target.DataContext) ?? GetDefaultEventHandler();
        }

        private void OnTargetDatacontextChanged(object Sender, DependencyPropertyChangedEventArgs E)
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

        private Delegate GetEventHandler(object Context)
        {
            if (Context is null) return null;
            var context_type = Context.GetType();
            var methods = context_type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            var event_handler_type = _TargetEvent.EventHandlerType;
            var invoke_method = event_handler_type.GetMethod(nameof(Action.Invoke), BindingFlags.Public | BindingFlags.Instance) ?? throw new InvalidOperationException();
            var invoke_method_parameters = invoke_method.GetParameters();
            MethodInfo target_method = null;
            foreach (var method in methods.Where(m => m.ReturnType == invoke_method.ReturnType))
            {
                var method_parameters = method.GetParameters();
                if (method_parameters.Length != invoke_method_parameters.Length) continue;
                var i = 0;
                for (; i < method_parameters.Length; i++)
                    if (!method_parameters[i].ParameterType.IsAssignableFrom(invoke_method_parameters[i].ParameterType))
                        break;
                if (i < method_parameters.Length)
                    continue;
                target_method = method;
                break;
            }

            return target_method?.CreateDelegate(event_handler_type, Context);
        }

        private Delegate GetDefaultEventHandler()
        {
            var event_handler_type = _TargetEvent.EventHandlerType;
            var invoke_method = event_handler_type.GetMethod(nameof(Action.Invoke), BindingFlags.Public | BindingFlags.Instance) ?? throw new InvalidOperationException();
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
}