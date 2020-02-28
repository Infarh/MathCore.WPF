//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Forms;
//using System.Windows.Input;
//using MathCore.WPF.Commands;
//using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
//using DialogResult = System.Windows.Forms.DialogResult;
//using Binding = System.Windows.Data.Binding;

//namespace MathCore.WPF
//{
//    public class DirectorySelectionDialog : Freezable
//    {
//        #region Description dependency property (Other : Заголовок - описание диалога) : string

//        /// <summary>Заголовок - описание диалога</summary>
//        public static readonly DependencyProperty DescriptionProperty =
//            DependencyProperty.Register(
//                nameof(Description),
//                typeof(string),
//                typeof(DirectorySelectionDialog),
//                new PropertyMetadata(default(string)));

//        /// <summary>Заголовок - описание диалога</summary>
//        [Category("Other")]
//        [Description("Заголовок - описание диалога")]
//        public string Description
//        {
//            get { return (string)GetValue(DescriptionProperty); }
//            set { SetValue(DescriptionProperty, value); }
//        }

//        #endregion

//        #region SelectedPath dependency property (Other : Выбранный путь к каталогу) : string

//        /// <summary>Выбранный путь к каталогу</summary>
//        public static readonly DependencyProperty SelectedPathProperty =
//            DependencyProperty.Register(
//                nameof(SelectedPath),
//                typeof(string),
//                typeof(DirectorySelectionDialog),
//                new FrameworkPropertyMetadata(
//                    default(string),
//                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
//                    OnSelectedPathPropertyChanged));

//        private static void OnSelectedPathPropertyChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
//        {
//            var path = (string)E.NewValue;
//            var old_dir_value = (DirectoryInfo)D.GetValue(SelectedDirectoryProperty);
//            if (path is null)
//                if (old_dir_value is null) return;
//                else D.SetValue(SelectedDirectoryProperty, null);
//            else if (!path.Equals(old_dir_value?.FullName, StringComparison.InvariantCultureIgnoreCase))
//                D.SetValue(SelectedDirectoryProperty, new DirectoryInfo(path));
//        }

//        /// <summary>Выбранный путь к каталогу</summary>
//        [Category("Other")]
//        [Description("Выбранный путь к каталогу")]
//        public string SelectedPath
//        {
//            get { return (string)GetValue(SelectedPathProperty); }
//            set { SetValue(SelectedPathProperty, value); }
//        }

//        #endregion

//        #region SelectedDirectory dependency property (Other : Информация о выбранной дирректории) : DirectoryInfo

//        /// <summary>Информация о выбранной дирректории</summary>
//        public static readonly DependencyProperty SelectedDirectoryProperty =
//            DependencyProperty.Register(
//                nameof(SelectedDirectory),
//                typeof(DirectoryInfo),
//                typeof(DirectorySelectionDialog),
//                new FrameworkPropertyMetadata(
//                    default(DirectoryInfo),
//                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
//                    SelectedDirectoryPropertyChanged));

//        private static void SelectedDirectoryPropertyChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
//        {
//            var dir = (DirectoryInfo)E.NewValue;
//            var old_path_value = (string)D.GetValue(SelectedPathProperty);
//            if (dir is null)
//                if (old_path_value is null) return;
//                else D.SetValue(SelectedDirectoryProperty, null);
//            else if (!dir.FullName.Equals(old_path_value, StringComparison.InvariantCultureIgnoreCase))
//                D.SetValue(SelectedDirectoryProperty, dir?.FullName);
//        }

//        /// <summary>Информация о выбранной дирректории</summary>
//        [Category("Other")]
//        [Description("Информация о выбранной дирректории")]
//        public DirectoryInfo SelectedDirectory
//        {
//            get { return (DirectoryInfo)GetValue(SelectedDirectoryProperty); }
//            set { SetValue(SelectedDirectoryProperty, value); }
//        }

//        #endregion

//        #region ShowNewFolderButton dependency property (Other : Отображать кнопку создания нового каталога) : bool

//        /// <summary>Отображать кнопку создания нового каталога</summary>
//        public static readonly DependencyProperty ShowNewFolderButtonProperty =
//            DependencyProperty.Register(
//                nameof(ShowNewFolderButton),
//                typeof(bool),
//                typeof(DirectorySelectionDialog),
//                new PropertyMetadata(default(bool)));

//        /// <summary>Отображать кнопку создания нового каталога</summary>
//        [Category("Other")]
//        [Description("Отображать кнопку создания нового каталога")]
//        public bool ShowNewFolderButton
//        {
//            get { return (bool)GetValue(ShowNewFolderButtonProperty); }
//            set { SetValue(ShowNewFolderButtonProperty, value); }
//        }

//        #endregion

//        #region Result readonly dependency property (Other : Результат выполнения диалога) : bool?

//        /// <summary>Результат выполнения диалога</summary>
//        private static readonly DependencyPropertyKey ResultPropertyKey =
//            DependencyProperty.RegisterReadOnly(
//                nameof(Result),
//                typeof(bool?),
//                typeof(DirectorySelectionDialog),
//                new PropertyMetadata(default(bool?)));

//        /// <summary>Результат выполнения диалога</summary>
//        public static readonly DependencyProperty ResultProperty = ResultPropertyKey.DependencyProperty;


//        /// <summary>Результат выполнения диалога</summary>
//        [Category("other")]
//        [Description("Результат выполнения диалога")]
//        public bool? Result
//        {
//            get { return (bool?)GetValue(ResultProperty); }
//            private set { SetValue(ResultPropertyKey, value); }
//        }

//        #endregion

//        public ICommand ShowDialogCommand { get; }

//        public DirectorySelectionDialog()
//        {
//            ShowDialogCommand = new LambdaCommand(OnShowDialogCommandExecuted);
//        }

//        /// <inheritdoc />
//        protected override Freezable CreateInstanceCore() => new DirectorySelectionDialog();

//        protected virtual void OnShowDialogCommandExecuted()
//        {
//            var dlg = new FolderBrowserDialog
//            {
//                Description = Description,
//                SelectedPath = SelectedPath,
//                ShowNewFolderButton = ShowNewFolderButton
//            };
//            var result = dlg.ShowDialog();
//            switch (result)
//            {
//                case DialogResult.Yes:
//                case DialogResult.OK:
//                    SelectedPath = dlg.SelectedPath;
//                    Result = true;
//                    break;
//                case DialogResult.No:
//                case DialogResult.Cancel:
//                    Result = false;
//                    break;
//                default:
//                    Result = null;
//                    break;
//            }
//        }
//    }

//    public class FileDialog : Freezable
//    {
//        #region Filter dependency property (Other : Строка фильтра диалога) : string

//        /// <summary>Строка фильтра диалога</summary>
//        public static readonly DependencyProperty FilterProperty =
//            DependencyProperty.Register(
//                nameof(Filter),
//                typeof(string),
//                typeof(FileDialog),
//                new PropertyMetadata("Все файлы (*.*)|*.*"));

//        /// <summary>Строка фильтра диалога</summary>
//        [Category("Other")]
//        [Description("Строка фильтра диалога")]
//        public string Filter
//        {
//            get { return (string)GetValue(FilterProperty); }
//            set { SetValue(FilterProperty, value); }
//        }

//        #endregion

//        #region FilterIndex dependency property (Other : Индекс выбора в списке фильтра расширений файлов) : int

//        /// <summary>Индекс выбора в списке фильтра расширений файлов</summary>
//        public static readonly DependencyProperty FilterIndexProperty =
//            DependencyProperty.Register(
//                nameof(FilterIndex),
//                typeof(int),
//                typeof(FileDialog),
//                new PropertyMetadata(default(int)));

//        /// <summary>Индекс выбора в списке фильтра расширений файлов</summary>
//        [Category("Other")]
//        [Description("Индекс выбора в списке фильтра расширений файлов")]
//        public int FilterIndex
//        {
//            get { return (int)GetValue(FilterIndexProperty); }
//            set { SetValue(FilterIndexProperty, value); }
//        }

//        #endregion

//        #region Title dependency property (Other : Заголовок окна диалога) : string

//        /// <summary>Заголовок окна диалога</summary>
//        public static readonly DependencyProperty TitleProperty =
//            DependencyProperty.Register(
//                nameof(Title),
//                typeof(string),
//                typeof(FileDialog),
//                new PropertyMetadata(default(string)));

//        /// <summary>Заголовок окна диалога</summary>
//        [Category("Other")]
//        [Description("Заголовок окна диалога")]
//        public string Title
//        {
//            get { return (string)GetValue(TitleProperty); }
//            set { SetValue(TitleProperty, value); }
//        }

//        #endregion

//        #region Multiselect dependency property (Other : Множественный выбор) : bool

//        /// <summary>Множественный выбор</summary>
//        public static readonly DependencyProperty MultiselectProperty =
//            DependencyProperty.Register(
//                nameof(Multiselect),
//                typeof(bool),
//                typeof(FileDialog),
//                new PropertyMetadata(default(bool)));

//        /// <summary>Множественный выбор</summary>
//        [Category("Other")]
//        [Description("Множественный выбор")]
//        public bool Multiselect
//        {
//            get { return (bool)GetValue(MultiselectProperty); }
//            set { SetValue(MultiselectProperty, value); }
//        }

//        #endregion

//        #region AddExtension dependency property (Other : Добавлять расширение при необходимости) : bool

//        /// <summary>Добавлять расширение при необходимости</summary>
//        public static readonly DependencyProperty AddExtensionProperty =
//            DependencyProperty.Register(
//                nameof(AddExtension),
//                typeof(bool),
//                typeof(FileDialog),
//                new PropertyMetadata(default(bool)));

//        /// <summary>Добавлять расширение при необходимости</summary>
//        [Category("Other")]
//        [Description("Добавлять расширение при необходимости")]
//        public bool AddExtension
//        {
//            get { return (bool)GetValue(AddExtensionProperty); }
//            set { SetValue(AddExtensionProperty, value); }
//        }

//        #endregion

//        #region DefaultExt dependency property (Other : Расширение по умолчанию) : string

//        /// <summary>Расширение по умолчанию</summary>
//        public static readonly DependencyProperty DefaultExtProperty =
//            DependencyProperty.Register(
//                nameof(DefaultExt),
//                typeof(string),
//                typeof(FileDialog),
//                new PropertyMetadata(default(string)));

//        /// <summary>Расширение по умолчанию</summary>
//        [Category("Other")]
//        [Description("Расширение по умолчанию")]
//        public string DefaultExt
//        {
//            get { return (string)GetValue(DefaultExtProperty); }
//            set { SetValue(DefaultExtProperty, value); }
//        }

//        #endregion

//        #region FileName dependency property (Other : Имя выбранного файла) : string

//        /// <summary>Имя выбранного файла</summary>
//        public static readonly DependencyProperty FileNameProperty =
//            DependencyProperty.Register(
//                nameof(FileName),
//                typeof(string),
//                typeof(FileDialog),
//                new FrameworkPropertyMetadata(
//                    default(string),
//                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
//                    OnFileNamePropertyChanged));

//        private static void OnFileNamePropertyChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
//        {
//            var path = (string)E.NewValue;
//            var old_file = (FileInfo)D.GetValue(SelectedFileProperty);
//            if (path is null)
//                if (old_file is null) return;
//                else D.SetValue(SelectedFileProperty, null);
//            else if (!path.Equals(old_file?.FullName, StringComparison.InvariantCultureIgnoreCase))
//                D.SetValue(SelectedFileProperty, new FileInfo(path));
//        }

//        /// <summary>Имя выбранного файла</summary>
//        [Category("Other")]
//        [Description("Имя выбранного файла")]
//        public string FileName
//        {
//            get { return (string)GetValue(FileNameProperty); }
//            set { SetValue(FileNameProperty, value); }
//        }

//        #endregion

//        #region SelectedFile dependency property (Other : Информация о выбранном файле) : FileInfo

//        /// <summary>Информация о выбранном файле</summary>
//        public static readonly DependencyProperty SelectedFileProperty =
//            DependencyProperty.Register(
//                nameof(SelectedFile),
//                typeof(FileInfo),
//                typeof(FileDialog),
//                new FrameworkPropertyMetadata(
//                    default(FileInfo),
//                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
//                    OnSelectedFilePropertyChanged));

//        private static void OnSelectedFilePropertyChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
//        {
//            var file_info = (FileInfo)E.NewValue;
//            var old_file_name = (string)D.GetValue(FileNameProperty);
//            if (file_info is null)
//                if (old_file_name is null) return;
//                else D.SetValue(FileNameProperty, null);
//            else if (!file_info.FullName.Equals(old_file_name, StringComparison.InvariantCultureIgnoreCase))
//                D.SetValue(FileNameProperty, file_info.FullName);
//        }

//        /// <summary>Информация о выбранном файле</summary>
//        [Category("Other")]
//        [Description("Информация о выбранном файле")]
//        public FileInfo SelectedFile
//        {
//            get { return (FileInfo)GetValue(SelectedFileProperty); }
//            set { SetValue(SelectedFileProperty, value); }
//        }

//        #endregion

//        #region FileNames readonly dependency property (Other : Выбранные файлы) : string[]

//        /// <summary>Выбранные файлы</summary>
//        private static readonly DependencyPropertyKey FileNamesPropertyKey =
//            DependencyProperty.RegisterReadOnly(
//                nameof(FileNames),
//                typeof(string[]),
//                typeof(FileDialog),
//                new PropertyMetadata(default(string[]), OnFileNamesPropertyChanged));

//        private static void OnFileNamesPropertyChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
//        {
//            var files = (string[])E.NewValue;
//            var old_file_infos = (FileInfo[])D.GetValue(SelectedFilesProperty);
//            if (files is null)
//                if (old_file_infos is null) return;
//                else D.SetValue(SelectedFilesPropertyKey, null);
//            else if (files.Length != old_file_infos.Length)
//                D.SetValue(SelectedFilesPropertyKey, files.Select(f => new FileInfo(f)).ToArray());
//            else
//                for (var i = 0; i < files.Length; i++)
//                    if (!files[i].Equals(old_file_infos[i]?.FullName, StringComparison.InvariantCultureIgnoreCase))
//                    {
//                        D.SetValue(SelectedFilesPropertyKey, files.Select(f => new FileInfo(f)).ToArray());
//                        break;
//                    }
//        }

//        /// <summary>Выбранные файлы</summary>
//        public static readonly DependencyProperty FileNamesProperty = FileNamesPropertyKey.DependencyProperty;

//        /// <summary>Выбранные файлы</summary>
//        [Category("Other")]
//        [Description("Выбранные файлы")]
//        public string[] FileNames
//        {
//            get { return (string[])GetValue(FileNamesProperty); }
//            private set { SetValue(FileNamesPropertyKey, value); }
//        }

//        #endregion

//        #region SelectedFiles readonly dependency property (Other : Информация о выбранных файлах) : FileInfo[]

//        /// <summary>Информация о выбранных файлах</summary>
//        private static readonly DependencyPropertyKey SelectedFilesPropertyKey =
//            DependencyProperty.RegisterReadOnly(
//                nameof(SelectedFiles),
//                typeof(FileInfo[]),
//                typeof(FileDialog),
//                new PropertyMetadata(default(FileInfo[]), OnSelectedFilesPropertyChanged));

//        private static void OnSelectedFilesPropertyChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
//        {
//            var file_infos = (FileInfo[])E.NewValue;
//            var old_file_names = (string[])D.GetValue(FileNamesProperty);
//            if (file_infos is null)
//                if (old_file_names is null) return;
//                else D.SetValue(FileNamesPropertyKey, null);
//            else if (file_infos.Length != old_file_names.Length)
//                D.SetValue(FileNamesPropertyKey, file_infos.Select(f => f.FullName).ToArray());
//            else
//                for (var i = 0; i < file_infos.Length; i++)
//                    if ((file_infos[i] is null && old_file_names[i] != null)
//                        || file_infos[i]?.FullName.Equals(old_file_names[i], StringComparison.InvariantCultureIgnoreCase) != true)
//                    {
//                        D.SetValue(FileNamesPropertyKey, file_infos.Select(f => f.FullName).ToArray());
//                        break;
//                    }
//        }

//        /// <summary>Информация о выбранных файлах</summary>
//        public static readonly DependencyProperty SelectedFilesProperty = SelectedFilesPropertyKey.DependencyProperty;

//        /// <summary>Информация о выбранных файлах</summary>
//        [Category("Other")]
//        [Description("Информация о выбранных файлах")]
//        public FileInfo[] SelectedFiles
//        {
//            get { return (FileInfo[])GetValue(SelectedFilesProperty); }
//            private set { SetValue(SelectedFilesPropertyKey, value); }
//        }

//        #endregion

//        #region Result readonly dependency property (Other : Результат выполнения диалога) : bool?

//        /// <summary>Результат выполнения диалога</summary>
//        private static readonly DependencyPropertyKey ResultPropertyKey =
//            DependencyProperty.RegisterReadOnly(
//                nameof(Result),
//                typeof(bool?),
//                typeof(FileDialog),
//                new PropertyMetadata(default(bool?)));

//        /// <summary>Результат выполнения диалога</summary>
//        public static readonly DependencyProperty ResultProperty = ResultPropertyKey.DependencyProperty;

//        /// <summary>Результат выполнения диалога</summary>
//        [Category("Other")]
//        [Description("Результат выполнения диалога")]
//        public bool? Result
//        {
//            get { return (bool?)GetValue(ResultProperty); }
//            private set { SetValue(ResultPropertyKey, value); }
//        }

//        #endregion

//        public ICommand OpenFileCommand { get; }
//        public ICommand SaveFileCommand { get; }

//        public FileDialog()
//        {
//            OpenFileCommand = new LambdaCommand(OnOpenFileCommandExecuted);
//            SaveFileCommand = new LambdaCommand(OnSaveFileCommandExecuted);
//        }

//        private void OnSaveFileCommandExecuted()
//        {
//            var dlg = new SaveFileDialog
//            {
//                Filter = Filter,
//                Title = Title,
//                AddExtension = AddExtension,
//                DefaultExt = DefaultExt,
//                FilterIndex = FilterIndex,
//                FileName = FileName
//            };
//            var result = dlg.ShowDialog();

//            if (result == DialogResult.OK)
//                FileName = dlg.FileName;

//            Result = result == DialogResult.OK;
//        }

//        private void OnOpenFileCommandExecuted()
//        {
//            var multiselect = Multiselect;
//            var dlg = new OpenFileDialog
//            {
//                Filter = Filter,
//                Title = Title,
//                Multiselect = multiselect,
//                AddExtension = AddExtension,
//                DefaultExt = DefaultExt,
//                FilterIndex = FilterIndex,
//                FileName = FileName
//            };
//            var result = dlg.ShowDialog();

//            if (result == DialogResult.OK)
//            {
//                FileName = dlg.FileName;
//                if (multiselect) FileNames = dlg.FileNames;
//            }

//            Result = result == DialogResult.OK;
//        }

//        /// <inheritdoc />
//        protected override Freezable CreateInstanceCore() => new FileDialog();


//    }
//}
