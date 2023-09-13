using System.IO;
using System.Text;

using Microsoft.Win32;

namespace MathCore.WPF;

public readonly ref struct FileDialogEx
{
    public readonly struct FileFilterItem : IEquatable<FileFilterItem>
    {
        public string Title { get; init; }

        public IEnumerable<string>? Values { get; init; }

        public FileFilterItem(string Title, params string[] Value)
        {
            this.Title = Title;
            if (Value is { Length: > 0 })
                Values = Value.Distinct();
        }

        internal void AppendTo(StringBuilder filter)
        {
            filter.Append(Title).Append(" (");

            if (Values is null)
            {
                filter.Append("*.*)|*.*|");
                return;
            }

            AppendFilterText(filter, Values);
            filter.Append(")|");
            AppendFilterText(filter, Values);
            static void AppendFilterText(StringBuilder str, IEnumerable<string> vvv)
            {
                var any = false;
                foreach (var s in vvv)
                {
                    any = true;
                    str.Append(s).Append(';');
                }

                if (any)
                    str.Length--;
                else
                    str.Append("*.*");
            }

            filter.Append('|');
        }

        public bool Equals(FileFilterItem other) => Title == other.Title && Values.SequenceEqual(other.Values, StringComparer.OrdinalIgnoreCase);
    }

    public static FileDialogEx OpenFile() => new FileDialogEx { IsSaveFileDialog = false };
    public static FileDialogEx OpenFile(string Title) => new FileDialogEx { IsSaveFileDialog = false, Title = Title };

    public static FileDialogEx CreateFile() => new FileDialogEx { IsSaveFileDialog = true };
    public static FileDialogEx CreateFile(string Title) => new FileDialogEx { IsSaveFileDialog = true, Title = Title };

    public static FileDialogEx New() => new();
    public static FileDialogEx New(string Title) => new FileDialogEx { Title = Title };

    public bool IsSaveFileDialog { get; init; }

    public string Title { get; init; }

    public bool? RestoreDirectory { get; init; }

    public string? InitialDirectory { get; init; }

    public bool? CheckFileExists { get; init; }

    public bool? ReadOnlyChecked { get; init; }

    public FileDialogEx() { }

    public FileDialogEx SetTitle(string Title) => this with { Title = Title };

    public IEnumerable<FileFilterItem>? Filter { get; init; }

#if NET5_0_OR_GREATER
    public FileDialogEx AddFilter(string Name, params string[] Ext) => Filter is { } filter
       ? this with { Filter = filter.Append(new(Name, Ext)) }
       : this with { Filter = new FileFilterItem[] { new(Name, Ext) } };
#else
   public FileDialogEx AddFilter(string Name, params string[] Ext) => Filter is { } filter
       ? this with { Filter = filter.AppendLast(new FileFilterItem(Name, Ext)) }
       : this with { Filter = new FileFilterItem[] { new(Name, Ext) } };
#endif

    public FileDialogEx AddFilterAllFiles() => Filter is null || Filter.Last().Title != "Все файлы" 
        ? AddFilter("Все файлы", "*.*") 
        : this;

    public OpenFileDialog CreateOpenFileDialog()
    {
        var dialog = new OpenFileDialog();

        if (Title is not null)
            dialog.Title = Title;

        if (Filter is { } filter)
        {
            var filter_str = new StringBuilder();

            foreach (var item in Filter)
                item.AppendTo(filter_str);

            filter_str.Length--;

            dialog.Filter = filter_str.ToString();
        }

        if (RestoreDirectory is { } restore_directory)
            dialog.RestoreDirectory = restore_directory;

        if (InitialDirectory is { Length: > 0 } initial_directory)
            dialog.InitialDirectory = initial_directory;

        if (CheckFileExists is { } check_file_exists)
            dialog.CheckFileExists = check_file_exists;

        if (ReadOnlyChecked is { } read_only_checked)
            dialog.ReadOnlyChecked = read_only_checked;

        return dialog;
    }

    public SaveFileDialog CreateSaveFileDialog()
    {
        var dialog = new SaveFileDialog();

        if (Title is not null)
            dialog.Title = Title;

        if (Filter is { } filter)
        {
            var filter_str = new StringBuilder();

            foreach (var item in Filter)
                item.AppendTo(filter_str);

            filter_str.Length--;

            dialog.Filter = filter_str.ToString();
        }

        if (RestoreDirectory is { } restore_directory)
            dialog.RestoreDirectory = restore_directory;

        if (InitialDirectory is { Length: > 0 } initial_directory)
            dialog.InitialDirectory = initial_directory;

        if (CheckFileExists is { } check_file_exists)
            dialog.CheckFileExists = check_file_exists;

        return dialog;
    }

    public string? GetFileName()
    {
        FileDialog dialog = IsSaveFileDialog ? CreateSaveFileDialog() : CreateOpenFileDialog();

        if (dialog.ShowDialog() != true)
            return null;

        return dialog.FileName;
    }

    public FileInfo? GetFileInfo() => GetFileName() is { } file_name ? new(file_name) : null;

    public FileStream? OpenFileStream() => GetFileName() is { } file_name
        ? IsSaveFileDialog
            ? File.Create(file_name)
            : File.OpenRead(file_name)
        : null;
}
