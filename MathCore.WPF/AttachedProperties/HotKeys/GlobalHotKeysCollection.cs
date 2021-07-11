using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace MathCore.WPF
{
    public class GlobalHotKeysCollection : FreezableCollection<GlobalHotKeyBinding>, IList
    {
        private readonly Window? _HostWindow;

        public GlobalHotKeysCollection(Window? HostWindow)
        {
            _HostWindow = HostWindow;
            ((INotifyCollectionChanged)this).CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems?.Cast<GlobalHotKeyBinding>() is { } added)
                        foreach (var item in e.NewItems.Cast<GlobalHotKeyBinding>()) item.SetHost(_HostWindow);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems?.Cast<GlobalHotKeyBinding>() is { } removed)
                        foreach (var item in removed) item.SetHost(null);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems?.Cast<GlobalHotKeyBinding>() is { } @new)
                        foreach (var item in @new) item.SetHost(_HostWindow);
                    if (e.OldItems?.Cast<GlobalHotKeyBinding>() is { } old)
                        foreach (var item in old) item.SetHost(null);
                    break;
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            var collection = new GlobalHotKeysCollection(_HostWindow);
            //collection.AddItems(this.Select(f => (GlobalHotKeyBinding)f.Clone()));
            return collection;
        }
    }
}