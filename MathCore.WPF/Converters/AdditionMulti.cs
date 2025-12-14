using System.Globalization;
using System.Windows.Markup;
using System.Windows.Data;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Суммирует последовательность числовых значений</summary>
[MarkupExtensionReturnType(typeof(AdditionMulti))]
public class AdditionMulti : MultiValueValueConverter
{
    /// <summary>Преобразует массив значений, суммируя их последовательно</summary>
    /// <returns>Сумма значений; null если вход null; double.NaN если один из элементов не может быть преобразован</returns>
    protected override object? Convert(object?[]? vv, Type? t, object? p, CultureInfo? c)
    {
        switch (vv)
        {
            case null:
                return null;
            case [null]:
                return double.NaN;
        }

        if (!DoubleValueConverter.TryConvertToDouble(vv[0], c, out var v))
            return double.NaN;

        for (var i = 1; i < vv.Length; i++)
        {
            if (vv[i] is null) return double.NaN;
            if (!DoubleValueConverter.TryConvertToDouble(vv[i], c, out var value))
                return double.NaN;

            v += value;
        }

        return v;
    }
}