using System.IO;
using System.Windows.Input;

// ReSharper disable UnusedType.Global

// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Commands;

public static class FileCommands
{
    public static ICommand ShowInExplorer => new LambdaCommand(OnShowInExplorerExecuted, OnShowInExplorerCanExecuteCheck);

    private static bool OnShowInExplorerCanExecuteCheck(object? file) => File.Exists(file as string) || file is FileInfo { Exists: true };

    private static void OnShowInExplorerExecuted(object? file)
    {
        if(file is string str) file = new FileInfo(str);
        (file as FileInfo)?.ShowInExplorer();
    }
}