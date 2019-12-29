using System.IO;
using System.Windows.Input;

namespace MathCore.WPF.Commands
{
    public static class FileCommands
    {
        public static ICommand ShowInExplorer => new LambdaCommand(OnShowInExplorerExecuted, OnShowInExplorerCanExecuteCheck);

        private static bool OnShowInExplorerCanExecuteCheck(object file) => File.Exists(file as string) || (file as FileInfo)?.Exists == true;

        private static void OnShowInExplorerExecuted(object file)
        {
            if(file is string) file = new FileInfo((string)file);
            (file as FileInfo)?.ShowInExplorer();
        }
    }
}