using System.Windows;
using System.Windows.Media;

namespace MathCore.WPF.Controls;

/// <summary>Описание одной линии спектра</summary>
public class SpectrumSeries
{
    /// <summary>Точки графика спектра в формате Frequency/AmplitudeDb</summary>
    public IEnumerable<Point>? Points { get; set; }

    /// <summary>Кисть линии спектра</summary>
    public Brush? Stroke { get; set; }

    /// <summary>Толщина линии спектра</summary>
    public double Thickness { get; set; } = 1.5;

    /// <summary>Штриховой стиль линии спектра</summary>
    public DoubleCollection? DashArray { get; set; }
}
