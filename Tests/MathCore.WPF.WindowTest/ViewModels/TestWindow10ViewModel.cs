using System.Windows.Threading;

using MathCore.WPF.Controls;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

internal class TestWindow10ViewModel : TitledViewModel, IDisposable
{
    private readonly DispatcherTimer _Timer;
    private readonly Random _Random = new(17);

    private double _Time;

    private double[] _CurrentSpectrum = [];
    private WaterfallFlowDirection _FlowDirection = WaterfallFlowDirection.TopToBottom;

    /// <summary>Текущий кадр спектра</summary>
    public double[] CurrentSpectrum { get => _CurrentSpectrum; private set => Set(ref _CurrentSpectrum, value); }

    /// <summary>Минимум амплитудной шкалы в дБ</summary>
    public double AmplitudeMinimum { get; } = -110;

    /// <summary>Максимум амплитудной шкалы в дБ</summary>
    public double AmplitudeMaximum { get; } = 10;

    /// <summary>Минимальная частота</summary>
    public double FrequencyMinimum { get; } = 20;

    /// <summary>Максимальная частота</summary>
    public double FrequencyMaximum { get; } = 24000;

    /// <summary>Шаг времени кадра</summary>
    public double TimeStep { get; } = 0.05;

    /// <summary>Единицы измерения частоты</summary>
    public string FrequencyUnit { get; } = "Hz";

    /// <summary>Единицы измерения времени</summary>
    public string TimeUnit { get; } = "s";

    /// <summary>Текущее направление водопада</summary>
    public WaterfallFlowDirection FlowDirection { get => _FlowDirection; set => Set(ref _FlowDirection, value); }

    /// <summary>Доступные направления водопада</summary>
    public IReadOnlyList<WaterfallFlowDirection> Directions { get; } =
    [
        WaterfallFlowDirection.TopToBottom,
        WaterfallFlowDirection.BottomToTop,
        WaterfallFlowDirection.LeftToRight,
        WaterfallFlowDirection.RightToLeft
    ];

    public TestWindow10ViewModel() : base("Тест спектрального водопада")
    {
        _Timer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(TimeStep * 1000)
        };
        _Timer.Tick += OnTimerTick;

        CurrentSpectrum = BuildSpectrumFrame();
        _Timer.Start();
    }

    private void OnTimerTick(object? Sender, EventArgs E)
    {
        _Time += TimeStep;
        CurrentSpectrum = BuildSpectrumFrame();
    }

    private double[] BuildSpectrumFrame()
    {
        const int point_count = 1024;
        var data = new double[point_count];

        for (var index = 0; index < point_count; index++)
        {
            var ratio = index / (double)(point_count - 1);
            var frequency = FrequencyMinimum + ratio * (FrequencyMaximum - FrequencyMinimum);

            var peak_1_center = 1200 + 350 * Math.Sin(_Time * 2.3);
            var peak_2_center = 5500 + 1200 * Math.Cos(_Time * 1.1);
            var peak_3_center = 12000 + 2600 * Math.Sin(_Time * 0.6 + 1.4);

            var peak_1 = 44 * Math.Exp(-Math.Pow((frequency - peak_1_center) / 520, 2));
            var peak_2 = 36 * Math.Exp(-Math.Pow((frequency - peak_2_center) / 780, 2));
            var peak_3 = 30 * Math.Exp(-Math.Pow((frequency - peak_3_center) / 1250, 2));

            var noise = (_Random.NextDouble() - 0.5) * 2.8;
            var floor = -98 + 2.2 * Math.Sin(_Time * 4 + ratio * 40);

            data[index] = floor + peak_1 + peak_2 + peak_3 + noise;
        }

        return data;
    }

    public void Dispose()
    {
        _Timer.Stop();
        _Timer.Tick -= OnTimerTick;
    }
}
