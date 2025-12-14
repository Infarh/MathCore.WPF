using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace MathCore.WPF;

/// <summary>Представляет значение, загружающее содержимое XAML из URI</summary>
public class XAMLContentValue : DependencyObject
{
    private Task<object?> _LoadContentTask; // задача последней загрузки содержимого
    private readonly FileSystemWatcher? _FileWatcher;

    /// <summary>Возвращает URI содержимого XAML</summary>
    public Uri URI { get; }

    /// <summary>Словарь пространств имён XAML для настройки контекста парсера</summary>
    public IDictionary<string, string>? XmlNamespaces { get; set; }

    #region Content : object - Содержимое

    /// <summary>Возвращает или задает загруженное содержимое XAML</summary>
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(
            nameof(Content),
            typeof(object),
            typeof(XAMLContentValue),
            new(default(object)));

    /// <summary>Возвращает или задает загруженное содержимое XAML</summary>
    [Description("Содержимое")]
    public object Content { get => GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

    #endregion

    /// <summary>Инициализирует новый экземпляр класса <see cref="XAMLContentValue"/></summary>
    /// <param name="uri">URI содержимого XAML</param>
    /// <exception cref="ArgumentException">Выбрасывается, если <paramref name="uri"/> равен null или пуст</exception>
    public XAMLContentValue(string? uri)
    {
        if (uri is not { Length: > 0 })
            throw new ArgumentException("URI не может быть null или пуст", nameof(uri));

        URI = new(uri);
        _LoadContentTask = ReloadSafeAsync();

        // Если URI является файлом и он существует, настраиваем наблюдатель файла для перезагрузки содержимого при изменении файла
        if (URI.IsFile && File.Exists(URI.LocalPath))
        {
            var file_path = Path.GetFullPath(URI.LocalPath);
            var directory_path = Path.GetDirectoryName(file_path);
            var file_name = Path.GetFileName(file_path);

            if (directory_path is not null && file_name.Length > 0)
            {
                _FileWatcher = new(directory_path, file_name)
                {
                    EnableRaisingEvents = true
                };
                _FileWatcher.Changed += OnFileChanged;
            }
        }
    }

    /// <summary>Обрабатывает событие изменения файла, перезагружая содержимое XAML</summary>
    private void OnFileChanged(object sender, FileSystemEventArgs e) => _LoadContentTask = ReloadSafeAsync();

    /// <summary>Безопасная перезагрузка содержимого с обработкой ошибок</summary>
    private async Task<object?> ReloadSafeAsync()
    {
        try
        {
            return await LoadContentAsync().ConfigureAwait(false);
        }
        catch
        {
            // здесь можно добавить логирование при необходимости
            return null;
        }
    }

    /// <summary>Загружает содержимое XAML из URI асинхронно</summary>
    private async Task<object> LoadContentAsync()
    {
        await Task.Yield().ConfigureAwait(false); // переключаемся на поток из пула

        var parser_context = CreateParserContext();
        var path = URI.IsFile ? URI.LocalPath : URI.ToString();

        using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite); // даём внешним процессам возможность записывать файл
        var result = XamlReader.Load(stream, parser_context);

        if (Application.Current?.Dispatcher is { } dispatcher)
            await dispatcher.InvokeAsync(() => Content = result); // установка значения свойства Content в UI-потоке
        else
            Content = result;

        return result;
    }

    /// <summary>Создать и настроить контекст парсера XAML</summary>
    private ParserContext CreateParserContext()
    {
        var parser_context = new ParserContext();

        var uri = URI;
        if (uri.IsFile)
        {
            var file_path = Path.GetFullPath(uri.LocalPath);
            parser_context.BaseUri = new(file_path);
        }

        var xmlns = parser_context.XmlnsDictionary;

        // Базовые пространства имён WPF, если явные не заданы
        if (XmlNamespaces is null || XmlNamespaces.Count == 0)
        {
            if (xmlns[string.Empty] is null)
                xmlns.Add(string.Empty, "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            if (xmlns["x"] is null)
                xmlns.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
        }
        else
            foreach (var pair in XmlNamespaces)
                xmlns[pair.Key ?? string.Empty] = pair.Value; // настраиваем пространства имён согласно словарю

        return parser_context;
    }
}