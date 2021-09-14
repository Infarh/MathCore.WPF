using System.IO;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace Microsoft.Win32
{
    public static class OpenFileDialogExtensions
    {
        public static string? GetFileName(this OpenFileDialog d) => d.ShowDialog() == true ? d.FileName : null;

        public static FileInfo? GetFileInfo(this OpenFileDialog dialog)
        {
            var file = dialog.GetFileName();
            return string.IsNullOrWhiteSpace(file) ? null : new FileInfo(file);
        }
    }
}