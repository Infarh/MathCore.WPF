using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Input;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace MathCore.WPF.Commands
{
    /// <summary>Defines the attached properties to create a CommandBehaviorBinding</summary>
    public class CommandBehavior
    {
        #region Behavior

        /// <summary>Behavior Attached Dependency Property</summary>
        private static readonly DependencyProperty __BehaviorProperty =
            DependencyProperty.RegisterAttached(
                "__Behavior",
                typeof(CommandBehaviorBinding),
                typeof(CommandBehavior),
                new FrameworkPropertyMetadata(default(CommandBehaviorBinding)));

        /// <summary>Gets the Behavior property</summary>
        private static CommandBehaviorBinding Get__Behavior([NotNull] DependencyObject d) => (CommandBehaviorBinding)d.GetValue(__BehaviorProperty);

        /// <summary>Sets the Behavior property</summary>
        private static void Set__Behavior([NotNull] DependencyObject d, CommandBehaviorBinding value) => d.SetValue(__BehaviorProperty, value);

        #endregion

        #region Command

        /// <summary>Command Attached Dependency Property</summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(CommandBehavior),
                new FrameworkPropertyMetadata(default(ICommand), OnCommandChanged));

        /// <summary>Gets the Command property</summary>
        public static ICommand GetCommand([NotNull] DependencyObject d) => (ICommand)d.GetValue(CommandProperty);

        /// <summary>Sets the Command property</summary>
        public static void SetCommand([NotNull] DependencyObject d, ICommand value) => d.SetValue(CommandProperty, value);

        /// <summary>Handles changes to the Command property</summary>
        private static void OnCommandChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            FetchOrCreateBinding(d).Command = (ICommand)e.NewValue;

        #endregion

        #region CommandParameter

        /// <summary>CommandParameter Attached Dependency Property</summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(object),
                typeof(CommandBehavior),
                new FrameworkPropertyMetadata(default, OnCommandParameterChanged));

        /// <summary>Gets the CommandParameter property</summary>
        public static object GetCommandParameter([NotNull] DependencyObject d) => d.GetValue(CommandParameterProperty);

        /// <summary>Sets the CommandParameter property</summary>
        public static void SetCommandParameter([NotNull] DependencyObject d, object value) =>
            d.SetValue(CommandParameterProperty, value);

        /// <summary>Handles changes to the CommandParameter property</summary>
        private static void OnCommandParameterChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            FetchOrCreateBinding(d).CommandParameter = e.NewValue;

        #endregion

        #region Event

        /// <summary>Event Attached Dependency Property</summary>
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.RegisterAttached(
                "Event",
                typeof(string),
                typeof(CommandBehavior),
                new FrameworkPropertyMetadata(string.Empty, OnEventChanged));

        /// <summary>Gets the Event property.  This dependency property indicates ....</summary>
        public static string GetEvent([NotNull] DependencyObject d) => (string)d.GetValue(EventProperty);

        /// <summary>Sets the Event property.  This dependency property indicates ....</summary>
        public static void SetEvent([NotNull] DependencyObject d, string value) => d.SetValue(EventProperty, value);

        /// <summary>Handles changes to the Event property</summary>
        private static void OnEventChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var binding = FetchOrCreateBinding(d);
            //check if the Event is set. If yes we need to rebind the Command to the new event and unregister the old one
            if(binding.Event != null && binding.Owner != null)
                binding.Dispose();
            //bind the new event to the command
            binding.BindEvent(d, e.NewValue.ToString()!);
        }

        #endregion

        #region Helpers
        //tries to get a CommandBehaviorBinding from the element. Creates a new instance if there is not one attached
        [NotNull]
        private static CommandBehaviorBinding FetchOrCreateBinding([NotNull] DependencyObject d)
        {
            var binding = Get__Behavior(d);
            if(binding != null) return binding;
            binding = new CommandBehaviorBinding();
            Set__Behavior(d, binding);
            return binding;
        }
        #endregion

    }

    /// <summary>Defines the command behavior binding</summary>
    public class CommandBehaviorBinding : IDisposable
    {
        #region Properties

        /// <summary>Get the owner of the CommandBinding ex: a Button. This property can only be set from the BindEvent Method</summary>
        public DependencyObject? Owner { get; private set; }

        /// <summary>The command to execute when the specified event is raised</summary>
        public ICommand? Command { get; set; }

        /// <summary>Gets or sets a CommandParameter</summary>
        public object? CommandParameter { get; set; }

        /// <summary>The event name to hook up to. This property can only be set from the BindEvent Method</summary>
        public string? EventName { get; private set; }

        /// <summary>The event info of the event</summary>
        public EventInfo? Event { get; private set; }

        /// <summary>Gets the EventHandler for the binding with the event</summary>
        public Delegate? EventHandler { get; private set; }

        #endregion

        //Creates an EventHandler on runtime and registers that handler to the Event specified
        public void BindEvent(DependencyObject owner, string EventName)
        {
            this.EventName = EventName;
            Owner = owner;
            this.Event = Owner.GetType().GetEvent(this.EventName, BindingFlags.Public | BindingFlags.Instance) ?? throw new InvalidOperationException();
            if(this.Event is null)
                throw new InvalidOperationException($"Could not resolve event name {this.EventName}");

            //Create an event handler for the event that will call the ExecuteCommand method
            EventHandler = EventHandlerGenerator.CreateDelegate(
                this.Event.EventHandlerType!,
                typeof(CommandBehaviorBinding).GetMethod("ExecuteCommand", BindingFlags.Public | BindingFlags.Instance) ?? throw new InvalidOperationException(),
                this);
            //Register the handler to the Event
            this.Event.AddEventHandler(Owner, EventHandler);
        }

        /// <summary>Executes the command</summary>
        public void ExecuteCommand()
        {
            var command = Command;
            if(command?.CanExecute(CommandParameter) == true)
                command.Execute(CommandParameter);
        }

        #region IDisposable Members

        private bool _Disposed;

        /// <summary>Unregister the EventHandler from the Event</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposing || _Disposed) return;
            Event?.RemoveEventHandler(Owner, EventHandler);
            _Disposed = true;
        }

        #endregion
    }

    /// <summary>Generates delegates according to the specified signature on runtime</summary>
    public static class EventHandlerGenerator
    {
        /// <summary>
        /// Generates a delegate with a matching signature of the supplied eventHandlerType
        /// This method only supports Events that have a delegate of type void
        /// </summary>
        /// <param name="MethodToInvoke">The method to invoke</param>
        /// <param name="MethodInvoker">The object where the method resides</param>
        /// <returns>Returns a delegate with the same signature as eventHandlerType that calls the methodToInvoke inside</returns>
        /// <exception cref="ApplicationException">Delegate has a return type. This only suppress event handlers that are void</exception>
        public static Delegate CreateDelegate([NotNull] Type EventHandlerType, MethodInfo MethodToInvoke, [NotNull] object MethodInvoker)
        {
            //Get the eventHandlerType signature
            var event_handler_info = EventHandlerType.GetMethod("Invoke") ?? throw new InvalidOperationException("Method Invoke not found");
            var return_type = event_handler_info.ReturnParameter.NotNull().ParameterType;
            if(return_type != typeof(void))
                throw new ApplicationException("Delegate has a return type. This only supprts event handlers that are void");

            var delegate_parameters = event_handler_info.GetParameters();
            //Get the list of type of parameters. Please note that we do + 1 because we have to push the object where the method resides i.e methodInvoker parameter
            var hookup_parameters = new Type[delegate_parameters.Length + 1];
            hookup_parameters[0] = MethodInvoker.GetType();
            for(var i = 0; i < delegate_parameters.Length; i++)
                hookup_parameters[i + 1] = delegate_parameters[i].ParameterType;

            var handler = new DynamicMethod("", null,
                hookup_parameters, typeof(EventHandlerGenerator));

            var event_il = handler.GetILGenerator();

            //load the parameters or everything will just BAM :)
            var local = event_il.DeclareLocal(typeof(object[]));
            event_il.Emit(OpCodes.Ldc_I4, delegate_parameters.Length + 1);
            event_il.Emit(OpCodes.Newarr, typeof(object));
            event_il.Emit(OpCodes.Stloc, local);

            //start from 1 because the first item is the instance. Load up all the arguments
            for(var i = 1; i < delegate_parameters.Length + 1; i++)
            {
                event_il.Emit(OpCodes.Ldloc, local);
                event_il.Emit(OpCodes.Ldc_I4, i);
                event_il.Emit(OpCodes.Ldarg, i);
                event_il.Emit(OpCodes.Stelem_Ref);
            }

            event_il.Emit(OpCodes.Ldloc, local);

            //Load as first argument the instance of the object for the methodToInvoke i.e methodInvoker
            event_il.Emit(OpCodes.Ldarg_0);

            //Now that we have it all set up call the actual method that we want to call for the binding
            event_il.EmitCall(OpCodes.Call, MethodToInvoke, null);

            event_il.Emit(OpCodes.Pop);
            event_il.Emit(OpCodes.Ret);

            //create a delegate from the dynamic method
            return handler.CreateDelegate(EventHandlerType, MethodInvoker);
        }
    }
}