using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Input;

namespace MathCore.WPF.Commands
{
    /// <summary>Defines the attached properties to create a CommandBehaviorBinding</summary>
    public class CommandBehavior
    {
        #region Behavior

        /// <summary>Behavior Attached Dependency Property</summary>
        private static readonly DependencyProperty _BehaviorProperty =
            DependencyProperty.RegisterAttached(
                "_Behavior",
                typeof(CommandBehaviorBinding),
                typeof(CommandBehavior),
                new FrameworkPropertyMetadata((CommandBehaviorBinding)null));

        /// <summary>Gets the Behavior property</summary>
        private static CommandBehaviorBinding Getf_Behavior(DependencyObject d) => (CommandBehaviorBinding)d.GetValue(_BehaviorProperty);

        /// <summary>
        /// Sets the Behavior property.  
        /// </summary>
        private static void Setf_Behavior(DependencyObject d, CommandBehaviorBinding value) => d.SetValue(_BehaviorProperty, value);

        #endregion

        #region Command

        /// <summary>Command Attached Dependency Property</summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(CommandBehavior),
                new FrameworkPropertyMetadata(null, OnCommandChanged));

        /// <summary>
        /// Gets the Command property.  
        /// </summary>
        public static ICommand GetCommand(DependencyObject d) => (ICommand)d.GetValue(CommandProperty);

        /// <summary>
        /// Sets the Command property. 
        /// </summary>
        public static void SetCommand(DependencyObject d, ICommand value) => d.SetValue(CommandProperty, value);

        /// <summary>Handles changes to the Command property</summary>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            FetchOrCreateBinding(d).Command = (ICommand)e.NewValue;

        #endregion

        #region CommandParameter

        /// <summary>CommandParameter Attached Dependency Property</summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(object),
                typeof(CommandBehavior),
                new FrameworkPropertyMetadata(null, OnCommandParameterChanged));

        /// <summary>
        /// Gets the CommandParameter property.  
        /// </summary>
        public static object GetCommandParameter(DependencyObject d) => d.GetValue(CommandParameterProperty);

        /// <summary>
        /// Sets the CommandParameter property. 
        /// </summary>
        public static void SetCommandParameter(DependencyObject d, object value) =>
            d.SetValue(CommandParameterProperty, value);

        /// <summary>
        /// Handles changes to the CommandParameter property.
        /// </summary>
        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
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

        /// <summary>
        /// Gets the Event property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static string GetEvent(DependencyObject d) => (string)d.GetValue(EventProperty);

        /// <summary>
        /// Sets the Event property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetEvent(DependencyObject d, string value) => d.SetValue(EventProperty, value);

        /// <summary>Handles changes to the Event property</summary>
        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var binding = FetchOrCreateBinding(d);
            //check if the Event is set. If yes we need to rebind the Command to the new event and unregister the old one
            if(binding.Event != null && binding.Owner != null)
                binding.Dispose();
            //bind the new event to the command
            binding.BindEvent(d, e.NewValue.ToString());
        }

        #endregion

        #region Helpers
        //tries to get a CommandBehaviorBinding from the element. Creates a new instance if there is not one attached
        private static CommandBehaviorBinding FetchOrCreateBinding(DependencyObject d)
        {
            var binding = Getf_Behavior(d);
            if(binding != null) return binding;
            binding = new CommandBehaviorBinding();
            Setf_Behavior(d, binding);
            return binding;
        }
        #endregion

    }

    /// <summary>Defines the command behavior binding</summary>
    public class CommandBehaviorBinding : IDisposable
    {
        #region Properties
        /// <summary>
        /// Get the owner of the CommandBinding ex: a Button
        /// This property can only be set from the BindEvent Method
        /// </summary>
        public DependencyObject Owner { get; private set; }
        /// <summary>
        /// The command to execute when the specified event is raised
        /// </summary>
        public ICommand Command { get; set; }
        /// <summary>
        /// Gets or sets a CommandParameter
        /// </summary>
        public object CommandParameter { get; set; }
        /// <summary>
        /// The event name to hook up to
        /// This property can only be set from the BindEvent Method
        /// </summary>
        public string EventName { get; private set; }
        /// <summary>
        /// The event info of the event
        /// </summary>
        public EventInfo Event { get; private set; }
        /// <summary>
        /// Gets the EventHandler for the binding with the event
        /// </summary>
        public Delegate EventHandler { get; private set; }

        #endregion

        //Creates an EventHandler on runtime and registers that handler to the Event specified
        public void BindEvent(DependencyObject owner, string eventName)
        {
            EventName = eventName;
            Owner = owner;
            Event = Owner.GetType().GetEvent(EventName, BindingFlags.Public | BindingFlags.Instance);
            if(Event == null)
                throw new InvalidOperationException($"Could not resolve event name {EventName}");

            //Create an event handler for the event that will call the ExecuteCommand method
            EventHandler = EventHandlerGenerator.CreateDelegate(
                Event.EventHandlerType,
                typeof(CommandBehaviorBinding).GetMethod("ExecuteCommand", BindingFlags.Public | BindingFlags.Instance),
                this);
            //Register the handler to the Event
            Event.AddEventHandler(Owner, EventHandler);
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        public void ExecuteCommand()
        {
            if(Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }

        #region IDisposable Members
        bool _Disposed;
        /// <summary>
        /// Unregisters the EventHandler from the Event
        /// </summary>
        public void Dispose()
        {
            if(_Disposed) return;
            Event.RemoveEventHandler(Owner, EventHandler);
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
        /// <param name="EventHandlerType"></param>
        /// <param name="MethodToInvoke">The method to invoke</param>
        /// <param name="MethodInvoker">The object where the method resides</param>
        /// <returns>Returns a delegate with the same signature as eventHandlerType that calls the methodToInvoke inside</returns>
        /// <exception cref="ApplicationException">Delegate has a return type. This only supprts event handlers that are void</exception>
        public static Delegate CreateDelegate(Type EventHandlerType, MethodInfo MethodToInvoke, object MethodInvoker)
        {
            //Get the eventHandlerType signature
            var lv_EventHandlerInfo = EventHandlerType.GetMethod("Invoke");
            Debug.Assert(lv_EventHandlerInfo.ReturnParameter != null, "eventHandlerInfo.ReturnParameter != null");
            var lv_ReturnType = lv_EventHandlerInfo.ReturnParameter.ParameterType;
            if(lv_ReturnType != typeof(void))
                throw new ApplicationException("Delegate has a return type. This only supprts event handlers that are void");

            var lv_DelegateParameters = lv_EventHandlerInfo.GetParameters();
            //Get the list of type of parameters. Please note that we do + 1 because we have to push the object where the method resides i.e methodInvoker parameter
            var lv_HookupParameters = new Type[lv_DelegateParameters.Length + 1];
            lv_HookupParameters[0] = MethodInvoker.GetType();
            for(var i = 0; i < lv_DelegateParameters.Length; i++)
                lv_HookupParameters[i + 1] = lv_DelegateParameters[i].ParameterType;

            var handler = new DynamicMethod("", null,
                lv_HookupParameters, typeof(EventHandlerGenerator));

            var lv_EventIl = handler.GetILGenerator();

            //load the parameters or everything will just BAM :)
            var local = lv_EventIl.DeclareLocal(typeof(object[]));
            lv_EventIl.Emit(OpCodes.Ldc_I4, lv_DelegateParameters.Length + 1);
            lv_EventIl.Emit(OpCodes.Newarr, typeof(object));
            lv_EventIl.Emit(OpCodes.Stloc, local);

            //start from 1 because the first item is the instance. Load up all the arguments
            for(var i = 1; i < lv_DelegateParameters.Length + 1; i++)
            {
                lv_EventIl.Emit(OpCodes.Ldloc, local);
                lv_EventIl.Emit(OpCodes.Ldc_I4, i);
                lv_EventIl.Emit(OpCodes.Ldarg, i);
                lv_EventIl.Emit(OpCodes.Stelem_Ref);
            }

            lv_EventIl.Emit(OpCodes.Ldloc, local);

            //Load as first argument the instance of the object for the methodToInvoke i.e methodInvoker
            lv_EventIl.Emit(OpCodes.Ldarg_0);

            //Now that we have it all set up call the actual method that we want to call for the binding
            lv_EventIl.EmitCall(OpCodes.Call, MethodToInvoke, null);

            lv_EventIl.Emit(OpCodes.Pop);
            lv_EventIl.Emit(OpCodes.Ret);

            //create a delegate from the dynamic method
            return handler.CreateDelegate(EventHandlerType, MethodInvoker);
        }

    }
}
