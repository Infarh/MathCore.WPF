using System.IO;
using System.Text;

using Microsoft.Win32;

namespace MathCore.WPF;

/// <summary>Построитель диалоговых окон выбора файла для открытия или сохранения</summary>
public readonly ref struct FileDialogEx
{
    /// <summary>Элемент фильтра файлов в диалоговом окне</summary>
    public readonly struct FileFilterItem : IEquatable<FileFilterItem>
    {
        /// <summary>Название фильтра, отображаемое пользователю</summary>
        public string Title { get; init; }

        /// <summary>Коллекция расширений файлов для фильтрации</summary>
        public IEnumerable<string>? Values { get; init; }

        /// <summary>Инициализирует новый элемент фильтра файлов</summary>
        /// <param name="Title">Название фильтра</param>
        /// <param name="Value">Массив расширений файлов</param>
        public FileFilterItem(string Title, params string[] Value)
        {
            this.Title = Title;
            if (Value is { Length: > 0 })
                Values = Value.Distinct();
        }

        /// <summary>Добавляет строковое представление фильтра к StringBuilder</summary>
        /// <param name="filter">StringBuilder для добавления строки фильтра</param>
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
                    str.Length--; // Удаление последнего разделителя
                else
                    str.Append("*.*");
            }

            filter.Append('|');
        }

        /// <summary>Определяет, равен ли текущий элемент фильтра указанному</summary>
        /// <param name="other">Элемент фильтра для сравнения</param>
        /// <returns>true, если элементы равны; иначе false</returns>
        public bool Equals(FileFilterItem other) => Title == other.Title && Values.SequenceEqual(other.Values, StringComparer.OrdinalIgnoreCase);

        public override bool Equals(object? obj) => obj is FileFilterItem item && Equals(item);

        public static bool operator ==(FileFilterItem left, FileFilterItem right) => left.Equals(right);

        public static bool operator !=(FileFilterItem left, FileFilterItem right) => !(left == right);

        public override int GetHashCode()
        {
            var hash = new HashBuilder("FileDialog".GetHashCode()).Append("FileFileter");
            foreach (var item in Values)
                hash = hash.Append(item.GetHashCode());
            return hash;
        }
    }

    /// <summary>Создает построитель диалога открытия файла</summary>
    /// <returns>Новый экземпляр FileDialogEx для открытия файла</returns>
    public static FileDialogEx OpenFile() => new() { IsSaveFileDialog = false };
    
    /// <summary>Создает построитель диалога открытия файла с указанным заголовком</summary>
    /// <param name="Title">Заголовок диалогового окна</param>
    /// <returns>Новый экземпляр FileDialogEx для открытия файла</returns>
    public static FileDialogEx OpenFile(string Title) => new() { IsSaveFileDialog = false, Title = Title };

    /// <summary>Создает построитель диалога сохранения файла</summary>
    /// <returns>Новый экземпляр FileDialogEx для сохранения файла</returns>
    public static FileDialogEx CreateFile() => new() { IsSaveFileDialog = true };
    
    /// <summary>Создает построитель диалога сохранения файла с указанным заголовком</summary>
    /// <param name="Title">Заголовок диалогового окна</param>
    /// <returns>Новый экземпляр FileDialogEx для сохранения файла</returns>
    public static FileDialogEx CreateFile(string Title) => new() { IsSaveFileDialog = true, Title = Title };

    /// <summary>Создает новый построитель диалога файла</summary>
    /// <returns>Новый экземпляр FileDialogEx</returns>
    public static FileDialogEx New() => new();
    
    /// <summary>Создает новый построитель диалога файла с указанным заголовком</summary>
    /// <param name="Title">Заголовок диалогового окна</param>
    /// <returns>Новый экземпляр FileDialogEx</returns>
    public static FileDialogEx New(string Title) => new() { Title = Title };

    /// <summary>Определяет, является ли диалог диалогом сохранения файла</summary>
    public bool IsSaveFileDialog { get; init; }

    /// <summary>Заголовок диалогового окна</summary>
    public string Title { get; init; }

    /// <summary>Определяет, следует ли восстанавливать текущую директорию при закрытии диалога</summary>
    public bool? RestoreDirectory { get; init; }

    /// <summary>Начальная директория, отображаемая в диалоге</summary>
    public string? InitialDirectory { get; init; }

    /// <summary>Определяет, следует ли проверять существование выбранного файла</summary>
    public bool? CheckFileExists { get; init; }

    /// <summary>Определяет, установлен ли флажок "Только для чтения"</summary>
    public bool? ReadOnlyChecked { get; init; }

    /// <summary>Коллекция фильтров файлов для диалога</summary>
    public IEnumerable<FileFilterItem>? Filter { get; init; }

    /// <summary>Определяет, можно ли выбрать несколько файлов одновременно</summary>
    public bool? Multiselect { get; init; }

    /// <summary>Имя файла по умолчанию</summary>
    public string? DefaultFileName { get; init; }

    /// <summary>Расширение файла по умолчанию (без точки)</summary>
    public string? DefaultExtension { get; init; }

    /// <summary>Индекс выбранного фильтра файлов (начиная с 1)</summary>
    public int? FilterIndex { get; init; }

    /// <summary>Определяет, следует ли проверять существование пути к файлу</summary>
    public bool? CheckPathExists { get; init; }

    /// <summary>Определяет, следует ли автоматически добавлять расширение к имени файла</summary>
    public bool? AddExtension { get; init; }

    /// <summary>Определяет, следует ли разыменовывать ярлыки (.lnk файлы)</summary>
    public bool? DereferenceLinks { get; init; }

    /// <summary>Определяет, следует ли проверять корректность имен файлов</summary>
    public bool? ValidateNames { get; init; }

    /// <summary>Запрашивает подтверждение при перезаписи существующего файла (только для SaveFileDialog)</summary>
    public bool? OverwritePrompt { get; init; }

    /// <summary>Запрашивает подтверждение при создании нового файла (только для SaveFileDialog)</summary>
    public bool? CreatePrompt { get; init; }

    public FileDialogEx() { }

    /// <summary>Устанавливает заголовок диалогового окна</summary>
    /// <param name="Title">Новый заголовок</param>
    /// <returns>Новый экземпляр FileDialogEx с обновленным заголовком</returns>
    public FileDialogEx SetTitle(string Title) => this with { Title = Title };

    /// <summary>Устанавливает начальную директорию</summary>
    /// <param name="Path">Путь к начальной директории</param>
    /// <returns>Новый экземпляр FileDialogEx с установленной начальной директорией</returns>
    public FileDialogEx WithInitialDirectory(string Path) => this with { InitialDirectory = Path };

    /// <summary>Включает восстановление текущей директории при закрытии диалога</summary>
    /// <param name="Restore">Значение флага восстановления</param>
    /// <returns>Новый экземпляр FileDialogEx с установленным флагом</returns>
    public FileDialogEx WithRestoreDirectory(bool Restore = true) => this with { RestoreDirectory = Restore };

    /// <summary>Включает проверку существования выбранного файла</summary>
    /// <param name="Check">Значение флага проверки</param>
    /// <returns>Новый экземпляр FileDialogEx с установленным флагом</returns>
    public FileDialogEx WithCheckFileExists(bool Check = true) => this with { CheckFileExists = Check };

    /// <summary>Включает проверку существования пути к файлу</summary>
    /// <param name="Check">Значение флага проверки</param>
    /// <returns>Новый экземпляр FileDialogEx с установленным флагом</returns>
    public FileDialogEx WithCheckPathExists(bool Check = true) => this with { CheckPathExists = Check };

    /// <summary>Устанавливает флажок "Только для чтения"</summary>
    /// <param name="ReadOnly">Значение флажка</param>
    /// <returns>Новый экземпляр FileDialogEx с установленным флажком</returns>
    public FileDialogEx WithReadOnlyChecked(bool ReadOnly = true) => this with { ReadOnlyChecked = ReadOnly };

    /// <summary>Включает возможность множественного выбора файлов</summary>
    /// <param name="Enable">Значение флага множественного выбора</param>
    /// <returns>Новый экземпляр FileDialogEx с установленным флагом</returns>
    public FileDialogEx EnableMultiselect(bool Enable = true) => this with { Multiselect = Enable };

    /// <summary>Устанавливает имя файла по умолчанию</summary>
    /// <param name="FileName">Имя файла</param>
    /// <returns>Новый экземпляр FileDialogEx с установленным именем файла</returns>
    public FileDialogEx WithDefaultFileName(string FileName) => this with { DefaultFileName = FileName };

    /// <summary>Устанавливает расширение файла по умолчанию</summary>
    /// <param name="Extension">Расширение файла (без точки)</param>
    /// <returns>Новый экземпляр FileDialogEx с установленным расширением</returns>
    public FileDialogEx WithDefaultExtension(string Extension) => this with { DefaultExtension = Extension.TrimStart('.') };

    /// <summary>Устанавливает индекс активного фильтра</summary>
    /// <param name="Index">Индекс фильтра (начиная с 1)</param>
    /// <returns>Новый экземпляр FileDialogEx с установленным индексом фильтра</returns>
    public FileDialogEx WithFilterIndex(int Index) => this with { FilterIndex = Index };

    /// <summary>Включает автоматическое добавление расширения к имени файла</summary>
    /// <param name="Add">Значение флага</param>
    /// <returns>Новый экземпляр FileDialogEx с установленным флагом</returns>
    public FileDialogEx WithAddExtension(bool Add = true) => this with { AddExtension = Add };

    /// <summary>Включает разыменование ярлыков</summary>
    /// <param name="Dereference">Значение флага</param>
    /// <returns>Новый экземпляр FileDialogEx с установленным флагом</returns>
    public FileDialogEx WithDereferenceLinks(bool Dereference = true) => this with { DereferenceLinks = Dereference };

    /// <summary>Включает проверку корректности имен файлов</summary>
    /// <param name="Validate">Значение флага</param>
    /// <returns>Новый экземпляр FileDialogEx с установленным флагом</returns>
    public FileDialogEx WithValidateNames(bool Validate = true) => this with { ValidateNames = Validate };

    /// <summary>Включает запрос подтверждения при перезаписи файла</summary>
    /// <param name="Prompt">Значение флага</param>
    /// <returns>Новый экземпляр FileDialogEx с установленным флагом</returns>
    public FileDialogEx WithOverwritePrompt(bool Prompt = true) => this with { OverwritePrompt = Prompt };

    /// <summary>Включает запрос подтверждения при создании нового файла</summary>
    /// <param name="Prompt">Значение флага</param>
    /// <returns>Новый экземпляр FileDialogEx с установленным флагом</returns>
    public FileDialogEx WithCreatePrompt(bool Prompt = true) => this with { CreatePrompt = Prompt };

#if NET5_0_OR_GREATER
    /// <summary>Добавляет новый фильтр файлов к диалогу</summary>
    /// <param name="Name">Название фильтра</param>
    /// <param name="Ext">Массив расширений файлов</param>
    /// <returns>Новый экземпляр FileDialogEx с добавленным фильтром</returns>
    public FileDialogEx AddFilter(string Name, params string[] Ext) => Filter is { } filter
       ? this with { Filter = filter.Append(new(Name, Ext)) }
       : this with { Filter = new FileFilterItem[] { new(Name, Ext) } };
#else
    /// <summary>Добавляет новый фильтр файлов к диалогу</summary>
    /// <param name="Name">Название фильтра</param>
    /// <param name="Ext">Массив расширений файлов</param>
    /// <returns>Новый экземпляр FileDialogEx с добавленным фильтром</returns>
   public FileDialogEx AddFilter(string Name, params string[] Ext) => Filter is { } filter
       ? this with { Filter = filter.AppendLast(new FileFilterItem(Name, Ext)) }
       : this with { Filter = [new(Name, Ext)] };
#endif

    /// <summary>Добавляет фильтр "Все файлы" к диалогу, если он еще не добавлен</summary>
    /// <returns>Новый экземпляр FileDialogEx с добавленным фильтром всех файлов</returns>
    public FileDialogEx AddFilterAllFiles() => Filter is null || Filter.Last().Title != "Все файлы"
        ? AddFilter("Все файлы", "*.*")
        : this;

    /// <summary>Добавляет фильтр для текстовых файлов</summary>
    /// <returns>Новый экземпляр FileDialogEx с добавленным фильтром</returns>
    public FileDialogEx AddTextFilesFilter() => AddFilter("Текстовые файлы", "*.txt");

    /// <summary>Добавляет фильтр для файлов изображений</summary>
    /// <returns>Новый экземпляр FileDialogEx с добавленным фильтром</returns>
    public FileDialogEx AddImageFilesFilter() => AddFilter("Изображения", "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif", "*.tif", "*.tiff", "*.ico");

    /// <summary>Добавляет фильтр для JSON файлов</summary>
    /// <returns>Новый экземпляр FileDialogEx с добавленным фильтром</returns>
    public FileDialogEx AddJsonFilesFilter() => AddFilter("JSON файлы", "*.json");

    /// <summary>Добавляет фильтр для XML файлов</summary>
    /// <returns>Новый экземпляр FileDialogEx с добавленным фильтром</returns>
    public FileDialogEx AddXmlFilesFilter() => AddFilter("XML файлы", "*.xml");

    /// <summary>Добавляет фильтр для CSV файлов</summary>
    /// <returns>Новый экземпляр FileDialogEx с добавленным фильтром</returns>
    public FileDialogEx AddCsvFilesFilter() => AddFilter("CSV файлы", "*.csv");

    /// <summary>Добавляет фильтр для документов Microsoft Word</summary>
    /// <returns>Новый экземпляр FileDialogEx с добавленным фильтром</returns>
    public FileDialogEx AddWordFilesFilter() => AddFilter("Документы Word", "*.docx", "*.doc");

    /// <summary>Добавляет фильтр для документов Microsoft Excel</summary>
    /// <returns>Новый экземпляр FileDialogEx с добавленным фильтром</returns>
    public FileDialogEx AddExcelFilesFilter() => AddFilter("Документы Excel", "*.xlsx", "*.xls");

    /// <summary>Добавляет фильтр для PDF файлов</summary>
    /// <returns>Новый экземпляр FileDialogEx с добавленным фильтром</returns>
    public FileDialogEx AddPdfFilesFilter() => AddFilter("PDF файлы", "*.pdf");

    /// <summary>Создает и настраивает экземпляр OpenFileDialog</summary>
    /// <returns>Настроенный экземпляр OpenFileDialog</returns>
    public OpenFileDialog CreateOpenFileDialog()
    {
        var dialog = new OpenFileDialog();

        ApplyCommonSettings(dialog);

        if (ReadOnlyChecked is { } read_only_checked)
            dialog.ReadOnlyChecked = read_only_checked;

        if (Multiselect is { } multiselect)
            dialog.Multiselect = multiselect;

        return dialog;
    }

    /// <summary>Создает и настраивает экземпляр SaveFileDialog</summary>
    /// <returns>Настроенный экземпляр SaveFileDialog</returns>
    public SaveFileDialog CreateSaveFileDialog()
    {
        var dialog = new SaveFileDialog();

        ApplyCommonSettings(dialog);

        if (OverwritePrompt is { } overwrite_prompt)
            dialog.OverwritePrompt = overwrite_prompt;

        if (CreatePrompt is { } create_prompt)
            dialog.CreatePrompt = create_prompt;

        return dialog;
    }

    /// <summary>Применяет общие настройки к диалогу</summary>
    /// <param name="dialog">Диалог для настройки</param>
    private void ApplyCommonSettings(FileDialog dialog)
    {
        if (Title is not null)
            dialog.Title = Title;

        if (Filter is { } filter)
        {
            var filter_str = new StringBuilder();

            foreach (var item in filter)
                item.AppendTo(filter_str);

            filter_str.Length--; // Удаление последнего разделителя

            dialog.Filter = filter_str.ToString();
        }

        if (FilterIndex is { } filter_index)
            dialog.FilterIndex = filter_index;

        if (RestoreDirectory is { } restore_directory)
            dialog.RestoreDirectory = restore_directory;

        if (InitialDirectory is { Length: > 0 } initial_directory)
            dialog.InitialDirectory = initial_directory;

        if (DefaultFileName is { Length: > 0 } default_file_name)
            dialog.FileName = default_file_name;

        if (DefaultExtension is { Length: > 0 } default_extension)
            dialog.DefaultExt = default_extension;

        if (CheckFileExists is { } check_file_exists)
            dialog.CheckFileExists = check_file_exists;

        if (CheckPathExists is { } check_path_exists)
            dialog.CheckPathExists = check_path_exists;

        if (AddExtension is { } add_extension)
            dialog.AddExtension = add_extension;

        if (DereferenceLinks is { } dereference_links)
            dialog.DereferenceLinks = dereference_links;

        if (ValidateNames is { } validate_names)
            dialog.ValidateNames = validate_names;
    }

    /// <summary>Отображает диалог и возвращает полный путь к выбранному файлу</summary>
    /// <returns>Полный путь к выбранному файлу или null, если диалог был отменен</returns>
    /// <example>
    /// <code>
    /// var file_name = FileDialogEx.OpenFile("Выберите файл")
    ///     .AddFilter("Текстовые файлы", "*.txt")
    ///     .AddFilterAllFiles()
    ///     .GetFileName();
    /// </code>
    /// </example>
    public string? GetFileName()
    {
        FileDialog dialog = IsSaveFileDialog ? CreateSaveFileDialog() : CreateOpenFileDialog();

        if (dialog.ShowDialog() != true)
            return null;

        return dialog.FileName;
    }

    /// <summary>Отображает диалог и возвращает массив путей к выбранным файлам</summary>
    /// <returns>Массив путей к выбранным файлам или null, если диалог был отменен</returns>
    /// <remarks>Работает только для диалога открытия файла с включенным Multiselect</remarks>
    public string[]? GetFileNames()
    {
        if (IsSaveFileDialog)
            return GetFileName() is { } file_name ? [file_name] : null;

        var dialog = CreateOpenFileDialog();

        if (dialog.ShowDialog() != true)
            return null;

        return dialog.FileNames;
    }

    /// <summary>Отображает диалог и возвращает FileInfo для выбранного файла</summary>
    /// <returns>FileInfo выбранного файла или null, если диалог был отменен</returns>
    public FileInfo? GetFileInfo() => GetFileName() is { } file_name ? new(file_name) : null;

    /// <summary>Отображает диалог и возвращает массив FileInfo для выбранных файлов</summary>
    /// <returns>Массив FileInfo выбранных файлов или null, если диалог был отменен</returns>
    public FileInfo[]? GetFileInfos() => GetFileNames() is { } file_names
        ? file_names.Select(f => new FileInfo(f)).ToArray()
        : null;

    /// <summary>Отображает диалог и открывает поток для выбранного файла</summary>
    /// <returns>FileStream для выбранного файла или null, если диалог был отменен</returns>
    /// <remarks>Для диалога сохранения создается новый файл, для диалога открытия - открывается для чтения</remarks>
    public FileStream? OpenFileStream() => GetFileName() is { } file_name
        ? IsSaveFileDialog
            ? File.Create(file_name)
            : File.OpenRead(file_name)
        : null;

    /// <summary>Отображает диалог и читает весь текст из выбранного файла</summary>
    /// <param name="Encoding">Кодировка текста (по умолчанию UTF-8)</param>
    /// <returns>Содержимое файла в виде строки или null, если диалог был отменен</returns>
    public string? ReadAllText(Encoding? Encoding = null) => GetFileName() is { } file_name
        ? Encoding is null
            ? File.ReadAllText(file_name)
            : File.ReadAllText(file_name, Encoding)
        : null;

    /// <summary>Отображает диалог и читает все строки из выбранного файла</summary>
    /// <param name="Encoding">Кодировка текста (по умолчанию UTF-8)</param>
    /// <returns>Массив строк файла или null, если диалог был отменен</returns>
    public string[]? ReadAllLines(Encoding? Encoding = null) => GetFileName() is { } file_name
        ? Encoding is null
            ? File.ReadAllLines(file_name)
            : File.ReadAllLines(file_name, Encoding)
        : null;

    /// <summary>Отображает диалог и читает все байты из выбранного файла</summary>
    /// <returns>Массив байтов файла или null, если диалог был отменен</returns>
    public byte[]? ReadAllBytes() => GetFileName() is { } file_name ? File.ReadAllBytes(file_name) : null;

    /// <summary>Отображает диалог и записывает текст в выбранный файл</summary>
    /// <param name="Content">Содержимое для записи</param>
    /// <param name="Encoding">Кодировка текста (по умолчанию UTF-8)</param>
    /// <returns>true, если файл был успешно записан; false, если диалог был отменен</returns>
    public bool WriteAllText(string Content, Encoding? Encoding = null)
    {
        if (GetFileName() is not { } file_name)
            return false;

        if (Encoding is null)
            File.WriteAllText(file_name, Content);
        else
            File.WriteAllText(file_name, Content, Encoding);

        return true;
    }

    /// <summary>Отображает диалог и записывает массив строк в выбранный файл</summary>
    /// <param name="Lines">Строки для записи</param>
    /// <param name="Encoding">Кодировка текста (по умолчанию UTF-8)</param>
    /// <returns>true, если файл был успешно записан; false, если диалог был отменен</returns>
    public bool WriteAllLines(string[] Lines, Encoding? Encoding = null)
    {
        if (GetFileName() is not { } file_name)
            return false;

        if (Encoding is null)
            File.WriteAllLines(file_name, Lines);
        else
            File.WriteAllLines(file_name, Lines, Encoding);

        return true;
    }

    /// <summary>Отображает диалог и записывает байты в выбранный файл</summary>
    /// <param name="Content">Байты для записи</param>
    /// <returns>true, если файл был успешно записан; false, если диалог был отменен</returns>
    public bool WriteAllBytes(byte[] Content)
    {
        if (GetFileName() is not { } file_name)
            return false;

        File.WriteAllBytes(file_name, Content);
        return true;
    }

    /// <summary>Отображает диалог и выполняет действие с выбранным файлом</summary>
    /// <typeparam name="T">Тип возвращаемого значения</typeparam>
    /// <param name="Action">Функция для выполнения с путем к файлу</param>
    /// <returns>Результат выполнения функции или default, если диалог был отменен</returns>
    /// <example>
    /// <code>
    /// var lines_count = FileDialogEx.OpenFile()
    ///     .AddTextFilesFilter()
    ///     .WithFile(path => File.ReadAllLines(path).Length);
    /// </code>
    /// </example>
    public T? WithFile<T>(Func<string, T> Action)
    {
        if (Action is null) throw new ArgumentNullException(nameof(Action));

        return GetFileName() is { } file_name ? Action(file_name) : default;
    }

    /// <summary>Отображает диалог и выполняет действие с потоком выбранного файла</summary>
    /// <typeparam name="T">Тип возвращаемого значения</typeparam>
    /// <param name="Action">Функция для выполнения с потоком файла</param>
    /// <returns>Результат выполнения функции или default, если диалог был отменен</returns>
    /// <example>
    /// <code>
    /// var content = FileDialogEx.OpenFile()
    ///     .AddTextFilesFilter()
    ///     .WithFileStream(stream =>
    ///     {
    ///         using var reader = new StreamReader(stream);
    ///         return reader.ReadToEnd();
    ///     });
    /// </code>
    /// </example>
    public T? WithFileStream<T>(Func<FileStream, T> Action)
    {
        if (Action is null) throw new ArgumentNullException(nameof(Action));

        if (OpenFileStream() is not { } stream)
            return default;

        using (stream)
            return Action(stream);
    }

    /// <summary>Отображает диалог и выполняет действие для каждого выбранного файла</summary>
    /// <param name="Action">Действие для выполнения с каждым файлом</param>
    /// <returns>Количество обработанных файлов</returns>
    public int ForEachFile(Action<string> Action)
    {
        if (Action is null) throw new ArgumentNullException(nameof(Action));

        if (GetFileNames() is not { } file_names)
            return 0;

        foreach (var file_name in file_names)
            Action(file_name);

        return file_names.Length;
    }
}
