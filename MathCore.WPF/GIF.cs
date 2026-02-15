using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MathCore.WPF;

/// <summary>Контрол для отображения анимированного GIF</summary>
[Copyright("https://stackoverflow.com/a/1134340")]
public class GIF : Image
{
    private bool _IsInitialized; // флаг инициализации декодера и анимации
    private GifBitmapDecoder _GifDecoder; // декодер GIF
    private Int32Animation _Animation; // анимация для переключения кадров

    /// <summary>Индекс текущего кадра GIF</summary>
    public int FrameIndex { get => (int)GetValue(FrameIndexProperty); set => SetValue(FrameIndexProperty, value); }

    private void Initialize()
    {
        ResetState();

        if (string.IsNullOrWhiteSpace(GifSource)) return;
        if (!Uri.TryCreate(GifSource, UriKind.RelativeOrAbsolute, out var gif_uri)) return;

        // Инициализируем декодер по заданному источнику GIF
        _GifDecoder = new(gif_uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

        if (_GifDecoder.Frames.Count == 0) return;

        if (_GifDecoder.Frames.Count > 1)
            _Animation = new(
                fromValue: 0,
                toValue: _GifDecoder.Frames.Count - 1,
                // ReSharper disable once PossibleLossOfFraction
                duration: new(new(0, 0, 0, _GifDecoder.Frames.Count / 10, (int)(1000 * (_GifDecoder.Frames.Count / 10d - _GifDecoder.Frames.Count / 10)))))
            { RepeatBehavior = RepeatBehavior.Forever }; // настраиваем бесконечную анимацию

        Source = _GifDecoder.Frames[0]; // устанавливаем первый кадр как источник изображения

        _IsInitialized = true; // помечаем как инициализированное
    }

    private void ResetState()
    {
        _IsInitialized = false;
        _GifDecoder = null;
        _Animation = null;
        Source = null;
    }

    static GIF() =>
        VisibilityProperty.OverrideMetadata(
            typeof(GIF),
            new FrameworkPropertyMetadata(VisibilityPropertyChanged)); // переопределяем поведение свойства Visibility

    private static void VisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not GIF gif) return;

        if ((Visibility)e.NewValue == Visibility.Visible)
            gif.StartAnimation(); // если стало видимым — запускаем анимацию
        else
            gif.StopAnimation(); // иначе — останавливаем
    }

    /// <summary>DependencyProperty для индекса кадра</summary>
    public static readonly DependencyProperty FrameIndexProperty =
        DependencyProperty.Register(
            nameof(FrameIndex),
            typeof(int),
            typeof(GIF),
            new UIPropertyMetadata(0, ChangingFrameIndex));

    private static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
    {
        if (obj is not GIF gif_image) return;
        if (gif_image._GifDecoder is null) return;

        var frame_index = (int)ev.NewValue;
        if (frame_index < 0 || frame_index >= gif_image._GifDecoder.Frames.Count) return;

        gif_image.Source = gif_image._GifDecoder.Frames[frame_index]; // меняем отображаемый кадр
    }

    /// <summary>Определяет будет ли анимация запускаться автоматически</summary>
    public bool AutoStart
    {
        get => (bool)GetValue(AutoStartProperty);
        set => SetValue(AutoStartProperty, value);
    }

    /// <summary>DependencyProperty для AutoStart</summary>
    public static readonly DependencyProperty AutoStartProperty =
        DependencyProperty.Register(
            nameof(AutoStart),
            typeof(bool),
            typeof(GIF),
            new UIPropertyMetadata(false, AutoStartPropertyChanged));

    private static void AutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not GIF gif) return;
        if ((bool)e.NewValue)
            gif.StartAnimation(); // при установке true запускаем анимацию
    }

    /// <summary>Путь или URI к GIF-изображению</summary>
    public string GifSource
    {
        get => (string)GetValue(GifSourceProperty);
        set => SetValue(GifSourceProperty, value);
    }

    /// <summary>DependencyProperty для GifSource</summary>
    public static readonly DependencyProperty GifSourceProperty =
        DependencyProperty.Register(
            nameof(GifSource),
            typeof(string),
            typeof(GIF),
            new UIPropertyMetadata(string.Empty, GifSourcePropertyChanged));

    private static void GifSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is GIF gif)
            gif.Initialize(); // при смене источника — инициализируем
    }

    /// <summary>Запустить анимацию GIF</summary>
    public void StartAnimation()
    {
        if (!_IsInitialized)
            Initialize(); // ленивое создание ресурсов

        if (!_IsInitialized || _Animation is null) return;

        BeginAnimation(FrameIndexProperty, _Animation); // запускаем анимацию смены индекса кадра
    }

    /// <summary>Остановить анимацию GIF</summary>
    public void StopAnimation() => BeginAnimation(FrameIndexProperty, null); // останавливаем анимацию
}
