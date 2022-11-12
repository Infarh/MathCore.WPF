using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Threading;
// ReSharper disable UnusedType.Global

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

public class DispatcheredObservableLinkedList<T> : ObservableLinkedList<T>
{
    protected override void OnCollectionChanged(NotifyCollectionChangedEventHandler? Handler, NotifyCollectionChangedEventArgs e)
    {
        if (e is null) throw new ArgumentNullException(nameof(e));
        if (Handler is null) return;
        var invocation_list = Handler.GetInvocationList();
        var args            = new object[] { this, e };
        foreach (var d in invocation_list)
        {
            var o = d.Target;
            switch (o)
            {
                case ISynchronizeInvoke {InvokeRequired: true} synchronize_invoke:
                    synchronize_invoke.Invoke(d, args);
                    break;
                case DispatcherObject dispatcher_obj when !dispatcher_obj.CheckAccess() && dispatcher_obj.Dispatcher != null:
                    dispatcher_obj.Dispatcher.Invoke(d, args);
                    break;
                default:
                    d.DynamicInvoke(args);
                    break;
            }
        }
    }
}