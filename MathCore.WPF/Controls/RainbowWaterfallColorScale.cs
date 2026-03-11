using System.Windows.Media;

namespace MathCore.WPF.Controls;

/// <summary>Цветовая шкала водопада от темного синего к белому через теплые оттенки</summary>
public sealed class RainbowWaterfallColorScale : WaterfallColorScale
{
    private static readonly (double Position, Color Color)[] __ColorStops =
    [
        (0.00, Color.FromRgb(0, 0, 0)),
        (0.14, Color.FromRgb(0, 0, 52)),
        (0.45, Color.FromRgb(180, 0, 0)),
        (0.66, Color.FromRgb(255, 120, 0)),
        (0.84, Color.FromRgb(255, 220, 0)),
        (1.00, Color.FromRgb(255, 255, 255))
    ];

    /// <summary>Возвращает цвет по нормализованной амплитуде от 0 до 1</summary>
    /// <param name="NormalizedValue">Нормализованная амплитуда</param>
    /// <returns>Цвет точки водопада</returns>
    public override Color GetColor(double NormalizedValue)
    {
        var normalized_value = Clamp01(NormalizedValue);

        for (var index = 1; index < __ColorStops.Length; index++)
        {
            var left_stop = __ColorStops[index - 1];
            var right_stop = __ColorStops[index];

            if (normalized_value > right_stop.Position)
                continue;

            var span = right_stop.Position - left_stop.Position;
            var ratio = span <= 1e-12 ? 0 : (normalized_value - left_stop.Position) / span;
            return Interpolate(left_stop.Color, right_stop.Color, ratio);
        }

        return __ColorStops[^1].Color;
    }

    /// <summary>Интерполирует цвет между двумя контрольными точками</summary>
    /// <param name="Left">Левый цвет</param>
    /// <param name="Right">Правый цвет</param>
    /// <param name="Ratio">Коэффициент интерполяции</param>
    /// <returns>Результирующий цвет</returns>
    private static Color Interpolate(Color Left, Color Right, double Ratio)
    {
        var ratio = Clamp01(Ratio);
        return Color.FromArgb(
            255,
            (byte)(Left.R + (Right.R - Left.R) * ratio),
            (byte)(Left.G + (Right.G - Left.G) * ratio),
            (byte)(Left.B + (Right.B - Left.B) * ratio));
    }
}
