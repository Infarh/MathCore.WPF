using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь вычитания вещественного числа из значения</summary>
/// <remarks>
/// Выполняет операцию вычитания: result = value - P.
/// Поддерживает двустороннее связывание: ConvertBack выполняет сложение result + P.
/// <example>
/// <code>
/// &lt;Slider Value="{Binding Height, Converter={converters:Subtraction P=10}}" /&gt;
/// &lt;ProgressBar Value="{Binding Progress, Converter={converters:Subtraction 50}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(Subtraction))]
public class Subtraction(double P) : SimpleDoubleValueConverter(P, (v, p) => v - p, (r, p) => r + p)
{
    /// <summary>Инициализирует конвертер с нулевым смещением</summary>
    public Subtraction() : this(0) { }
}