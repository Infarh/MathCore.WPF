using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Конвертер логической инверсии булева значения</summary>
/// <remarks>
/// Выполняет логическую операцию НЕ (NOT) над входным булевым значением
/// <para>Возвращаемые значения:</para>
/// <list type="bullet">
/// <item><description>true → false</description></item>
/// <item><description>false → true</description></item>
/// <item><description>null → null</description></item>
/// </list>
/// <para>Поддерживает двустороннее преобразование через ConvertBack с аналогичной логикой</para>
/// </remarks>
/// <example>
/// <code language="xaml"><![CDATA[
/// <!-- Инверсия видимости элемента -->
/// <Button Visibility="{Binding IsEnabled, Converter={x:Static converters:Not}}" />
/// 
/// <!-- Инверсия для IsEnabled -->
/// <TextBox IsEnabled="{Binding IsReadOnly, Converter={x:Static converters:Not}}" />
/// ]]></code>
/// </example>
[MarkupExtensionReturnType(typeof(Not))]
[ValueConversion(typeof(bool), typeof(bool))]
public class Not : ValueConverter
{
    /// <inheritdoc />
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => !(bool?) v;

    /// <inheritdoc />
    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => !(bool?)v;
}