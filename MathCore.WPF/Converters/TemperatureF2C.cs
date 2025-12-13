using System.Windows.Markup;

namespace MathCore.WPF.Converters;


/// <summary>Конвертер преобразования температуры из Фаренгейта в Цельсий</summary>
[MarkupExtensionReturnType(typeof(TemperatureF2C))]
public class TemperatureF2C() : Linear(1.8, 32);