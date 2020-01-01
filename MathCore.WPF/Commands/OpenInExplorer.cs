using System.IO;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Commands
{
    [MarkupExtensionReturnType(typeof(OpenInExplorer))]
    public class OpenInExplorer : Command
    {
        public override bool CanExecute(object? parameter)
        {
            switch (parameter)
            {
                default:
                    return false;

                case string path when Directory.Exists(path):
                case DirectoryInfo dir when dir.Exists:
                case DriveInfo drive when drive.IsReady:
                    return true;
            }
        }

        public override void Execute(object? parameter)
        {
            switch (parameter)
            {
                default: return;
                case string path when Directory.Exists(path): Execute(new DirectoryInfo(path)); break;
                case DirectoryInfo dir when dir.Exists: dir.OpenInFileExplorer(); break;
                case DriveInfo drive when drive.IsReady: Execute(drive.RootDirectory); break;
            }
        }
    }
}