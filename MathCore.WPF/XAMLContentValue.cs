﻿using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace MathCore.WPF;

public class XAMLContentValue : DependencyObject
{
    private Task<object> _LoadContentTask;
    private readonly FileSystemWatcher? _FileWatcher;

    public Uri URI { get; }

    #region Content : object - Содержимое

    /// <summary>Содержимое</summary>
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(
            nameof(Content),
            typeof(object),
            typeof(XAMLContentValue),
            new(default(object)));

    /// <summary>Содержимое</summary>
    //[Category("")]
    [Description("Содержимое")]
    public object Content { get => GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

    #endregion

    public XAMLContentValue(string? URI)
    {
        if (URI is not { Length: > 0 })
            throw new ArgumentException(nameof(URI));

        this.URI         = new(URI);
        _LoadContentTask = LoadContentAsync();
        if(!this.URI.IsFile || !File.Exists(URI))
            return;

        _FileWatcher = new(Path.GetDirectoryName(Path.GetFullPath(URI))!, Path.GetFileName(URI))
        {
            EnableRaisingEvents = true
        };
        _FileWatcher.Changed += OnFileChanged;

    }

    private void OnFileChanged(object Sender, FileSystemEventArgs E) => _LoadContentTask = LoadContentAsync();

    private async Task<object> LoadContentAsync()
    {
        await Task.Yield().ConfigureAwait(false);

        var parser_context = new ParserContext
        {
                
        };

        var result = XamlReader.Load(File.OpenRead(URI.ToString()), parser_context);

        Content = result;
        return result;
    }

    private static Task<Stream> GetDataStreamAsync(Uri uri)
    {
        if(uri.IsFile && uri.ToString() is var file_path)
            return File.Exists(file_path) 
                ? Task.FromResult<Stream>(File.OpenRead(file_path)) 
                : throw new FileNotFoundException("Файл не найден", file_path);

        throw new NotSupportedException("Чтение не из файлового потока не поддерживается");
        //var client = new HttpClient();
        //return await client.GetStreamAsync(uri);
    }
}