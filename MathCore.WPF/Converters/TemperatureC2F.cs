using System.Windows.Markup;

namespace MathCore.WPF.Converters;

/// <summary>Конвертер температуры из градусов Цельсия в градусы Фаренгейта</summary>
/// <remarks>
/// Выполняет преобразование: F = C / 1.8 - 32 / 1.8.
/// Поддерживает двустороннее связывание для конвертации в обе стороны.
/// <example>
/// <code>
/// &lt;TextBox Text="{Binding CelsiusTemp, Converter={converters:TemperatureC2F}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(TemperatureC2F))]
public class TemperatureC2F() : Linear(1 / 1.8, -32 / 1.8);