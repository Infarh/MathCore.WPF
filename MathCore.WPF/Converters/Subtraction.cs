using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь вычитания вещественного числа из значения</summary>
/// <remarks>
/// Выполняет арифметическое вычитание заданного параметра из входного значения.
/// <para><b>Формула Convert:</b> result = value - P</para>
/// <para><b>Формула ConvertBack:</b> value = result + P</para>
/// <para><b>ConvertBack:</b> Полностью поддерживается. Операция вычитания обратима через сложение.
/// При двусторонней привязке корректно восстанавливает исходное значение.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Уменьшить значение на 5 --&gt;
/// &lt;TextBlock Text="{Binding Value, Converter={converters:Subtraction P=5}}" /&gt;
/// 
/// &lt;!-- Двусторонняя привязка с вычитанием смещения --&gt;
/// &lt;Slider Value="{Binding Year, Converter={converters:Subtraction 2000}, Mode=TwoWay}" /&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(Subtraction))]
public class Subtraction(double P) : SimpleDoubleValueConverter(P, (v, p) => v - p, (r, p) => r + p)
{
    public Subtraction() : this(0) { }
}