using System.Windows.Markup;

namespace MathCore.WPF.Converters;

/// <summary>Конвертер температуры из градусов Фаренгейта в градусы Цельсия</summary>
/// <remarks>
/// Использует линейное преобразование с коэффициентами K = 1.8, B = 32.
/// Прямое преобразование: F = 1.8 * C + 32
/// Обратное преобразование (ConvertBack): C = (F - 32) / 1.8
/// Поддерживает двустороннее связывание для конвертации в обе стороны.
/// <example>
/// <code>
/// &lt;!-- Привязка: значение в Фаренгейтах в модели, отображение в Цельсиях --&gt;
/// &lt;TextBox Text="{Binding FahrenheitTemp, Converter={converters:TemperatureF2C}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(TemperatureF2C))]
public class TemperatureF2C() : Linear(1.8, 32);