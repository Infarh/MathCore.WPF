using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь сложения значения с вещественным числом</summary>
/// <remarks>
/// Выполняет операцию сложения: result = value + P.
/// Поддерживает двустороннее связывание: ConvertBack выполняет вычитание result - P.
/// <example>
/// <code>
/// &lt;Slider Value="{Binding Temperature, Converter={converters:Addition P=273.15}}" /&gt;
/// &lt;TextBox Text="{Binding Offset, Converter={converters:Addition 100}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(Addition))]
public class Addition(double P) : SimpleDoubleValueConverter(P, (v, p) => v + p, (r, p) => r - p)
{
    /// <summary>Инициализирует конвертер с нулевым смещением</summary>
    public Addition() : this(0) { }
}