using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;

namespace MathCore.WPF;

//https://stackoverflow.com/a/1134340

public class GIF : Image
{
    private bool _IsInitialized;
    private GifBitmapDecoder _GifDecoder;
    private Int32Animation _Animation;

    public int FrameIndex
    {
        get => (int)GetValue(FrameIndexProperty);
        set => SetValue(FrameIndexProperty, value);
    }

    private void Initialize()
    {
        _GifDecoder = new(new Uri(GifSource), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

        _Animation = new(
            fromValue: 0, 
            toValue: _GifDecoder.Frames.Count - 1,
            // ReSharper disable once PossibleLossOfFraction
            duration: new(new(0, 0, 0, _GifDecoder.Frames.Count / 10, (int)(1000 * (_GifDecoder.Frames.Count / 10d - _GifDecoder.Frames.Count / 10)))))
        {
            RepeatBehavior = RepeatBehavior.Forever
        };

        Source = _GifDecoder.Frames[0];

        _IsInitialized = true;
    }

    static GIF() =>
        VisibilityProperty.OverrideMetadata(
            typeof(GIF),
            new FrameworkPropertyMetadata(VisibilityPropertyChanged));

    private static void VisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if ((Visibility)e.NewValue == Visibility.Visible)
            ((GIF)sender).StartAnimation();
        else
            ((GIF)sender).StopAnimation();
    }

    public static readonly DependencyProperty FrameIndexProperty =
        DependencyProperty.Register(
            nameof(FrameIndex),
            typeof(int), 
            typeof(GIF),
            new UIPropertyMetadata(0, ChangingFrameIndex));

    private static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
    {
        var gif_image = obj as GIF;
        gif_image.Source = gif_image._GifDecoder.Frames[(int)ev.NewValue];
    }

    /// <summary>
    /// Defines whether the animation starts on it's own
    /// </summary>
    public bool AutoStart
    {
        get => (bool)GetValue(AutoStartProperty);
        set => SetValue(AutoStartProperty, value);
    }

    public static readonly DependencyProperty AutoStartProperty =
        DependencyProperty.Register(
            nameof(AutoStart),
            typeof(bool),
            typeof(GIF), 
            new UIPropertyMetadata(false, AutoStartPropertyChanged));

    private static void AutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue)
            (sender as GIF).StartAnimation();
    }

    public string GifSource
    {
        get => (string)GetValue(GifSourceProperty);
        set => SetValue(GifSourceProperty, value);
    }

    public static readonly DependencyProperty GifSourceProperty =
        DependencyProperty.Register(
            nameof(GifSource), 
            typeof(string), 
            typeof(GIF), 
            new UIPropertyMetadata(string.Empty, GifSourcePropertyChanged));

    private static void GifSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) => (sender as GIF).Initialize();

    /// <summary>
    /// Starts the animation
    /// </summary>
    public void StartAnimation()
    {
        if (!_IsInitialized)
            Initialize();

        BeginAnimation(FrameIndexProperty, _Animation);
    }

    /// <summary>
    /// Stops the animation
    /// </summary>
    public void StopAnimation() => BeginAnimation(FrameIndexProperty, null);
}
