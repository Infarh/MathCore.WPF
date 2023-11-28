using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(DivideMulti))]
public class DivideMulti : MultiValueValueConverter
{
    protected override object? Convert(object?[]? vv, Type? t, object? p, CultureInfo? c)
    {
        if (vv is null) return null;
        if (vv is [null]) return double.NaN;

        var v = vv[0] is double d ? d : System.Convert.ToDouble(vv[0]);

        for (var i = 1; i < vv.Length; i++)
        {
            if (vv[i] is null or double.NaN) return double.NaN;
            var div = vv[i] is double dv ? dv : System.Convert.ToDouble(vv[i]);
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