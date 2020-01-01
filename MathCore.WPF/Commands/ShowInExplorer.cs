using System.IO;
using System.Windows.Markup;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Commands
{
    [MarkupExtensionReturnType(typeof(ShowInExplorer))]
    public class ShowInExplorer : Command
    {
        public override bool CanExecute(object? parameter)
        {
            switch (parameter)
            {
                default:
                    return false;

                case string path when File.Exists(path) || Directory.Exists(path):
                case FileSystemInfo file_or_dir when file_or_dir.Exists:
                case DriveInfo drive when drive.IsReady:
                    return true;
            }
        }

        public override void Execute(object? parameter)
        {
            switch (parameter)
            {
                default: return;
                case string path when File.Exists(path): Execute(new FileInfo(path)); break;
                case string path when Directory.Exists(path): Execute(new DirectoryInfo(path)); break;
                case FileSystemInfo file_or_dir when file_or_dir.Exists: file_or_dir.ShowInFileExplorer(); break;
                case DriveInfo drive when drive.IsReady: Execute(drive.RootDirectory); break;
            }
        }
    }
}