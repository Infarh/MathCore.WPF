using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Converters;


/// <summary>Мультиконвертер вычисления среднего арифметического нескольких значений</summary>
[MarkupExtensionReturnType(typeof(AverageMulti))]
public class AverageMulti() : MultiValueValueConverter
{
    protected override object? Convert(object?[]? vv, Type? t, object? p, CultureInfo? c)
    {
        switch (vv)
        {
            case null:
                return null;
            case [null]:
                return double.NaN;
        }

        var v = vv[0] is double d ? d : System.Convert.ToDouble(vv[0]);

        for (var i = 1; i < vv.Length; i++)
        {
            if (vv[i] is null) return double.NaN;

            if (!DoubleValueConverter.TryConvertToDouble(vv[i], c, out var value))
                return double.NaN;

            v += value;
        }

        return v / vv.Length;
    }
}