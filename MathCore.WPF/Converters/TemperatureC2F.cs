using System.Windows.Markup;

namespace MathCore.WPF.Converters;


/// <summary>Конвертер преобразования температуры из Цельсия в Фаренгейт</summary>
/// <remarks>Формула: F = C * 9/5 + 32 = C * 1.8 + 32</remarks>
[MarkupExtensionReturnType(typeof(TemperatureC2F))]
public class TemperatureC2F() : Linear(1.8, 32);