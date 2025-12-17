using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Инвертирует логическое значение</summary>
[MarkupExtensionReturnType(typeof(Not))]
[ValueConversion(typeof(bool), typeof(bool))]
public class Not : ValueConverter
{
    /// <summary>Инвертирует логическое значение</summary>
    /// <param name="v">Входное значение</param>
    /// <param name="t">Тип целевого значения</param>
    /// <param name="p">Параметр преобразования</param>
    /// <param name="c">Культура</param>
    /// <returns>Инвертированное булево значение или Binding.DoNothing при некорректном входе</returns>
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => v is bool b ? !b : Binding.DoNothing;

    /// <summary>Инвертирует логическое значение при обратном преобразовании</summary>
    /// <param name="v">Входное значение</param>
    /// <param name="t">Тип исходного значения</param>
    /// <param name="p">Параметр преобразования</param>
    /// <param name="c">Культура</param>
    /// <returns>Инвертированное булево значение или Binding.DoNothing при некорректном входе</returns>
    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => v is bool b ? !b : Binding.DoNothing;
}