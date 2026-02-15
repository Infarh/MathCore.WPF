// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
using MathCore.WPF;

namespace Microsoft.Win32;

public static class OpenFileDialogExtensions
{
    extension(OpenFileDialog)
    {
        public static FileDialogEx Open(string Title) => FileDialogEx.OpenFile(Title);
    }

    extension(OpenFileDialog dialog)
    {
        public string? GetFileName() => dialog.ShowDialog() == true ? dialog.FileName : null;

        public string? GetFileName(System.Windows.Window owner) => dialog.ShowDialog(owner) == true ? dialog.FileName : null;

        public System.IO.FileInfo? GetFileInfo()
        {
            var file = dialog.GetFileName();
            return string.IsNullOrWhiteSpace(file) ? null : new(file);
        }

        public System.IO.FileInfo? GetFileInfo(System.Windows.Window owner)
        {
            var file = dialog.GetFileName(owner);
            return string.IsNullOrWhiteSpace(file) ? null : new(file);
        }
    }
}