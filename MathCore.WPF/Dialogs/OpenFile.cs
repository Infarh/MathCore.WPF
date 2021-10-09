using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

using Microsoft.Win32;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Dialogs
{
    /// <summary>Диалог выбора файла</summary>
    public class OpenFile : Dialog
    {
        #region Dependency properties

        #region SelectedFile : FileInfo - Выбранный файл

        /// <summary>Выбранный файл</summary>
        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register(
                nameof(SelectedFile),
                typeof(FileInfo),
                typeof(OpenFile),
                new PropertyMetadata(
                    default(FileInfo), 
                    (d, e) => d.SetValue(SelectedFileNameProperty, ((FileInfo?)e.NewValue)?.FullName)));

        /// <summary>Выбранный файл</summary>
        public FileInfo? SelectedFile
        {
            get => (FileInfo?)GetValue(SelectedFileProperty);
            set => SetValue(SelectedFileProperty, value);
        }

        #endregion

        #region SelectedFileName : string - Путь к выбранному файлу

        /// <summary>Путь к выбранному файлу</summary>
        public static readonly DependencyProperty SelectedFileNameProperty =
            DependencyProperty.Register(
                nameof(SelectedFileName),
                typeof(string),
                typeof(OpenFile),
                new PropertyMetadata(default(string), OnSelectedFileNameChanged));

        /// <summary>Обработчик события изменения свойства <see cref="SelectedFileName"/></summary>
        private static void OnSelectedFileNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var path = (string)e.NewValue;
            var old_file = (FileInfo?)d.GetValue(SelectedFileProperty);
            if (old_file is null || !string.Equals(old_file.FullName, path, StringComparison.Ordinal))
                d.SetValue(SelectedFileProperty, path is { Length: > 0 } str ? new FileInfo(str) : null);
        }

        /// <summary>Путь к выбранному файлу</summary>
        //[Category("")]
        [Description("Путь к выбранному файлу")]
        public string SelectedFileName
        {
            get => (string)GetValue(SelectedFileNameProperty);
            set => SetValue(SelectedFileNameProperty, value);
        }

        #endregion

        #region SelectedFiles : FileInfo[] - Массив выбранный файлов диалога

        /// <summary>Массив выбранный файлов диалога</summary>
        private static readonly DependencyPropertyKey __SelectedFilesPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(SelectedFiles),
                typeof(FileInfo[]),
                typeof(OpenFile),
                new FrameworkPropertyMetadata(default(FileInfo[])));

        public static readonly DependencyProperty SelectedFilesProperty = __SelectedFilesPropertyKey.DependencyProperty;

        /// <summary>Массив выбранный файлов диалога</summary>
        public FileInfo[] SelectedFiles
        {
            get => (FileInfo[])GetValue(SelectedFilesProperty);
            private set => SetValue(__SelectedFilesPropertyKey, value);
        }

        #endregion

        #region Filter : string - Фильтр в формате - Текст (*.*)|*.* - маска файлов фильтра

        /// <summary>Фильтр в формате - Текст (*.*)|*.* - маска файлов фильтра</summary>
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register(
                nameof(Filter),
                typeof(string),
                typeof(OpenFile),
                new PropertyMetadata("*.*|*.*"));

        /// <summary>Фильтр в формате - Текст (*.*)|*.* - маска файлов фильтра</summary>
        public string Filter
        {
            get => (string)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        #endregion

        #region Result : bool? - Результат последнего выбора пользователя в диалоге

        /// <summary>Результат последнего выбора пользователя в диалоге</summary>
        private static readonly DependencyPropertyKey __ResultPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(Result),
                typeof(bool?),
                typeof(OpenFile),
                new FrameworkPropertyMetadata(default(bool?)));

        public static readonly DependencyProperty ResultProperty = __ResultPropertyKey.DependencyProperty;

        /// <summary>Результат последнего выбора пользователя в диалоге</summary>
        public bool? Result
        {
            get => (bool?)GetValue(ResultProperty);
            private set => SetValue(__ResultPropertyKey, value);
        }

        #endregion

        #region Multiselect : bool - Разрешить множественный выбор

        /// <summary>Разрешить множественный выбор</summary>
        public static readonly DependencyProperty MultiSelectProperty =
            DependencyProperty.Register(
                nameof(MultiSelect),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)));

        /// <summary>Разрешить множественный выбор</summary>
        public bool MultiSelect
        {
            get => (bool)GetValue(MultiSelectProperty);
            set => SetValue(MultiSelectProperty, value);
        }

        #endregion

        #region AddExtension : bool - Добавлять расширение

        /// <summary>Добавлять расширение</summary>
        public static readonly DependencyProperty AddExtensionProperty =
            DependencyProperty.Register(
                nameof(AddExtension),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)));

        /// <summary>Добавлять расширение</summary>
        public bool AddExtension
        {
            get => (bool)GetValue(AddExtensionProperty);
            set => SetValue(AddExtensionProperty, value);
        }

        #endregion

        #region CheckFileExists : bool - Проверять что файл существует

        /// <summary>Проверять что файл существует</summary>
        public static readonly DependencyProperty CheckFileExistsProperty =
            DependencyProperty.Register(
                nameof(CheckFileExists),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)));

        /// <summary>Проверять что файл существует</summary>
        public bool CheckFileExists
        {
            get => (bool)GetValue(CheckFileExistsProperty);
            set => SetValue(CheckFileExistsProperty, value);
        }

        #endregion

        #region CheckPathExists : bool - Проверять что путь существует

        /// <summary>Проверять что путь существует</summary>
        public static readonly DependencyProperty CheckPathExistsProperty =
            DependencyProperty.Register(
                nameof(CheckPathExists),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)));

        /// <summary>Проверять что путь существует</summary>
        public bool CheckPathExists
        {
            get => (bool)GetValue(CheckPathExistsProperty);
            set => SetValue(CheckPathExistsProperty, value);
        }

        #endregion

        #region DefaultExt : string - Расширение по умолчанию

        /// <summary>Расширение по умолчанию</summary>
        public static readonly DependencyProperty DefaultExtProperty =
            DependencyProperty.Register(
                nameof(DefaultExt),
                typeof(string),
                typeof(OpenFile),
                new PropertyMetadata(default(string)));

        /// <summary>Расширение по умолчанию</summary>
        public string DefaultExt
        {
            get => (string)GetValue(DefaultExtProperty);
            set => SetValue(DefaultExtProperty, value);
        }

        #endregion

        #region DereferenceLinks : bool

        public static readonly DependencyProperty DereferenceLinksProperty =
            DependencyProperty.Register(
                nameof(DereferenceLinks),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)));

        public bool DereferenceLinks
        {
            get => (bool)GetValue(DereferenceLinksProperty);
            set => SetValue(DereferenceLinksProperty, value);
        }

        #endregion

        #region FilterIndex : int - Выбранный индекс фильтра

        /// <summary>Выбранный индекс фильтра</summary>
        public static readonly DependencyProperty FilterIndexProperty =
            DependencyProperty.Register(
                nameof(FilterIndex),
                typeof(int),
                typeof(OpenFile),
                new PropertyMetadata(default(int)), v => v is >= 0);

        /// <summary>Выбранный индекс фильтра</summary>
        public int FilterIndex
        {
            get => (int)GetValue(FilterIndexProperty);
            set => SetValue(FilterIndexProperty, value);
        }

        #endregion

        #region InitialDirectory : string - Начальная директория

        /// <summary>Начальная директория</summary>
        public static readonly DependencyProperty InitialDirectoryProperty =
            DependencyProperty.Register(
                nameof(InitialDirectory),
                typeof(string),
                typeof(OpenFile),
                new PropertyMetadata(default(string)));

        /// <summary>Начальная директория</summary>
        public string InitialDirectory
        {
            get => (string)GetValue(InitialDirectoryProperty);
            set => SetValue(InitialDirectoryProperty, value);
        }

        #endregion

        #region ReadOnlyChecked : bool - Выбран режим - Только для чтения

        /// <summary>Выбран режим - Только для чтения</summary>
        public static readonly DependencyProperty ReadOnlyCheckedProperty =
            DependencyProperty.Register(
                nameof(ReadOnlyChecked),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)));

        /// <summary>Выбран режим - Только для чтения</summary>
        public bool ReadOnlyChecked
        {
            get => (bool)GetValue(ReadOnlyCheckedProperty);
            set => SetValue(ReadOnlyCheckedProperty, value);
        }

        #endregion

        #region RestoreDirectory : bool - Восстанавливать директорию

        /// <summary>Восстанавливать директорию</summary>
        public static readonly DependencyProperty RestoreDirectoryProperty =
            DependencyProperty.Register(
                nameof(RestoreDirectory),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)));

        /// <summary>Восстанавливать директорию</summary>
        public bool RestoreDirectory
        {
            get => (bool)GetValue(RestoreDirectoryProperty);
            set => SetValue(RestoreDirectoryProperty, value);
        }

        #endregion

        #region ShowReadOnly: bool - Показать кнопку ReadOnly

        /// <summary>Показать кнопку ReadOnly</summary>
        public static readonly DependencyProperty ShowReadOnlyProperty =
            DependencyProperty.Register(
                nameof(ShowReadOnly),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(true));

        /// <summary>Показать кнопку ReadOnly</summary>
        public bool ShowReadOnly
        {
            get => (bool)GetValue(ShowReadOnlyProperty);
            set => SetValue(ShowReadOnlyProperty, value);
        }

        #endregion

        #region ValidateNames : bool - Проверять имена файлов

        /// <summary>Проверять имена файлов</summary>
        public static readonly DependencyProperty ValidateNamesProperty =
            DependencyProperty.Register(
                nameof(ValidateNames),
                typeof(bool),
                typeof(OpenFile),
                new PropertyMetadata(default(bool)));

        /// <summary>Проверять имена файлов</summary>
        public bool ValidateNames
        {
            get => (bool)GetValue(ValidateNamesProperty);
            set => SetValue(ValidateNamesProperty, value);
        }

        #endregion

        #region CustomPlaces : IList<FileDialogCustomPlace> - Собственные расположения

        /// <summary></summary>
        public static readonly DependencyProperty CustomPlacesProperty =
            DependencyProperty.Register(
                nameof(CustomPlaces),
                typeof(IList<FileDialogCustomPlace>),
                typeof(OpenFile),
                new PropertyMetadata(default(IList<FileDialogCustomPlace>)));

        /// <summary>Собственные расположения</summary>
        public IList<FileDialogCustomPlace> CustomPlaces
        {
            get => (IList<FileDialogCustomPlace>)GetValue(CustomPlacesProperty);
            set => SetValue(CustomPlacesProperty, value);
        }

        #endregion

        #endregion

        protected override void OpenDialog(object? p)
        {
            var dialog = new OpenFileDialog
            {
                Filter = Filter,
                FileName = p as string ?? SelectedFile?.FullName ?? string.Empty,
                AddExtension = AddExtension,
                CheckFileExists = CheckFileExists,
                CheckPathExists = CheckPathExists,
                DefaultExt = DefaultExt,
                DereferenceLinks = DereferenceLinks,
                FilterIndex = FilterIndex,
                Multiselect = MultiSelect,
                ReadOnlyChecked = ReadOnlyChecked,
                RestoreDirectory = RestoreDirectory,
                ShowReadOnly = ShowReadOnly,
                ValidateNames = ValidateNames
            };
            if (Title is { } title) dialog.Title = title;
            if (InitialDirectory is { } initial_directory) dialog.InitialDirectory = initial_directory;
            if (CustomPlaces is { } custom_places) dialog.CustomPlaces = custom_places;

            var result = dialog.ShowDialog();

            if (result != true && !UpdateIfResultFalse) return;
            SelectedFiles = dialog.FileNames.Select(f => new FileInfo(f)).ToArray();
            SelectedFile = new FileInfo(dialog.FileName);
        }
    }
}
