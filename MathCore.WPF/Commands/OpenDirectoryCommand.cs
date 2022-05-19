using System.IO;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Commands;

[MarkupExtensionReturnType(typeof(OpenDirectoryCommand))]
public class OpenDirectoryCommand : LambdaCommand
{
    public OpenDirectoryCommand() : base(OnOpenDirectoryExecuted, OnOpenDirectoryCanExecuteCheck) { }

    private static bool OnOpenDirectoryCanExecuteCheck(object? path) =>
        File.Exists(path as string) 
        || (path as FileInfo)?.Exists == true
        || Directory.Exists(path as string) 
        || (path as DirectoryInfo)?.Exists == true;

    private static void OnOpenDirectoryExecuted(object? path)
    {
        switch (path)
        {
            case string path_str:
                var dir = new DirectoryInfo(path_str);
                if (dir.Exists) dir.OpenInFileExplorer();
                else
                {
                    var file = new FileInfo(path_str);
                    if (file.Exists) file.ShowInExplorer();
                }
                break;
            case DirectoryInfo directory:
                if (directory.Exists) directory.OpenInFileExplorer();
                if (directory.Parent is null) break;
                directory = directory.Parent;
                if (directory.Exists) directory.OpenInFileExplorer();
                break;
            case FileInfo file:
                if (file.Exists) file.ShowInExplorer();
                break;
        }
    }

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider service) => new OpenDirectoryCommand();
}