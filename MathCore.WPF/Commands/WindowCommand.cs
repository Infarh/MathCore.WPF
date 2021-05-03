using System.Linq;
using System.Windows;

namespace MathCore.WPF.Commands
{
    public abstract class WindowCommand : Command
    {
        protected virtual Window? GetWindow(object? parameter) =>
            parameter as Window
             ?? RootObject as Window
             ?? Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsFocused)
             ?? Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);

        public override bool CanExecute(object? obj) => obj is Window || GetWindow(obj) is not null;

        public override void Execute(object? parameter)
        {
            var window = GetWindow(parameter);
            if (window is null || !CanExecute(window)) return;
            Execute(window);
        }

        protected abstract void Execute(Window? window);
    }
}