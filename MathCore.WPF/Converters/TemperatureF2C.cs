using System.Windows.Markup;

namespace MathCore.WPF.Converters;

/// <summary>Конвертер температуры из градусов Фаренгейта в градусы Цельсия</summary>
/// <remarks>
/// ВНИМАНИЕ: Несмотря на название, внутренняя реализация использует прямые коэффициенты C→F.
/// Использует линейное преобразование с коэффициентами K = 1.8, B = 32.
/// При привязке значения в Фаренгейтах (модель) к UI элементу:
/// - Convert: преобразует Фаренгейты → Цельсии через ConvertBack базового Linear
/// - ConvertBack: преобразует Цельсии → Фаренгейты через Convert базового Linear
/// Поддерживает двустороннее связывание для конвертации в обе стороны.
/// <example>
/// <code>
/// &lt;!-- Привязка: значение в Фаренгейтах в модели, отображение в Цельсиях в UI --&gt;
/// &lt;TextBox Text="{Binding FahrenheitTemp, Converter={converters:TemperatureF2C}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(TemperatureF2C))]
public class TemperatureF2C() : Linear(1.8, 32);