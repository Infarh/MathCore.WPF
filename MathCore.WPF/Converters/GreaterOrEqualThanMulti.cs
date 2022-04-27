using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(GreaterThanMulti))]
public class GreaterOrEqualThanMulti : MultiValueValueConverter
{
    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
    {
        if (vv is not { Length: > 1 })
            return Binding.DoNothing;

        if (!DoubleValueConverter.TryConvertToDouble(vv[0], out var first_value))
            return Binding.DoNothing;

        for (var i = 1; i < vv.Length; i++)
        {
            if (!DoubleValueConverter.TryConvertToDouble(vv[i], out var value))
                return Binding.DoNothing;
            if (value > first_value)
                return false;
        }

        return true;
    }
}