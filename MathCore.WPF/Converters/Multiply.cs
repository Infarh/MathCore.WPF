using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь умножения значения на вещественное число</summary>
/// <remarks>
/// Выполняет арифметическое умножение входного значения на заданный коэффициент.
/// <para><b>Формула Convert:</b> result = value * K</para>
/// <para><b>Формула ConvertBack:</b> value = result / K</para>
/// <para><b>ConvertBack:</b> Полностью поддерживается при K ≠ 0. Операция умножения обратима через деление.
/// При K = 0 обратное преобразование вернёт Infinity или NaN.
/// При двусторонней привязке корректно восстанавливает исходное значение.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Увеличить значение в 2 раза --&gt;
/// &lt;TextBlock Text="{Binding Value, Converter={converters:Multiply K=2}}" /&gt;
/// 
/// &lt;!-- Двусторонняя привязка с масштабированием --&gt;
/// &lt;Slider Value="{Binding Meters, Converter={converters:Multiply 100}, Mode=TwoWay}" /&gt;
/// 
/// &lt;!-- Инвертировать знак --&gt;
/// &lt;TextBlock Text="{Binding Value, Converter={converters:Multiply -1}}" /&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(Multiply))]
public class Multiply(double K) : SimpleDoubleValueConverter(K, (v, k) => v * k, (r, k) => r / k)
{
    public Multiply() : this(1) { }
}