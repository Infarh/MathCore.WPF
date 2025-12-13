using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь деления значения на вещественное число</summary>
/// <remarks>
/// Выполняет операцию деления: result = value / K.
/// Поддерживает двустороннее связывание: ConvertBack выполняет умножение result * K.
/// <example>
/// <code>
/// &lt;Slider Value="{Binding TotalValue, Converter={converters:Divide K=100}}" /&gt;
/// &lt;ProgressBar Maximum="{Binding Size, Converter={converters:Divide 2}}" /&gt;
/// </code>
/// </example>
/// </remarks>
[MarkupExtensionReturnType(typeof(Divide))]
// ReSharper disable once UnusedType.Global
public class Divide(double K) : SimpleDoubleValueConverter(K, (v, k) => v / k, (r, k) => r * k)
{
    /// <summary>Инициализирует конвертер с делителем 1</summary>
    public Divide() : this(1) { }
}