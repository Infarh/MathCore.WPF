using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Проверяет что все последующие значения больше первого значения</summary>
[MarkupExtensionReturnType(typeof(LessThanMulti))]
public class LessThanMulti : MultiValueValueConverter
{
    /// <summary>Преобразует массив значений в логическое значение</summary>
    /// <param name="vv">Входные значения, первый элемент используется как эталон</param>
    /// <param name="t">Тип целевого значения</param>
    /// <param name="p">Параметр преобразования</param>
    /// <param name="c">Культура</param>
    /// <returns>
    /// true если все последующие значения строго больше первого;
    /// false если найдено значение меньше или равное первому;
    /// Binding.DoNothing при некорректных входных данных
    /// </returns>
    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
    {
        if (vv is not { Length: > 1 })
            return Binding.DoNothing;

        if (!DoubleValueConverter.TryConvertToDouble(vv[0], c, out var first_value))
            return Binding.DoNothing;

        for (var i = 1; i < vv.Length; i++)
        {
            if (!DoubleValueConverter.TryConvertToDouble(vv[i], c, out var value))
                return Binding.DoNothing;
            if (value <= first_value)
                return false;
        }

        return true;
    }
}