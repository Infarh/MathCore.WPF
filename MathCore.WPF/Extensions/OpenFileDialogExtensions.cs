// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace Microsoft.Win32;

public static class OpenFileDialogExtensions
{
    public static string? GetFileName(this OpenFileDialog d) => d.ShowDialog() == true ? d.FileName : null;
    public static string? GetFileName(this OpenFileDialog d, System.Windows.Window owner) => d.ShowDialog(owner) == true ? d.FileName : null;

    public static System.IO.FileInfo? GetFileInfo(this OpenFileDialog dialog)
    {
        var file = dialog.GetFileName();
        return string.IsNullOrWhiteSpace(file) ? null : new(file);
    }

    public static System.IO.FileInfo? GetFileInfo(this OpenFileDialog dialog, System.Windows.Window owner)
    {
        var file = dialog.GetFileName(owner);
        return string.IsNullOrWhiteSpace(file) ? null : new(file);
    }
}