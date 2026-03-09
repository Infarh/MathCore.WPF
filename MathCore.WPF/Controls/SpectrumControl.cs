using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MathCore.WPF.Controls;

/// <summary>Элемент визуализации амплитудного спектра</summary>
public class SpectrumControl : Control
{
    private const double __LeftMargin = 62;
    private const double __RightMargin = 16;
    private const double __TopMargin = 14;
    private const double __BottomMargin = 34;

    private INotifyCollectionChanged? _ItemsNotifier;

    /// <summary>Коллекция графиков спектра</summary>
    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>DependencyProperty коллекции графиков спектра</summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnItemsSourceChanged));

    /// <summary>Минимум вертикальной шкалы (дБ)</summary>
    public double AmplitudeMinimum
    {
        get => (double)GetValue(AmplitudeMinimumProperty);
        set => SetValue(AmplitudeMinimumProperty, value);
    }

    /// <summary>DependencyProperty минимума вертикальной шкалы</summary>
    public static readonly DependencyProperty AmplitudeMinimumProperty =
        DependencyProperty.Register(
            nameof(AmplitudeMinimum),
            typeof(double),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(-120d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Максимум вертикальной шкалы (дБ)</summary>
    public double AmplitudeMaximum
    {
        get => (double)GetValue(AmplitudeMaximumProperty);
        set => SetValue(AmplitudeMaximumProperty, value);
    }

    /// <summary>DependencyProperty максимума вертикальной шкалы</summary>
    public static readonly DependencyProperty AmplitudeMaximumProperty =
        DependencyProperty.Register(
            nameof(AmplitudeMaximum),
            typeof(double),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Минимум горизонтальной шкалы частоты</summary>
    public double FrequencyMinimum
    {
        get => (double)GetValue(FrequencyMinimumProperty);
        set => SetValue(FrequencyMinimumProperty, value);
    }

    /// <summary>DependencyProperty минимума частоты</summary>
    public static readonly DependencyProperty FrequencyMinimumProperty =
        DependencyProperty.Register(
            nameof(FrequencyMinimum),
            typeof(double),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Максимум горизонтальной шкалы частоты</summary>
    public double FrequencyMaximum
    {
        get => (double)GetValue(FrequencyMaximumProperty);
        set => SetValue(FrequencyMaximumProperty, value);
    }

    /// <summary>DependencyProperty максимума частоты</summary>
    public static readonly DependencyProperty FrequencyMaximumProperty =
        DependencyProperty.Register(
            nameof(FrequencyMaximum),
            typeof(double),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(20000d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Единицы измерения частоты</summary>
    public string FrequencyUnit
    {
        get => (string)GetValue(FrequencyUnitProperty);
        set => SetValue(FrequencyUnitProperty, value);
    }

    /// <summary>DependencyProperty единиц измерения частоты</summary>
    public static readonly DependencyProperty FrequencyUnitProperty =
        DependencyProperty.Register(
            nameof(FrequencyUnit),
            typeof(string),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata("Hz", FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Тип масштаба оси частот</summary>
    public SpectrumFrequencyScale FrequencyScale
    {
        get => (SpectrumFrequencyScale)GetValue(FrequencyScaleProperty);
        set => SetValue(FrequencyScaleProperty, value);
    }

    /// <summary>DependencyProperty типа масштаба оси частот</summary>
    public static readonly DependencyProperty FrequencyScaleProperty =
        DependencyProperty.Register(
            nameof(FrequencyScale),
            typeof(SpectrumFrequencyScale),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(SpectrumFrequencyScale.Linear, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Кисть линий сетки</summary>
    public Brush GridLineBrush
    {
        get => (Brush)GetValue(GridLineBrushProperty);
        set => SetValue(GridLineBrushProperty, value);
    }

    /// <summary>DependencyProperty кисти линий сетки</summary>
    public static readonly DependencyProperty GridLineBrushProperty =
        DependencyProperty.Register(
            nameof(GridLineBrush),
            typeof(Brush),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(Brushes.DimGray, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Толщина линий сетки</summary>
    public double GridLineThickness
    {
        get => (double)GetValue(GridLineThicknessProperty);
        set => SetValue(GridLineThicknessProperty, value);
    }

    /// <summary>DependencyProperty толщины линий сетки</summary>
    public static readonly DependencyProperty GridLineThicknessProperty =
        DependencyProperty.Register(
            nameof(GridLineThickness),
            typeof(double),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(0.8d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Кисть осей</summary>
    public Brush AxisBrush
    {
        get => (Brush)GetValue(AxisBrushProperty);
        set => SetValue(AxisBrushProperty, value);
    }

    /// <summary>DependencyProperty кисти осей</summary>
    public static readonly DependencyProperty AxisBrushProperty =
        DependencyProperty.Register(
            nameof(AxisBrush),
            typeof(Brush),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(Brushes.WhiteSmoke, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Толщина линий осей</summary>
    public double AxisThickness
    {
        get => (double)GetValue(AxisThicknessProperty);
        set => SetValue(AxisThicknessProperty, value);
    }

    /// <summary>DependencyProperty толщины осей</summary>
    public static readonly DependencyProperty AxisThicknessProperty =
        DependencyProperty.Register(
            nameof(AxisThickness),
            typeof(double),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(1.1d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Кисть области построения</summary>
    public Brush PlotBackground
    {
        get => (Brush)GetValue(PlotBackgroundProperty);
        set => SetValue(PlotBackgroundProperty, value);
    }

    /// <summary>DependencyProperty кисти области построения</summary>
    public static readonly DependencyProperty PlotBackgroundProperty =
        DependencyProperty.Register(
            nameof(PlotBackground),
            typeof(Brush),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Количество горизонтальных делений сетки</summary>
    public int HorizontalGridDivisions
    {
        get => (int)GetValue(HorizontalGridDivisionsProperty);
        set => SetValue(HorizontalGridDivisionsProperty, value);
    }

    /// <summary>DependencyProperty количества горизонтальных делений</summary>
    public static readonly DependencyProperty HorizontalGridDivisionsProperty =
        DependencyProperty.Register(
            nameof(HorizontalGridDivisions),
            typeof(int),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(8, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Количество вертикальных делений сетки</summary>
    public int VerticalGridDivisions
    {
        get => (int)GetValue(VerticalGridDivisionsProperty);
        set => SetValue(VerticalGridDivisionsProperty, value);
    }

    /// <summary>DependencyProperty количества вертикальных делений</summary>
    public static readonly DependencyProperty VerticalGridDivisionsProperty =
        DependencyProperty.Register(
            nameof(VerticalGridDivisions),
            typeof(int),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Кисть графиков по умолчанию</summary>
    public Brush DefaultSeriesStroke
    {
        get => (Brush)GetValue(DefaultSeriesStrokeProperty);
        set => SetValue(DefaultSeriesStrokeProperty, value);
    }

    /// <summary>DependencyProperty кисти графиков по умолчанию</summary>
    public static readonly DependencyProperty DefaultSeriesStrokeProperty =
        DependencyProperty.Register(
            nameof(DefaultSeriesStroke),
            typeof(Brush),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(Brushes.LimeGreen, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Толщина графиков по умолчанию</summary>
    public double DefaultSeriesThickness
    {
        get => (double)GetValue(DefaultSeriesThicknessProperty);
        set => SetValue(DefaultSeriesThicknessProperty, value);
    }

    /// <summary>DependencyProperty толщины графиков по умолчанию</summary>
    public static readonly DependencyProperty DefaultSeriesThicknessProperty =
        DependencyProperty.Register(
            nameof(DefaultSeriesThickness),
            typeof(double),
            typeof(SpectrumControl),
            new FrameworkPropertyMetadata(1.8d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Инициализирует новый экземпляр SpectrumControl</summary>
    public SpectrumControl()
    {
        SnapsToDevicePixels = true;
        UseLayoutRounding = true;
        Foreground = Brushes.WhiteSmoke;
    }

    protected override void OnRender(DrawingContext DrawingContext)
    {
        base.OnRender(DrawingContext);

        var plot_rect = new Rect(__LeftMargin, __TopMargin, Math.Max(1d, ActualWidth - __LeftMargin - __RightMargin), Math.Max(1d, ActualHeight - __TopMargin - __BottomMargin));

        DrawingContext.DrawRectangle(PlotBackground, null, plot_rect);

        DrawGrid(DrawingContext, plot_rect);
        DrawAxis(DrawingContext, plot_rect);
        DrawSeries(DrawingContext, plot_rect);
    }

    private void DrawGrid(DrawingContext DrawingContext, Rect PlotRect)
    {
        var horizontal_count = Math.Max(1, HorizontalGridDivisions);
        var vertical_count = Math.Max(1, VerticalGridDivisions);
        var grid_pen = new Pen(GridLineBrush, Math.Max(0.1, GridLineThickness)) { DashStyle = DashStyles.Dot };

        for (var index = 0; index <= horizontal_count; index++)
        {
            var t = (double)index / horizontal_count;
            var y = PlotRect.Top + PlotRect.Height * t;
            DrawingContext.DrawLine(grid_pen, new(PlotRect.Left, y), new(PlotRect.Right, y));

            var level = Lerp(AmplitudeMaximum, AmplitudeMinimum, t);
            DrawText(DrawingContext, $"{level:0.#} dB", new(3, y - 8), TextAlignment.Left);
        }

        for (var index = 0; index <= vertical_count; index++)
        {
            var t = (double)index / vertical_count;
            var x = PlotRect.Left + PlotRect.Width * t;
            DrawingContext.DrawLine(grid_pen, new(x, PlotRect.Top), new(x, PlotRect.Bottom));

            var frequency = GetFrequencyFromScale(t);
            DrawText(DrawingContext, $"{frequency:0.##} {FrequencyUnit}", new(x, PlotRect.Bottom + 4), TextAlignment.Center);
        }
    }

    private void DrawAxis(DrawingContext DrawingContext, Rect PlotRect)
    {
        var axis_pen = new Pen(AxisBrush, Math.Max(0.1, AxisThickness));

        DrawingContext.DrawRectangle(null, axis_pen, PlotRect);
        DrawText(DrawingContext, "A, dB", new(4, PlotRect.Top - 11), TextAlignment.Left);
        DrawText(DrawingContext, $"f, {FrequencyUnit}", new(PlotRect.Right - 2, PlotRect.Bottom + 16), TextAlignment.Right);
    }

    private void DrawSeries(DrawingContext DrawingContext, Rect PlotRect)
    {
        if (ItemsSource is null)
            return;

        foreach (var item in ItemsSource)
        {
            if (item is not SpectrumSeries { Points: not null } series)
                continue;

            var geometry = new StreamGeometry();
            using var context = geometry.Open();

            var is_started = false;
            foreach (var point in series.Points)
            {
                if (!TryMapPoint(point, PlotRect, out var screen_point))
                    continue;

                if (!is_started)
                {
                    context.BeginFigure(screen_point, false, false);
                    is_started = true;
                    continue;
                }

                context.LineTo(screen_point, true, true);
            }

            if (!is_started)
                continue;

            geometry.Freeze();

            var stroke = series.Stroke ?? DefaultSeriesStroke;
            var thickness = series.Thickness > 0 ? series.Thickness : DefaultSeriesThickness;
            var pen = new Pen(stroke, thickness);

            if (series.DashArray is { Count: > 0 })
                pen.DashStyle = new(series.DashArray, 0);

            DrawingContext.DrawGeometry(null, pen, geometry);
        }
    }

    private bool TryMapPoint(Point DataPoint, Rect PlotRect, out Point ScreenPoint)
    {
        var frequency = DataPoint.X;
        var amplitude = DataPoint.Y;

        if (double.IsNaN(frequency) || double.IsNaN(amplitude) || double.IsInfinity(frequency) || double.IsInfinity(amplitude))
        {
            ScreenPoint = default;
            return false;
        }

        var x_t = GetXRatio(frequency);
        var y_t = GetYRatio(amplitude);

        if (x_t is < 0 or > 1 || y_t is < 0 or > 1)
        {
            ScreenPoint = default;
            return false;
        }

        var x = PlotRect.Left + PlotRect.Width * x_t;
        var y = PlotRect.Bottom - PlotRect.Height * y_t;
        ScreenPoint = new(x, y);
        return true;
    }

    private double GetXRatio(double Frequency)
    {
        var min_frequency = FrequencyMinimum;
        var max_frequency = FrequencyMaximum;

        if (FrequencyScale == SpectrumFrequencyScale.Logarithmic)
        {
            if (min_frequency <= 0 || max_frequency <= 0 || Frequency <= 0)
                return -1;

            var min_log = Math.Log10(min_frequency);
            var max_log = Math.Log10(max_frequency);
            return (Math.Log10(Frequency) - min_log) / Math.Max(1e-12, max_log - min_log);
        }

        return (Frequency - min_frequency) / Math.Max(1e-12, max_frequency - min_frequency);
    }

    private double GetYRatio(double Amplitude) =>
        (Amplitude - AmplitudeMinimum) / Math.Max(1e-12, AmplitudeMaximum - AmplitudeMinimum);

    private double GetFrequencyFromScale(double Ratio)
    {
        if (FrequencyScale == SpectrumFrequencyScale.Logarithmic && FrequencyMinimum > 0 && FrequencyMaximum > 0)
        {
            var exponent = Lerp(Math.Log10(FrequencyMinimum), Math.Log10(FrequencyMaximum), Ratio);
            return Math.Pow(10, exponent);
        }

        return Lerp(FrequencyMinimum, FrequencyMaximum, Ratio);
    }

    private void DrawText(DrawingContext DrawingContext, string Text, Point Origin, TextAlignment Alignment)
    {
#if NET461
        var text = new FormattedText(
           Text,
           CultureInfo.CurrentUICulture,
           FlowDirection.LeftToRight,
           new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
           Math.Max(8d, FontSize),
           Foreground);
#else
        var text = new FormattedText(
           Text,
           CultureInfo.CurrentUICulture,
           FlowDirection.LeftToRight,
           new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
           Math.Max(8d, FontSize),
           Foreground,
           96);
#endif

        text.TextAlignment = Alignment;
        DrawingContext.DrawText(text, Origin);
    }

    private static double Lerp(double Start, double End, double Ratio) => Start + (End - Start) * Ratio;

    private static void OnItemsSourceChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        if (D is not SpectrumControl control)
            return;

        control._ItemsNotifier?.CollectionChanged -= control.OnItemsCollectionChanged;

        control._ItemsNotifier = E.NewValue as INotifyCollectionChanged;
        control._ItemsNotifier?.CollectionChanged += control.OnItemsCollectionChanged;

        control.InvalidateVisual();
    }

    private void OnItemsCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E) => InvalidateVisual();
}
