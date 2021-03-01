using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using MathCore.Annotations;
using MathCore.IoC;

namespace MathCore.WPF.IoC
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class ViewSystem : IViewSystem
    {
        private readonly IServiceManager _ServiceManager;
        private readonly Dictionary<Type, Type> _ViewModelMap = new();
        private readonly Dictionary<Type, Type> _ViewMap = new();
        private readonly List<Window> _Views = new();

        public ViewSystem(IServiceManager ServiceManager) => _ServiceManager = ServiceManager;

        public void Register<TObject, TViewModel>() where TViewModel : class => _ViewModelMap[typeof(TObject)] = typeof(TViewModel);

        public void RegisterViewModel<TViewModel, TView>() where TViewModel : class where TView : Window
        {
            _ViewMap[typeof(TViewModel)] = typeof(TView);
            _ServiceManager.RegisterSingleCall<TView>();
        }

        [CanBeNull]
        public Window? CreateView([CanBeNull] object Model)
        {
            if (Model is null || !_ViewMap.TryGetValue(Model.GetType(), out var window_type)) return null;
            if (_ServiceManager.Get(window_type) is not Window window) return null;
            window.DataContext = Model;
            window.Loaded += OnViewLoaded;
            return window;
        }

        public Window CreateView<TWindow>() where TWindow : Window
        {
            var window_type = typeof(TWindow);
            Type? model_type = null;
            foreach (var (t, w) in _ViewModelMap)
            {
                if (w != window_type) continue;
                model_type = t;
                break;
            }
            if (model_type is null)
                throw new ApplicationException("Представление не зарегистрировано в системе визаулизации");
            var window = (Window)_ServiceManager.Get(window_type);
            var model = _ServiceManager.Get(model_type);
            window.DataContext = model;
            return window;
        }

        private void OnViewLoaded(object Sender, RoutedEventArgs E)
        {
            if (Sender is not Window window) return;
            window.Loaded -= OnViewLoaded;
            window.Closed += OnViewClosed;
            _Views.Add(window);
        }

        private void OnViewClosed(object? Sender, EventArgs E)
        {
            if (Sender is not Window window) return;
            window.Closed -= OnViewClosed;
            _Views.Remove(window);
        }

        [CanBeNull]
        private Window? GetView([CanBeNull] object? obj)
        {
            if (obj is null) return null;
            var obj_type = obj.GetType();
            if (_ViewMap.ContainsKey(obj_type)) return CreateView(obj);
            if (!_ViewModelMap.TryGetValue(obj_type, out var view_model_type)) return null;
            var view_model = _ServiceManager.Get(view_model_type);
            return view_model is null ? null : CreateView(view_model);
        }

        [CanBeNull]
        public Window? View(object obj)
        {
            var view = GetView(obj);
            view?.Show();
            return view;
        }

        public bool? ViewDialog(object obj) => GetView(obj)?.ShowDialog();

        #region IEnumerator<Window>

        IEnumerator<Window> IEnumerable<Window>.GetEnumerator() => _Views.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Views).GetEnumerator();

        #endregion
    }
}
