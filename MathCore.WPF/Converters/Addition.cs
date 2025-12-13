using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразователь сложения значения с вещественным числом</summary>
/// <remarks>
/// Выполняет арифметическое сложение входного значения с заданным параметром.
/// <para><b>Формула Convert:</b> result = value + P</para>
/// <para><b>Формула ConvertBack:</b> value = result - P</para>
/// <para><b>ConvertBack:</b> Полностью поддерживается. Операция сложения обратима через вычитание.
/// При двусторонней привязке корректно восстанавливает исходное значение.</para>
/// </remarks>
/// <example>
/// <code language="xaml">
/// &lt;!-- Увеличить значение на 10 --&gt;
/// &lt;TextBlock Text="{Binding Value, Converter={converters:Addition P=10}}" /&gt;
/// 
/// &lt;!-- Двусторонняя привязка с добавлением смещения --&gt;
/// &lt;Slider Value="{Binding Temperature, Converter={converters:Addition 273.15}, Mode=TwoWay}" /&gt;
/// </code>
/// </example>
[MarkupExtensionReturnType(typeof(Addition))]
public class Addition(double P) : SimpleDoubleValueConverter(P, (v, p) => v + p, (r, p) => r - p)
{
    public Addition() : this(0) { }
}