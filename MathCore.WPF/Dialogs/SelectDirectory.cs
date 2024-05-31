using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Interop;

//using System.Windows.Forms;

namespace MathCore.WPF.Dialogs;

/// <summary>Диалог выбора директории</summary>
public class SelectDirectory : Dialog
{
    #region Dependency properties

    #region SelectedDirectory : DirectoryInfo - Выбранная директория

    /// <summary>Выбранная директория</summary>
    public static readonly DependencyProperty SelectedDirectoryProperty =
        DependencyProperty.Register(
            nameof(SelectedDirectory),
            typeof(DirectoryInfo),
            typeof(SelectDirectory),
            new(default(DirectoryInfo?), (d, e) => d.SetValue(SelectedDirectoryPathProperty, ((DirectoryInfo?)e.NewValue)?.FullName)));

    /// <summary>Выбранная директория</summary>
    public DirectoryInfo? SelectedDirectory
    {
        get => (DirectoryInfo?)GetValue(SelectedDirectoryProperty);
        set => SetValue(SelectedDirectoryProperty, value);
    }

    #endregion

    #region SelectedDirectoryPath : string - Выбранный путь к каталогу

    /// <summary>Выбранный путь к каталогу</summary>
    public static readonly DependencyProperty SelectedDirectoryPathProperty =
        DependencyProperty.Register(
            nameof(SelectedDirectoryPath),
            typeof(string),
            typeof(SelectDirectory),
            new(default(string), OnSelectedDirectoryPathChanged));

    private static void OnSelectedDirectoryPathChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        var path    = (string)E.NewValue;
        var old_dir = (DirectoryInfo?)D.GetValue(SelectedDirectoryProperty);
        if (old_dir is null || !string.Equals(old_dir.FullName, path, StringComparison.Ordinal))
            D.SetValue(SelectedDirectoryProperty, path is { Length: > 0 } ? new DirectoryInfo(path) : null);
    }

    /// <summary>Выбранный путь к каталогу</summary>
    //[Category("")]
    [Description("Выбранный путь к каталогу")]
    public string SelectedDirectoryPath { get => (string)GetValue(SelectedDirectoryPathProperty); set => SetValue(SelectedDirectoryPathProperty, value); }

    #endregion

    #region ShowNewFolderButton property : bool

    public static readonly DependencyProperty ShowNewFolderButtonProperty =
        DependencyProperty.Register(
            nameof(ShowNewFolderButton),
            typeof(bool),
            typeof(SelectDirectory),
            new(default(bool)));

    public bool ShowNewFolderButton
    {
        get => (bool)GetValue(ShowNewFolderButtonProperty);
        set => SetValue(ShowNewFolderButtonProperty, value);
    }

    #endregion

    #region RootFolder property : Environment.SpecialFloder default = Desktop

    public static readonly DependencyProperty RootFolderProperty =
        DependencyProperty.Register(
            nameof(RootFolder),
            typeof(Environment.SpecialFolder),
            typeof(SelectDirectory),
            new(default(Environment.SpecialFolder)));

    public Environment.SpecialFolder RootFolder
    {
        get => (Environment.SpecialFolder)GetValue(RootFolderProperty);
        set => SetValue(RootFolderProperty, value);
    }

    #endregion

    #region OkButtonText : string - Текст кнопки Ok

    /// <summary>Текст кнопки Ok</summary>
    public static readonly DependencyProperty OkButtonTextProperty =
        DependencyProperty.Register(
            nameof(OkButtonText),
            typeof(string),
            typeof(SelectDirectory),
            new("Ok"));

    /// <summary>Текст кнопки Ok</summary>
    //[Category("")]
    [Description("Текст кнопки Ok")]
    public string OkButtonText { get => (string)GetValue(OkButtonTextProperty); set => SetValue(OkButtonTextProperty, value); }

    #endregion

    #region FileNameCaption : string - Текст подписи имени файла

    /// <summary>Текст подписи имени файла</summary>
    public static readonly DependencyProperty FileNameCaptionProperty =
        DependencyProperty.Register(
            nameof(FileNameCaption),
            typeof(string),
            typeof(SelectDirectory),
            new(default(string)));

    /// <summary>Текст подписи имени файла</summary>
    //[Category("")]
    [Description("Текст подписи имени файла")]
    public string FileNameCaption { get => (string)GetValue(FileNameCaptionProperty); set => SetValue(FileNameCaptionProperty, value); }

    #endregion

    #region ForceFileSystem : bool - Принудительный выбор файловой системы

    /// <summary>Принудительный выбор файловой системы</summary>
    public static readonly DependencyProperty ForceFileSystemProperty =
        DependencyProperty.Register(
            nameof(ForceFileSystem),
            typeof(bool),
            typeof(SelectDirectory),
            new(default(bool)));

    /// <summary>Принудительный выбор файловой системы</summary>
    //[Category("")]
    [Description("Принудительный выбор файловой системы")]
    public bool ForceFileSystem { get => (bool)GetValue(ForceFileSystemProperty); set => SetValue(ForceFileSystemProperty, value); }

    #endregion

    #endregion

    protected override void OpenDialog(object? p)
    {
        //var dialog = new FolderBrowserDialog();
        //var description = Title;
        //if (description != null)
        //    dialog.Description = description;

        //var selectedpath = p as string ?? SelectedDirectory?.FullName;
        //if (selectedpath != null)
        //    dialog.SelectedDirectory = selectedpath;

        //dialog.ShowNewFolderButton = ShowNewFolderButton;
        //dialog.RootFolder = RootFolder;

        //var result = dialog.ShowDialog();
        //if (result == DialogResult.OK || UpdateIfResultFalse)
        //    SelectedDirectory = new DirectoryInfo(dialog.SelectedDirectory);

        ShowDialog();
    }

    #region pInvoke
        
    public bool? ShowDialog(Window? owner = null, bool ThrowOnError = false)
    {
        owner ??= Application.Current.MainWindow;
        return ShowDialog(owner != null ? new WindowInteropHelper(owner).Handle : IntPtr.Zero, ThrowOnError);
    }

    // for all .NET
    public virtual bool? ShowDialog(IntPtr OwnerHandle, bool ThrowOnError = false)
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        var dialog     = (IFileOpenDialog)new FileOpenDialog();
        var input_path = SelectedDirectoryPath;
        if (!string.IsNullOrEmpty(input_path))
        {
            if (CheckDialogResult(SHCreateItemFromParsingName(input_path, null, typeof(IShellItem).GUID, out var item), ThrowOnError) != 0)
                return null;

            dialog.SetFolder(item);
        }

        var options = FOS.PickFolders;
        options = (FOS)SetOptions((int)options);
        dialog.SetOptions(options);

        if (Title is { Length: > 0 } title)
            dialog.SetTitle(title);

        if (OkButtonText is { Length: > 0 } ok_button_text)
            dialog.SetOkButtonLabel(ok_button_text);

        if (FileNameCaption is { Length: > 0 } file_name_caption)
            dialog.SetFileName(file_name_caption);

        if (OwnerHandle == IntPtr.Zero)
        {
            OwnerHandle = Process.GetCurrentProcess().MainWindowHandle;
            if (OwnerHandle == IntPtr.Zero) 
                OwnerHandle = GetDesktopWindow();
        }

        var dialog_result = dialog.Show(OwnerHandle);
        if (dialog_result == __DialogResultCancel)
            return null;

        if (CheckDialogResult(dialog_result, ThrowOnError) != 0)
            return null;

        if (CheckDialogResult(dialog.GetResult(out var result), ThrowOnError) != 0)
            return null;

        if (CheckDialogResult(result.GetDisplayName(SIGDN.DeskTopAbsoluteParsing, out var path), ThrowOnError) != 0)
            return null;

        var result_path = path;
        if (CheckDialogResult(result.GetDisplayName(SIGDN.DeskTopAbsoluteEditing, out path), false) == 0)
            result_path = path;

        SelectedDirectoryPath = result_path;
        return true;
    }

    private int SetOptions(int options)
    {
        if (ForceFileSystem) 
            options |= (int)FOS.ForceFileSystem;
        return options;
    }

    private static int CheckDialogResult(int Result, bool ThrowOnError)
    {
        if (Result == 0) return Result;
        if (ThrowOnError)
            Marshal.ThrowExceptionForHR(Result);
        return Result;
    }

    [DllImport("shell32")]
    private static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx? pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IShellItem ppv);

    [DllImport("user32")]
    private static extern IntPtr GetDesktopWindow();

    private const int __DialogResultCancel = unchecked((int)0x800704C7);

    [ComImport, Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")] // CLSID_FileOpenDialog
    private class FileOpenDialog
    {
    }

    [ComImport, Guid("42f85136-db7e-439c-85f1-e4075d135fc8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IFileOpenDialog
    {
        [PreserveSig] int Show(IntPtr parent); // IModalWindow
        [PreserveSig] int SetFileTypes();      // not fully defined
        [PreserveSig] int SetFileTypeIndex(int iFileType);
        [PreserveSig] int GetFileTypeIndex(out int piFileType);
        [PreserveSig] int Advise(); // not fully defined
        [PreserveSig] int Unadvise();
        [PreserveSig] int SetOptions(FOS fos);
        [PreserveSig] int GetOptions(out FOS pfos);
        [PreserveSig] int SetDefaultFolder(IShellItem psi);
        [PreserveSig] int SetFolder(IShellItem psi);
        [PreserveSig] int GetFolder(out IShellItem ppsi);
        [PreserveSig] int GetCurrentSelection(out IShellItem ppsi);
        [PreserveSig] int SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        [PreserveSig] int GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
        [PreserveSig] int SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
        [PreserveSig] int SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);
        [PreserveSig] int SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
        [PreserveSig] int GetResult(out IShellItem ppsi);
        [PreserveSig] int AddPlace(IShellItem psi, int alignment);
        [PreserveSig] int SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
        [PreserveSig] int Close(int hr);
        [PreserveSig] int SetClientGuid(); // not fully defined
        [PreserveSig] int ClearClientData();
        [PreserveSig] int SetFilter([MarshalAs(UnmanagedType.IUnknown)] object pFilter);
        [PreserveSig] int GetResults([MarshalAs(UnmanagedType.IUnknown)] out object ppenum);
        [PreserveSig] int GetSelectedItems([MarshalAs(UnmanagedType.IUnknown)] out object ppsai);
    }

    [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellItem
    {
        [PreserveSig] int BindToHandler(); // not fully defined
        [PreserveSig] int GetParent();     // not fully defined
        [PreserveSig] int GetDisplayName(SIGDN SigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string Name);
        [PreserveSig] int GetAttributes(); // not fully defined
        [PreserveSig] int Compare();       // not fully defined
    }

#pragma warning disable CA1712 // Do not prefix enum values with type name
    private enum SIGDN : uint
    {
        DeskTopAbsoluteEditing = 0x8004c000,
        DeskTopAbsoluteParsing = 0x80028000,
        FileSysPath = 0x80058000,
        NormalDisplay = 0,
        ParentRelative = 0x80080001,
        ParentRelativeEditing = 0x80031001,
        ParentRelativeForAddressBar = 0x8007c001,
        ParentRelativeParsing = 0x80018001,
        Url = 0x80068000
    }

    [Flags]
    private enum FOS
    {
        OverWritePrompt = 0x2,
        StrictFileTypes = 0x4,
        NoChangeDir = 0x8,
        PickFolders = 0x20,
        ForceFileSystem = 0x40,
        AllNonStorageItems = 0x80,
        NoValidate = 0x100,
        AllowMultiSelect = 0x200,
        PathMustExist = 0x800,
        FileMustExist = 0x1000,
        CratePrompt = 0x2000,
        ShareAware = 0x4000,
        NoReadonlyReturn = 0x8000,
        NoTestFileCreate = 0x10000,
        HideMRUPlaces = 0x20000,
        HidePinnedPlaces = 0x40000,
        NoDereferenceLinks = 0x100000,
        OkButtonNeedsInteraction = 0x200000,
        DontAddToRecent = 0x2000000,
        ForceShowHidden = 0x10000000,
        DefaultNoMiniMode = 0x20000000,
        ForcePreviewWPaneOn = 0x40000000,
        SupportStreamAbleItems = unchecked((int)0x80000000)
    }

    #endregion
}