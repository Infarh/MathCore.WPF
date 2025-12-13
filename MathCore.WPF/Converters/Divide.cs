using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь деления значения на вещественное число</summary>
/// <remarks>
/// Выполняет арифметическое деление входного значения на заданный делитель.
/// <para><b>Формула Convert:</b> result = value / K</para>
/// <para><b>Формула ConvertBack:</b> value = result * K</para>
/// <para><b>ConvertBack:</b> Полностью поддерживается. Операция деления обратима через умножение.
/// При K = 0 прямое преобразование вернёт Infinity или NaN.
/// При двусторонней привязке корректно восстанавливает исходное значение.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Уменьшить значение в 2 раза --&gt;
/// &lt;TextBlock Text="{Binding Value, Converter={converters:Divide K=2}}" /&gt;
/// 
/// &lt;!-- Двусторонняя привязка с делением --&gt;
/// &lt;Slider Value="{Binding Centimeters, Converter={converters:Divide 100}, Mode=TwoWay}" /&gt;
/// 
/// &lt;!-- Преобразование процентов (деление на 100) --&gt;
/// &lt;ProgressBar Value="{Binding Percent, Converter={converters:Divide 100}}" /&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(Divide))]
// ReSharper disable once UnusedType.Global
public class Divide(double K) : SimpleDoubleValueConverter(K, (v, k) => v / k, (r, k) => r * k)
{
    public Divide() : this(1) { }
}