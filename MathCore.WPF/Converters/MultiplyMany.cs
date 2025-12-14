using System.Globalization;
using System.Windows.Markup;
using System.Windows.Data;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Умножает последовательность числовых значений</summary>
[MarkupExtensionReturnType(typeof(MultiplyMany))]
public class MultiplyMany : MultiValueValueConverter
{
    /// <summary>Преобразует массив значений, перемножая их последовательно</summary>
    /// <param name="vv">Входные значения</param>
    /// <param name="t">Тип целевого значения</param>
    /// <param name="p">Параметр</param>
    /// <param name="c">Культура</param>
    /// <returns>
    /// Произведение значений; null если вход null; double.NaN если первый элемент null или один из элементов не может быть конвертирован
    /// </returns>
    protected override object? Convert(object?[]? vv, Type? t, object? p, CultureInfo? c)
    {
        switch (vv)
        {
            case null:
                return null;
            case [null]:
                return double.NaN;
        }

        if (!DoubleValueConverter.TryConvertToDouble(vv[0], c, out var value))
            return double.NaN;
        var v = value;

        for (var i = 1; i < vv.Length; i++)
        {
            if (vv[i] is null) return double.NaN;
            if (!DoubleValueConverter.TryConvertToDouble(vv[i], c, out value))
                return double.NaN;
            v *= value;
        }

        return v;
    }
}