using System.Windows.Media;

namespace MathCore.WPF.Controls;

/// <summary>Базовая цветовая шкала для спектрального водопада</summary>
public abstract class WaterfallColorScale
{
    /// <summary>Возвращает цвет по нормализованной амплитуде от 0 до 1</summary>
    /// <param name="NormalizedValue">Нормализованная амплитуда</param>
    /// <returns>Цвет точки водопада</returns>
    public abstract Color GetColor(double NormalizedValue);

    /// <summary>Ограничивает значение интервалом от 0 до 1</summary>
    /// <param name="Value">Проверяемое значение</param>
    /// <returns>Ограниченное значение</returns>
    protected static double Clamp01(double Value) => Value < 0 ? 0 : (Value > 1 ? 1 : Value);
}
