using System.Windows.Markup;

namespace MathCore.WPF.Converters;

/// <summary>Конвертер температуры из градусов Цельсия в градусы Фаренгейта</summary>
/// <remarks>
/// ВНИМАНИЕ: Несмотря на название, внутренняя реализация использует обратные коэффициенты.
/// Использует линейное преобразование с коэффициентами K = 1/1.8 ≈ 0.556, B = -32/1.8 ≈ -17.78.
/// При привязке значения в Цельсиях (модель) к UI элементу:
/// - Convert: преобразует Цельсии → Фаренгейты через ConvertBack базового Linear
/// - ConvertBack: преобразует Фаренгейты → Цельсии через Convert базового Linear
/// Поддерживает двустороннее связывание для конвертации в обе стороны.
/// <example>
/// <code>
/// &lt;!-- Привязка: значение в Цельсиях в модели, отображение в Фаренгейтах в UI --&gt;
/// &lt;TextBox Text="{Binding CelsiusTemp, Converter={converters:TemperatureC2F}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(TemperatureC2F))]
public class TemperatureC2F() : Linear(1 / 1.8, -32 / 1.8);