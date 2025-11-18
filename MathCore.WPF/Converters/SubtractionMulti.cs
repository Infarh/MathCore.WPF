using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(SubtractionMulti))]
public class SubtractionMulti : MultiValueValueConverter
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

        if (!DoubleValueConverter.TryConvertToDouble(vv[0], c, out var value))
            return double.NaN;
        var v = value;

        for (var i = 1; i < vv.Length; i++)
        {
            if (vv[i] is null) return double.NaN;
            if (!DoubleValueConverter.TryConvertToDouble(vv[i], c, out value))
                return double.NaN;
            v -= value;
        }

        return v;
    }
}