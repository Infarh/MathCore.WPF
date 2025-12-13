using System.Windows.Markup;

namespace MathCore.WPF.Converters;

/// <summary>Конвертер температуры из градусов Фаренгейта в градусы Цельсия</summary>
/// <remarks>
/// Выполняет преобразование: F = 1.8 * C + 32.
/// Поддерживает двустороннее связывание для конвертации в обе стороны.
/// <example>
/// <code>
/// &lt;TextBox Text="{Binding FahrenheitTemp, Converter={converters:TemperatureF2C}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(TemperatureF2C))]
public class TemperatureF2C() : Linear(1.8, 32);