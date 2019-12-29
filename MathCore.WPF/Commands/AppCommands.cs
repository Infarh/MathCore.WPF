using System.Windows;
using System.Windows.Input;

namespace MathCore.WPF.Commands
{
    public static class AppCommands
    {
        public static ICommand Close { get; } = new LambdaCommand(OnCloseCommandExecute);

        private static void OnCloseCommandExecute(object Obj)
        {
            var app = Obj as Application ?? Application.Current;
            if(Obj is int)
                app.Shutdown((int)Obj);
            else
                app.Shutdown();
        }
    }
}