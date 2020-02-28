using System.IO;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace Microsoft.Win32
{
    public static class OpenFileDialogExtensions
    {
        [CanBeNull] public static string? GetFileName([NotNull] this OpenFileDialog d) => d.ShowDialog() == true ? d.FileName : null;

        [CanBeNull]
        public static FileInfo? GetFileInfo([NotNull] this OpenFileDialog dialog)
        {
            var file = dialog.GetFileName();
            return string.IsNullOrWhiteSpace(file) ? null : new FileInfo(file);
        }
    }
}