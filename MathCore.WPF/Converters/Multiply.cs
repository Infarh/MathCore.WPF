using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь умножения значения на вещественное число</summary>
/// <remarks>
/// Выполняет операцию умножения: result = value * K.
/// Поддерживает двустороннее связывание: ConvertBack выполняет деление result / K.
/// <example>
/// <code>
/// &lt;Slider Maximum="{Binding BaseValue, Converter={converters:Multiply K=2}}" /&gt;
/// &lt;TextBox Text="{Binding Scale, Converter={converters:Multiply 100}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(Multiply))]
public class Multiply(double K) : SimpleDoubleValueConverter(K, (v, k) => v * k, (r, k) => r / k)
{
    /// <summary>Инициализирует конвертер с коэффициентом 1</summary>
    public Multiply() : this(1) { }
}