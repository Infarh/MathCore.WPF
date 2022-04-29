using System.IO;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Commands;

[MarkupExtensionReturnType(typeof(OpenInExplorer))]
public class OpenInExplorer : Command
{
    public override bool CanExecute(object? parameter) => parameter switch
    {
        string path when Directory.Exists(path) => true,
        DirectoryInfo {Exists: true} dir => true,
        DriveInfo {IsReady: true} drive => true,
        _ => false
    };

    public override void Execute(object? parameter)
    {
        while (true)
        {
            switch (parameter)
            {
                default: return;
                case string path when Directory.Exists(path):
                    parameter = new DirectoryInfo(path);
                    continue;
                case DirectoryInfo {Exists: true} dir:
                    _ = dir.OpenInFileExplorer();
                    break;
                case DriveInfo {IsReady: true} drive:
                    parameter = drive.RootDirectory;
                    continue;
            }

            break;
        }
    }
}