//using System.Reflection;
//using System.Windows;
//using System.Windows.Markup;

//namespace MathCore.WPF;

//// https://github.com/Singulink/Singulink.WPF.Data.MethodBinding/blob/main/Source/Singulink.WPF.Data.MethodBinding/MethodBindingExtension.cs
//public class BindingMethod(params object?[] args) : MarkupExtension
//{
//    private readonly List<DependencyProperty> _ArgumentProperties = [];

//    public string? Method { get; init; }

//    public BindingMethod() : this([]) { }

//    public override object ProvideValue(IServiceProvider sp)
//    {
//        var method = Method;

//        if (method is null && args is [string { Length: > 0 } arg_method, ..])
//            method = arg_method;

//        if (method is not { Length: > 0 })
//            throw new InvalidOperationException("Не задано имя метода");

//        var target_provider = sp.GetValueTargetProvider()!;

//        var event_handler_type = target_provider.TargetProperty switch
//        {
//            EventInfo { EventHandlerType: { } type } => type,
//            MethodInfo method_info when method_info.GetParameters() is [{ ParameterType: var type }, _] => type,
//            _ => null
//        };

//        if(target_provider.TargetObject is DependencyObject target && event_handler_type is not null)
//        {
//            foreach (var arg in args)
//            {
//                var arg_property = SetUnusedStorageProperty(target, arg);
//                _ArgumentProperties.Add(arg_property);
//            }

//            return CreateEventHandler(target, event_handler_type);
//        }

//        throw new InvalidOperationException();

//        DependencyProperty SetUnusedStorageProperty(DependencyObject obj, object? value)
//        {
//            return null;
//        }

//        Delegate CreateEventHandler(DependencyObject source, Type eventHandlerType)
//        {
//            return null!;
//        }
//    }
//}
