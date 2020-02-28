using System.Windows;
using System.Windows.Input;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Commands
{
    public static class AppCommands
    {
        public static ICommand Close { get; } = new LambdaCommand(OnCloseCommandExecute);

        private static void OnCloseCommandExecute(object? Obj)
        {
            var app = Obj as Application ?? Application.Current;
            if(Obj is int code)
                app.Shutdown(code);
            else
                app.Shutdown();
        }
    }
}