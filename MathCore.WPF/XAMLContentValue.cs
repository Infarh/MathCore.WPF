using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace MathCore.WPF;

/// <summary>Представляет значение, загружающее содержимое XAML из URI.</summary>
public class XAMLContentValue : DependencyObject
{
    private Task<object> _LoadContentTask;
    private readonly FileSystemWatcher? _FileWatcher;

    /// <summary>Возвращает URI содержимого XAML.</summary>
    public Uri URI { get; }

    #region Content : object - Содержимое

    /// <summary>Возвращает или задает загруженное содержимое XAML.</summary>
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(
            nameof(Content),
            typeof(object),
            typeof(XAMLContentValue),
            new(default(object)));

    /// <summary>Возвращает или задает загруженное содержимое XAML.</summary>
    [Description("Содержимое")]
    public object Content { get => GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

    #endregion

    /// <summary>Инициализирует новый экземпляр класса <see cref="XAMLContentValue"/>.</summary>
    /// <param name="uri">URI содержимого XAML.</param>
    /// <exception cref="ArgumentException">Выбрасывается, если <paramref name="uri"/> равен null или пуст.</exception>
    public XAMLContentValue(string? uri)
    {
        if (uri is not { Length: > 0 })
            throw new ArgumentException("URI не может быть null или пуст.", nameof(uri));

        URI = new(uri);
        _LoadContentTask = LoadContentAsync();

        // Если URI является файлом и он существует, настраиваем наблюдатель файла для перезагрузки содержимого при изменении файла.
        if (this.URI.IsFile && File.Exists(uri))
        {
            _FileWatcher = new(Path.GetDirectoryName(Path.GetFullPath(uri))!, Path.GetFileName(uri))
            {
                EnableRaisingEvents = true
            };
            _FileWatcher.Changed += OnFileChanged;
        }
    }

    /// <summary>Обрабатывает событие изменения файла, перезагружая содержимое XAML.</summary>
    /// <param name="sender">Источник события.</param>
    /// <param name="e">Аргументы события.</param>
    private void OnFileChanged(object sender, FileSystemEventArgs e) => _LoadContentTask = LoadContentAsync();

    /// <summary>Загружает содержимое XAML из URI асинхронно.</summary>
    /// <returns>Задача, представляющая загруженное содержимое XAML.</returns>
    private async Task<object> LoadContentAsync()
    {
        await Task.Yield().ConfigureAwait(false);

        // Создаем контекст парсера для загрузки содержимого XAML.
        var parser_context = new ParserContext
        {
            // TODO: Настройте контекст парсера по необходимости.
        };

        // Загружаем содержимое XAML из файла.
        var result = XamlReader.Load(File.OpenRead(URI.ToString()), parser_context);

        // Устанавливаем загруженное содержимое в качестве значения свойства Content.
        Content = result;
        return result;
    }

    ///// <summary>Возвращает поток для указанного URI асинхронно.</summary>
    ///// <param name="uri">URI, для которого необходимо получить поток.</param>
    ///// <returns>Задача, представляющая поток для указанного URI.</returns>
    //private static Task<Stream> GetDataStreamAsync(Uri uri)
    //{
    //    // Если URI является файлом и он существует, возвращаем поток файла.
    //    if (uri.IsFile && uri.ToString() is var filePath)
    //        return File.Exists(filePath)
    //            ? Task.FromResult<Stream>(File.OpenRead(filePath))
    //            : throw new FileNotFoundException("Файл не найден", filePath);

    //    // TODO: Реализуйте поддержку URI, не являющихся файлами.
    //    throw new NotSupportedException("Чтение не из файлового потока не поддерживается");
    //}
}