using System.Windows;
using System.Windows.Input;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Commands
{
    public static class WindowsCommands
    {
        public static ICommand Close { get; } = new LambdaCommand(p => (p as Window)?.Close(), p => p is Window);
        public static ICommand Hide { get; } = new LambdaCommand(p => (p as Window)?.Hide(), p => (p as Window)?.IsVisible == true);
        public static ICommand Show { get; } = new LambdaCommand(p => (p as Window)?.Show(), p => (p as Window)?.IsVisible == false);
        public static ICommand BringIntoView { get; } = new LambdaCommand(p => (p as Window)?.BringIntoView(), p => p is Window);
    }
}