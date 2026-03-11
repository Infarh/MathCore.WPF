using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MathCore.WPF.Controls;

/// <summary>Элемент визуализации спектрального водопада</summary>
public class SpectrumWaterfallControl : Control
{
    private const double LeftMargin = 62;
    private const double RightMargin = 16;
    private const double TopMargin = 14;
    private const double BottomMargin = 34;

    private static readonly WaterfallColorScale __DefaultColorScale = new RainbowWaterfallColorScale();

    private WriteableBitmap? _Bitmap;
    private double[]? _AmplitudeBuffer;
    private int[]? _PixelBuffer;
    private int[] _Palette = [];

    /// <summary>Текущий спектральный кадр амплитуд в дБ</summary>
    public IEnumerable<double>? CurrentSpectrum
    {
        get => (IEnumerable<double>?)GetValue(CurrentSpectrumProperty);
        set => SetValue(CurrentSpectrumProperty, value);
    }

    /// <summary>DependencyProperty текущего кадра амплитуд</summary>
    public static readonly DependencyProperty CurrentSpectrumProperty =
        DependencyProperty.Register(
            nameof(CurrentSpectrum),
            typeof(IEnumerable<double>),
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata(null, OnCurrentSpectrumChanged));

    /// <summary>Минимум амплитуды в дБ</summary>
    public double AmplitudeMinimum
    {
        get => (double)GetValue(AmplitudeMinimumProperty);
        set => SetValue(AmplitudeMinimumProperty, value);
    }

    /// <summary>DependencyProperty минимума амплитуды</summary>
    public static readonly DependencyProperty AmplitudeMinimumProperty =
        DependencyProperty.Register(
            nameof(AmplitudeMinimum),
            typeof(double),
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata(-120d, FrameworkPropertyMetadataOptions.AffectsRender, OnAmplitudeScaleChanged));

    /// <summary>Максимум амплитуды в дБ</summary>
    public double AmplitudeMaximum
    {
        get => (double)GetValue(AmplitudeMaximumProperty);
        set => SetValue(AmplitudeMaximumProperty, value);
    }

    /// <summary>DependencyProperty максимума амплитуды</summary>
    public static readonly DependencyProperty AmplitudeMaximumProperty =
        DependencyProperty.Register(
            nameof(AmplitudeMaximum),
            typeof(double),
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender, OnAmplitudeScaleChanged));

    /// <summary>Минимум частоты</summary>
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
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Максимум частоты</summary>
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
            typeof(SpectrumWaterfallControl),
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
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata("Hz", FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Единицы измерения времени</summary>
    public string TimeUnit
    {
        get => (string)GetValue(TimeUnitProperty);
        set => SetValue(TimeUnitProperty, value);
    }

    /// <summary>DependencyProperty единиц измерения времени</summary>
    public static readonly DependencyProperty TimeUnitProperty =
        DependencyProperty.Register(
            nameof(TimeUnit),
            typeof(string),
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata("s", FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Шаг времени одного кадра</summary>
    public double TimeStep
    {
        get => (double)GetValue(TimeStepProperty);
        set => SetValue(TimeStepProperty, value);
    }

    /// <summary>DependencyProperty шага времени кадра</summary>
    public static readonly DependencyProperty TimeStepProperty =
        DependencyProperty.Register(
            nameof(TimeStep),
            typeof(double),
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata(0.05d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Направление обновления водопада</summary>
    public WaterfallFlowDirection WaterfallDirection
    {
        get => (WaterfallFlowDirection)GetValue(WaterfallDirectionProperty);
        set => SetValue(WaterfallDirectionProperty, value);
    }

    /// <summary>DependencyProperty направления обновления</summary>
    public static readonly DependencyProperty WaterfallDirectionProperty =
        DependencyProperty.Register(
            nameof(WaterfallDirection),
            typeof(WaterfallFlowDirection),
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata(WaterfallFlowDirection.TopToBottom, FrameworkPropertyMetadataOptions.AffectsRender, OnFlowDirectionChanged));

    /// <summary>Цветовая шкала водопада</summary>
    public WaterfallColorScale ColorScale
    {
        get => (WaterfallColorScale)GetValue(ColorScaleProperty);
        set => SetValue(ColorScaleProperty, value);
    }

    /// <summary>DependencyProperty цветовой шкалы</summary>
    public static readonly DependencyProperty ColorScaleProperty =
        DependencyProperty.Register(
            nameof(ColorScale),
            typeof(WaterfallColorScale),
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata(__DefaultColorScale, FrameworkPropertyMetadataOptions.AffectsRender, OnColorScaleChanged));

    /// <summary>Кисть линий сетки</summary>
    public Brush GridLineBrush
    {
        get => (Brush)GetValue(GridLineBrushProperty);
        set => SetValue(GridLineBrushProperty, value);
    }

    /// <summary>DependencyProperty кисти сетки</summary>
    public static readonly DependencyProperty GridLineBrushProperty =
        DependencyProperty.Register(
            nameof(GridLineBrush),
            typeof(Brush),
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata(Brushes.DimGray, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Толщина линий сетки</summary>
    public double GridLineThickness
    {
        get => (double)GetValue(GridLineThicknessProperty);
        set => SetValue(GridLineThicknessProperty, value);
    }

    /// <summary>DependencyProperty толщины сетки</summary>
    public static readonly DependencyProperty GridLineThicknessProperty =
        DependencyProperty.Register(
            nameof(GridLineThickness),
            typeof(double),
            typeof(SpectrumWaterfallControl),
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
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata(Brushes.WhiteSmoke, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Толщина осей</summary>
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
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata(1.1d, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Кисть фона области построения</summary>
    public Brush PlotBackground
    {
        get => (Brush)GetValue(PlotBackgroundProperty);
        set => SetValue(PlotBackgroundProperty, value);
    }

    /// <summary>DependencyProperty фона области построения</summary>
    public static readonly DependencyProperty PlotBackgroundProperty =
        DependencyProperty.Register(
            nameof(PlotBackground),
            typeof(Brush),
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Количество горизонтальных делений</summary>
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
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata(8, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Количество вертикальных делений</summary>
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
            typeof(SpectrumWaterfallControl),
            new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Инициализирует новый экземпляр SpectrumWaterfallControl</summary>
    public SpectrumWaterfallControl()
    {
        SnapsToDevicePixels = true;
        UseLayoutRounding = true;
        Foreground = Brushes.WhiteSmoke;
        BuildPalette();
    }

    protected override void OnRender(DrawingContext DrawingContext)
    {
        base.OnRender(DrawingContext);

        var plot_rect = GetPlotRect();
        if (plot_rect.Width < 2 || plot_rect.Height < 2)
            return;

        EnsureBuffers(plot_rect);

        DrawingContext.DrawRectangle(PlotBackground, null, plot_rect);

        if (_Bitmap != null)
            DrawingContext.DrawImage(_Bitmap, plot_rect);

        DrawGrid(DrawingContext, plot_rect);
        DrawAxis(DrawingContext, plot_rect);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo SizeInfo)
    {
        base.OnRenderSizeChanged(SizeInfo);
        ResetBuffers();
        InvalidateVisual();
    }

    private void AppendSpectrumFrame(IReadOnlyList<double> Spectrum)
    {
        if (Spectrum.Count == 0)
            return;

        var plot_rect = GetPlotRect();
        if (plot_rect.Width < 2 || plot_rect.Height < 2)
            return;

        EnsureBuffers(plot_rect);

        if (_AmplitudeBuffer is null || _Bitmap is null)
            return;

        var width = _Bitmap.PixelWidth;
        var height = _Bitmap.PixelHeight;

        switch (WaterfallDirection)
        {
            case WaterfallFlowDirection.TopToBottom:
                ShiftRowsDown(width, height);
                WriteRow(0, width, Spectrum);
                break;
            case WaterfallFlowDirection.BottomToTop:
                ShiftRowsUp(width, height);
                WriteRow(height - 1, width, Spectrum);
                break;
            case WaterfallFlowDirection.LeftToRight:
                ShiftColumnsRight(width, height);
                WriteColumn(0, width, height, Spectrum);
                break;
            case WaterfallFlowDirection.RightToLeft:
                ShiftColumnsLeft(width, height);
                WriteColumn(width - 1, width, height, Spectrum);
                break;
        }

        UpdateBitmapPixels();
        InvalidateVisual();
    }

    private void ShiftRowsDown(int Width, int Height)
    {
        if (_AmplitudeBuffer is null || Height <= 1)
            return;

        Array.Copy(_AmplitudeBuffer, 0, _AmplitudeBuffer, Width, Width * (Height - 1));
    }

    private void ShiftRowsUp(int Width, int Height)
    {
        if (_AmplitudeBuffer is null || Height <= 1)
            return;

        Array.Copy(_AmplitudeBuffer, Width, _AmplitudeBuffer, 0, Width * (Height - 1));
    }

    private void ShiftColumnsRight(int Width, int Height)
    {
        if (_AmplitudeBuffer is null || Width <= 1)
            return;

        for (var y = 0; y < Height; y++)
        {
            var row_offset = y * Width;
            Array.Copy(_AmplitudeBuffer, row_offset, _AmplitudeBuffer, row_offset + 1, Width - 1);
        }
    }

    private void ShiftColumnsLeft(int Width, int Height)
    {
        if (_AmplitudeBuffer is null || Width <= 1)
            return;

        for (var y = 0; y < Height; y++)
        {
            var row_offset = y * Width;
            Array.Copy(_AmplitudeBuffer, row_offset + 1, _AmplitudeBuffer, row_offset, Width - 1);
        }
    }

    private void WriteRow(int RowIndex, int Width, IReadOnlyList<double> Spectrum)
    {
        if (_AmplitudeBuffer is null)
            return;

        var row_offset = RowIndex * Width;
        for (var x = 0; x < Width; x++)
        {
            var source_index = ScaleIndex(x, Width, Spectrum.Count);
            _AmplitudeBuffer[row_offset + x] = Spectrum[source_index];
        }
    }

    private void WriteColumn(int ColumnIndex, int Width, int Height, IReadOnlyList<double> Spectrum)
    {
        if (_AmplitudeBuffer is null)
            return;

        for (var y = 0; y < Height; y++)
        {
            var source_index = ScaleIndex(y, Height, Spectrum.Count);
            _AmplitudeBuffer[y * Width + ColumnIndex] = Spectrum[source_index];
        }
    }

    private static int ScaleIndex(int PixelIndex, int PixelCount, int SourceCount)
    {
        if (SourceCount <= 1)
            return 0;

        if (PixelCount <= 1)
            return SourceCount - 1;

        var ratio = PixelIndex / (double)(PixelCount - 1);
        var source_index = (int)Math.Round(ratio * (SourceCount - 1), MidpointRounding.AwayFromZero);
        return source_index < 0 ? 0 : (source_index >= SourceCount ? SourceCount - 1 : source_index);
    }

    private void UpdateBitmapPixels()
    {
        if (_Bitmap is null || _PixelBuffer is null || _AmplitudeBuffer is null)
            return;

        var range = AmplitudeMaximum - AmplitudeMinimum;
        if (range <= 1e-12)
            range = 1;

        for (var index = 0; index < _AmplitudeBuffer.Length; index++)
        {
            var normalized = (_AmplitudeBuffer[index] - AmplitudeMinimum) / range;
            normalized = normalized < 0 ? 0 : (normalized > 1 ? 1 : normalized);
            var palette_index = (int)(normalized * (_Palette.Length - 1));
            _PixelBuffer[index] = _Palette[palette_index];
        }

        _Bitmap.WritePixels(new Int32Rect(0, 0, _Bitmap.PixelWidth, _Bitmap.PixelHeight), _PixelBuffer, _Bitmap.BackBufferStride, 0);
    }

    private void DrawGrid(DrawingContext DrawingContext, Rect PlotRect)
    {
        var horizontal_count = Math.Max(1, HorizontalGridDivisions);
        var vertical_count = Math.Max(1, VerticalGridDivisions);
        var grid_pen = new Pen(GridLineBrush, Math.Max(0.1, GridLineThickness)) { DashStyle = DashStyles.Dot };

        var vertical_time = IsVerticalTimeDirection();

        for (var index = 0; index <= horizontal_count; index++)
        {
            var t = index / (double)horizontal_count;
            var y = PlotRect.Top + PlotRect.Height * t;
            DrawingContext.DrawLine(grid_pen, new(PlotRect.Left, y), new(PlotRect.Right, y));

            var value = vertical_time ? GetTimeByVerticalRatio(t, PlotRect.Height) : Lerp(FrequencyMinimum, FrequencyMaximum, t);
            var unit = vertical_time ? TimeUnit : FrequencyUnit;
            DrawText(DrawingContext, $"{value:0.##} {unit}", new(3, y - 8), TextAlignment.Left);
        }

        for (var index = 0; index <= vertical_count; index++)
        {
            var t = index / (double)vertical_count;
            var x = PlotRect.Left + PlotRect.Width * t;
            DrawingContext.DrawLine(grid_pen, new(x, PlotRect.Top), new(x, PlotRect.Bottom));

            var value = vertical_time ? Lerp(FrequencyMinimum, FrequencyMaximum, t) : GetTimeByHorizontalRatio(t, PlotRect.Width);
            var unit = vertical_time ? FrequencyUnit : TimeUnit;
            DrawText(DrawingContext, $"{value:0.##} {unit}", new(x, PlotRect.Bottom + 4), TextAlignment.Center);
        }
    }

    private void DrawAxis(DrawingContext DrawingContext, Rect PlotRect)
    {
        var axis_pen = new Pen(AxisBrush, Math.Max(0.1, AxisThickness));
        DrawingContext.DrawRectangle(null, axis_pen, PlotRect);

        if (IsVerticalTimeDirection())
        {
            DrawText(DrawingContext, $"t, {TimeUnit}", new(4, PlotRect.Top - 11), TextAlignment.Left);
            DrawText(DrawingContext, $"f, {FrequencyUnit}", new(PlotRect.Right - 2, PlotRect.Bottom + 16), TextAlignment.Right);
            return;
        }

        DrawText(DrawingContext, $"f, {FrequencyUnit}", new(4, PlotRect.Top - 11), TextAlignment.Left);
        DrawText(DrawingContext, $"t, {TimeUnit}", new(PlotRect.Right - 2, PlotRect.Bottom + 16), TextAlignment.Right);
    }

    private double GetTimeByVerticalRatio(double Ratio, double Height)
    {
        var total_time = Math.Max(0, (Height - 1) * Math.Max(TimeStep, 1e-6));
        return WaterfallDirection == WaterfallFlowDirection.TopToBottom
            ? Lerp(0, total_time, Ratio)
            : Lerp(total_time, 0, Ratio);
    }

    private double GetTimeByHorizontalRatio(double Ratio, double Width)
    {
        var total_time = Math.Max(0, (Width - 1) * Math.Max(TimeStep, 1e-6));
        return WaterfallDirection == WaterfallFlowDirection.LeftToRight
            ? Lerp(0, total_time, Ratio)
            : Lerp(total_time, 0, Ratio);
    }

    private bool IsVerticalTimeDirection() =>
        WaterfallDirection is WaterfallFlowDirection.TopToBottom or WaterfallFlowDirection.BottomToTop;

    private void EnsureBuffers(Rect PlotRect)
    {
        var width = Math.Max(1, (int)Math.Round(PlotRect.Width));
        var height = Math.Max(1, (int)Math.Round(PlotRect.Height));

        if (_Bitmap?.PixelWidth == width && _Bitmap.PixelHeight == height)
            return;

        _Bitmap = new(width, height, 96, 96, PixelFormats.Bgra32, null);
        _AmplitudeBuffer = new double[width * height];
        _PixelBuffer = new int[width * height];
        UpdateBitmapPixels();
    }

    private void ResetBuffers()
    {
        _Bitmap = null;
        _AmplitudeBuffer = null;
        _PixelBuffer = null;
    }

    private Rect GetPlotRect() =>
        new(
            LeftMargin,
            TopMargin,
            Math.Max(1d, ActualWidth - LeftMargin - RightMargin),
            Math.Max(1d, ActualHeight - TopMargin - BottomMargin));

    private void BuildPalette()
    {
        var color_scale = ColorScale ?? __DefaultColorScale;
        var palette = new int[1024];

        for (var index = 0; index < palette.Length; index++)
        {
            var color = color_scale.GetColor(index / (double)(palette.Length - 1));
            palette[index] = color.B | (color.G << 8) | (color.R << 16) | (255 << 24);
        }

        _Palette = palette;
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

    private static void OnCurrentSpectrumChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        if (D is not SpectrumWaterfallControl control)
            return;

        var spectrum = ToSpectrumArray(E.NewValue as IEnumerable<double>);
        if (spectrum.Length == 0)
            return;

        control.AppendSpectrumFrame(spectrum);
    }

    private static void OnColorScaleChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        if (D is not SpectrumWaterfallControl control)
            return;

        control.BuildPalette();
        control.UpdateBitmapPixels();
        control.InvalidateVisual();
    }

    private static void OnFlowDirectionChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        if (D is not SpectrumWaterfallControl control)
            return;

        control.ResetBuffers();
        control.InvalidateVisual();
    }

    private static void OnAmplitudeScaleChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
    {
        if (D is not SpectrumWaterfallControl control)
            return;

        control.UpdateBitmapPixels();
        control.InvalidateVisual();
    }

    private static double[] ToSpectrumArray(IEnumerable<double>? Source)
    {
        if (Source is null)
            return [];

        return Source switch
        {
            double[] values => values,
            IReadOnlyCollection<double> values => [.. values],
            _ => [.. Source]
        };
    }
}
