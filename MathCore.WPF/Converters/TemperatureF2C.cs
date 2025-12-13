using System.Windows.Markup;

namespace MathCore.WPF.Converters;


/// <summary>Конвертер преобразования температуры из Фаренгейта в Цельсий</summary>
/// <remarks>Формула: C = (F - 32) * 5/9 = (F - 32) / 1.8</remarks>
[MarkupExtensionReturnType(typeof(TemperatureF2C))]
public class TemperatureF2C() : Linear(1 / 1.8, -32 / 1.8);