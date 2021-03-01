using System.IO;
using System.Windows.Markup;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Commands
{
    [MarkupExtensionReturnType(typeof(ShowInExplorer))]
    public class ShowInExplorer : Command
    {
        public override bool CanExecute(object? parameter) => parameter switch
        {
            string path when File.Exists(path) || Directory.Exists(path) => true,
            FileSystemInfo {Exists: true} => true,
            DriveInfo {IsReady: true} => true,
            _ => false
        };

        public override void Execute(object? parameter)
        {
            switch (parameter)
            {
                default: return;
                case string path when File.Exists(path): Execute(new FileInfo(path)); break;
                case string path when Directory.Exists(path): Execute(new DirectoryInfo(path)); break;
                case FileSystemInfo { Exists: true } file_or_dir: file_or_dir.ShowInFileExplorer(); break;
                case DriveInfo { IsReady: true } drive: Execute(drive.RootDirectory); break;
            }
        }
    }
}