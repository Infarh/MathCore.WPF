using System.Windows.Interop;
using System.Windows.Media;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Windows
{
    public static class WindowExtensions
    {
        public static IntPtr GetWindowHandle(this Window window) => new WindowInteropHelper(window).Handle;

        public static void ForWindowFromChild(this object ChildDependencyObject, Action<Window> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            var element = ChildDependencyObject as DependencyObject;
            while (element != null)
            {
                element = VisualTreeHelper.GetParent(element);
                if(element is not Window window) continue;
                action(window); 
                break;
            }
        }

        public static void ForWindowFromTemplate(this object TemplateFrameworkElement, Action<Window> action)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (((FrameworkElement)TemplateFrameworkElement)?.TemplatedParent is Window window) action(window);
        }

        public static void AddHook(this Window window, HwndSourceHook WndProc)
        {
            if (window.IsLoaded)
            {
                var hwnd = window.GetWindowHandle();
                var source = HwndSource.FromHwnd(hwnd) ?? throw new InvalidOperationException();
                source.AddHook(WndProc);
            }
            else
                window.SourceInitialized += (_,_) => window.AddHook(WndProc);
        }

        public static void RemoveHook(this Window window, HwndSourceHook WndProc)
        {
            if (!window.IsLoaded) return;
            var hwnd = window.GetWindowHandle();
            var source = HwndSource.FromHwnd(hwnd) ?? throw new InvalidOperationException();
            source.RemoveHook(WndProc);
        }
    }
}