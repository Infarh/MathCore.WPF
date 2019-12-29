using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace MathCore.WPF.Dialogs
{
    public class OpenFile : Dialog
    {
        #region Dependency properties

        #region SelectedFile dependency property : FileInfo

        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register(
                nameof(SelectedFile),
                typeof(FileInfo),
                typeof(OpenFile),
                new PropertyMetadata(default(FileInfo)), v => v == null || v is FileInfo);

        public FileInfo SelectedFile
        {
            get => (FileInfo)GetValue(SelectedFileProperty);
            set => SetValue(SelectedFileProperty, value);
        }

        #endregion

        #region SelectedFiles readonly dependency property : FileInfo[]

        private static readonly DependencyPropertyKey SelectedFilesPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(SelectedFiles),
                typeof(FileInfo[]),
                typeof(OpenFile),
                new FrameworkPropertyMetadata(default(FileInfo[])));

        public static readonly DependencyProperty SelectedFilesProperty = SelectedFilesPropertyKey.DependencyProperty;

        public FileInfo[] SelectedFiles
        {
            get => (FileInfo[])GetValue(SelectedFilesProperty);
            private set => SetValue(SelectedFilesPropertyKey, value);
        }

        #endregion

        #region Filter dependency property : string

        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register(
                nameof(Filter),
                typeof(string),
                typeof(OpenFile),
                new PropertyMetadata("*.*|*.*"), v => v == null || v is string);

        public string Filter
        {
            get => (string)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        #endregion

        #region Result readonly dependency property : bool?

        private static readonly DependencyPropertyKey ResultPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(Result),
                typeof(bool?),
                typeof(OpenFile),
                new FrameworkPropertyMetadata(default(bool?)), v => v == null || v is bool?);

        public static readonly DependencyProperty ResultProperty = ResultPropertyKey.DependencyProperty;

        public bool? Result
        {
            get => (bool?)GetValue(ResultProperty);
            private set => SetValue(ResultPropertyKey, value);
        }

        #endregion

        #region Multiselect dependency property : bool

        public static readonly DependencyProperty MultiselectProperty =
            DependencyProperty.Register(
                nameof(Multiselect),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)), v => v is bool);

        public bool Multiselect
        {
            get => (bool)GetValue(MultiselectProperty);
            set => SetValue(MultiselectProperty, value);
        }

        #endregion

        #region AddExtension dependency property : bool

        public static readonly DependencyProperty AddExtensionProperty =
            DependencyProperty.Register(
                nameof(AddExtension),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)), v => v is bool);

        public bool AddExtension
        {
            get => (bool)GetValue(AddExtensionProperty);
            set => SetValue(AddExtensionProperty, value);
        }

        #endregion

        #region CheckFileExists dependency property : bool

        public static readonly DependencyProperty CheckFileExistsProperty =
            DependencyProperty.Register(
                nameof(CheckFileExists),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)), v => v is bool);

        public bool CheckFileExists
        {
            get => (bool)GetValue(CheckFileExistsProperty);
            set => SetValue(CheckFileExistsProperty, value);
        }

        #endregion

        #region CheckPathExists dependency property : bool

        public static readonly DependencyProperty CheckPathExistsProperty =
            DependencyProperty.Register(
                nameof(CheckPathExists),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)), v => v is bool);

        public bool CheckPathExists
        {
            get => (bool)GetValue(CheckPathExistsProperty);
            set => SetValue(CheckPathExistsProperty, value);
        }

        #endregion

        #region DefaultExt dependency property : string

        public static readonly DependencyProperty DefaultExtProperty =
            DependencyProperty.Register(
                nameof(DefaultExt),
                typeof(string),
                typeof(OpenFile),
                new PropertyMetadata(default(string)), v => v == null || v is string);

        public string DefaultExt
        {
            get => (string)GetValue(DefaultExtProperty);
            set => SetValue(DefaultExtProperty, value);
        }

        #endregion

        #region DereferenceLinks dependency property : bool

        public static readonly DependencyProperty DereferenceLinksProperty =
            DependencyProperty.Register(
                nameof(DereferenceLinks),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)), v => v is bool);

        public bool DereferenceLinks
        {
            get => (bool)GetValue(DereferenceLinksProperty);
            set => SetValue(DereferenceLinksProperty, value);
        }

        #endregion

        #region FilterIndex dependency property : int

        public static readonly DependencyProperty FilterIndexProperty =
            DependencyProperty.Register(
                nameof(FilterIndex),
                typeof(int),
                typeof(OpenFile),
                new PropertyMetadata(default(int)), v => v is int && (int)v >= 0);

        public int FilterIndex
        {
            get => (int)GetValue(FilterIndexProperty);
            set => SetValue(FilterIndexProperty, value);
        }

        #endregion

        #region InitialDirectory dependency property : string

        public static readonly DependencyProperty InitialDirectoryProperty =
            DependencyProperty.Register(
                nameof(InitialDirectory),
                typeof(string),
                typeof(OpenFile),
                new PropertyMetadata(default(string)), v => v == null || v is string);

        public string InitialDirectory
        {
            get => (string)GetValue(InitialDirectoryProperty);
            set => SetValue(InitialDirectoryProperty, value);
        }

        #endregion

        #region ReadOnlyChecked dependency property : bool

        public static readonly DependencyProperty ReadOnlyCheckedProperty =
            DependencyProperty.Register(
                nameof(ReadOnlyChecked),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)), v => v is bool);

        public bool ReadOnlyChecked
        {
            get => (bool)GetValue(ReadOnlyCheckedProperty);
            set => SetValue(ReadOnlyCheckedProperty, value);
        }

        #endregion

        #region RestoreDirectory dependency property : bool

        public static readonly DependencyProperty RestoreDirectoryProperty =
            DependencyProperty.Register(
                nameof(RestoreDirectory),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)), v => v is bool);

        public bool RestoreDirectory
        {
            get => (bool)GetValue(RestoreDirectoryProperty);
            set => SetValue(RestoreDirectoryProperty, value);
        }

        #endregion

        #region ShowReadOnly dependency property : bool

        public static readonly DependencyProperty ShowReadOnlyProperty =
            DependencyProperty.Register(
                nameof(ShowReadOnly),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(true), v => v is bool);

        public bool ShowReadOnly
        {
            get => (bool)GetValue(ShowReadOnlyProperty);
            set => SetValue(ShowReadOnlyProperty, value);
        }

        #endregion

        #region ValidateNames dependency property : bool

        public static readonly DependencyProperty ValidateNamesProperty =
            DependencyProperty.Register(
                nameof(ValidateNames),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)), v => v is bool);

        public bool ValidateNames
        {
            get => (bool)GetValue(ValidateNamesProperty);
            set => SetValue(ValidateNamesProperty, value);
        }

        #endregion

        #region CustomPlaces dependency property : IList<FileDialogCustomPlace>

        public static readonly DependencyProperty CustomPlacesProperty =
            DependencyProperty.Register(
                nameof(CustomPlaces),
                typeof(IList<FileDialogCustomPlace>),
                typeof(OpenFile),
                new PropertyMetadata(default(IList<FileDialogCustomPlace>)), v => v == null || v is IList<FileDialogCustomPlace>);

        public IList<FileDialogCustomPlace> CustomPlaces
        {
            get => (IList<FileDialogCustomPlace>)GetValue(CustomPlacesProperty);
            set => SetValue(CustomPlacesProperty, value);
        }

        #endregion

        #endregion

        protected override void OpenDialog(object p)
        {
            var dialog = new OpenFileDialog
            {
                Filter = Filter,
                FileName = p as string ?? SelectedFile?.FullName,
                AddExtension = AddExtension,
                CheckFileExists = CheckFileExists,
                CheckPathExists = CheckPathExists,
                DefaultExt = DefaultExt,
                DereferenceLinks = DereferenceLinks,
                FilterIndex = FilterIndex,
                Multiselect = Multiselect,
                ReadOnlyChecked = ReadOnlyChecked,
                RestoreDirectory = RestoreDirectory,
                ShowReadOnly = ShowReadOnly,
                ValidateNames = ValidateNames
            };
            var title = Title;
            if(title != null) dialog.Title = Title;
            var initial_directory = InitialDirectory;
            if(initial_directory != null) dialog.InitialDirectory = initial_directory;
            var custom_places = CustomPlaces;
            if(custom_places != null) dialog.CustomPlaces = custom_places;

            var result = dialog.ShowDialog();

            if(result != true && !UpdateIfResultFalse) return;
            SelectedFiles = dialog.FileNames.Select(f => new FileInfo(f)).ToArray();
            SelectedFile = new FileInfo(dialog.FileName);
        }
    }
}
