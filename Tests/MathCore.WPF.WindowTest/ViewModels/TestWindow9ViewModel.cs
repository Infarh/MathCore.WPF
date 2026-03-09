using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

using MathCore.WPF.Controls;
using MathCore.WPF.ViewModels;

namespace MathCore.WPF.WindowTest.ViewModels;

internal class TestWindow9ViewModel() : TitledViewModel("Тест спектра")
{
    public ObservableCollection<SpectrumSeries> Spectra { get; } =
    [
        new()
        {
            Stroke = Brushes.DeepSkyBlue,
            Thickness = 2,
            Points = CreateSpectrum(0, 1)
        },
        new()
        {
            Stroke = Brushes.Orange,
            Thickness = 1.8,
            DashArray = new DoubleCollection([5d, 2d]),
            Points = CreateSpectrum(0.35, 0.85)
        },
        new()
        {
            Stroke = Brushes.MediumSpringGreen,
            Thickness = 1.6,
            DashArray = new DoubleCollection([2d, 2d]),
            Points = CreateSpectrum(0.6, 0.7)
        }
    ];

    public double AmplitudeMinimum { get; } = -110;

    public double AmplitudeMaximum { get; } = 10;

    public double FrequencyMinimum { get; } = 20;

    public double FrequencyMaximum { get; } = 20000;

    public string FrequencyUnit { get; } = "Hz";

    private static IEnumerable<Point> CreateSpectrum(double Shift, double Gain)
    {
        for (var index = 0; index <= 500; index++)
        {
            var ratio = index / 500d;
            var frequency = 20d * Math.Pow(1000, ratio);

            var peak_1 = 48 * Math.Exp(-Math.Pow((Math.Log10(frequency) - 2.2 - Shift * 0.2) / 0.16, 2));
            var peak_2 = 32 * Math.Exp(-Math.Pow((Math.Log10(frequency) - 3.45 + Shift * 0.15) / 0.18, 2));
            var floor = -96 + 5 * Math.Sin(ratio * 25 + Shift * 3);

            yield return new(frequency, floor + Gain * (peak_1 + peak_2));
        }
    }
}
