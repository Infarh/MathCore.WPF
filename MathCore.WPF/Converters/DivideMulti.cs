using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

/// <summary>Выполняет последовательное деление элементов массива</summary>
[MarkupExtensionReturnType(typeof(DivideMulti))]
public class DivideMulti : MultiValueValueConverter
{
    /// <summary>Выполняет последовательное деление элементов массива</summary>
    /// <remarks>Возвращает NaN при некорректных входах; при делении на ноль возвращает Infinity в зависимости от знака</remarks>
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
            if (vv[i] is null or double.NaN) return double.NaN;
            if (!DoubleValueConverter.TryConvertToDouble(vv[i], c, out var div))
                return double.NaN;
            if (div == 0)
                return v == 0
                    ? double.NaN
                    : v > 0
                        ? double.PositiveInfinity
                        : double.NegativeInfinity;
            v /= div;
        }

        return v;
    }
}