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
        // Инициализируем декодер по заданному источнику GIF
        _GifDecoder = new(new Uri(GifSource), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

        _Animation = new(
            fromValue: 0,
            toValue: _GifDecoder.Frames.Count - 1,
            // ReSharper disable once PossibleLossOfFraction
            duration: new(new(0, 0, 0, _GifDecoder.Frames.Count / 10, (int)(1000 * (_GifDecoder.Frames.Count / 10d - _GifDecoder.Frames.Count / 10)))))
        { RepeatBehavior = RepeatBehavior.Forever }; // настраиваем бесконечную анимацию

        Source = _GifDecoder.Frames[0]; // устанавливаем первый кадр как источник изображения

        _IsInitialized = true; // помечаем как инициализированное
    }

    static GIF() =>
        VisibilityProperty.OverrideMetadata(
            typeof(GIF),
            new FrameworkPropertyMetadata(VisibilityPropertyChanged)); // переопределяем поведение свойства Visibility

    private static void VisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if ((Visibility)e.NewValue == Visibility.Visible)
            ((GIF)sender).StartAnimation(); // если стало видимым — запускаем анимацию
        else
            ((GIF)sender).StopAnimation(); // иначе — останавливаем
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
        gif_image.Source = gif_image._GifDecoder.Frames[(int)ev.NewValue]; // меняем отображаемый кадр
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
        if ((bool)e.NewValue)
            (sender as GIF).StartAnimation(); // при установке true запускаем анимацию
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

    private static void GifSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as GIF).Initialize(); // при смене источника — инициализируем

    /// <summary>Запустить анимацию GIF</summary>
    public void StartAnimation()
    {
        if (!_IsInitialized)
            Initialize(); // ленивое создание ресурсов

        BeginAnimation(FrameIndexProperty, _Animation); // запускаем анимацию смены индекса кадра
    }

    /// <summary>Остановить анимацию GIF</summary>
    public void StopAnimation() => BeginAnimation(FrameIndexProperty, null); // останавливаем анимацию
}
