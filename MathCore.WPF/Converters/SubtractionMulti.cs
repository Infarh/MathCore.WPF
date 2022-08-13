﻿using System.Globalization;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(SubtractionMulti))]
public class SubtractionMulti : MultiValueValueConverter
{
    protected override object? Convert(object?[]? vv, Type? t, object? p, CultureInfo? c)
    {
        if (vv is null) return null;
        if (vv.Length == 1 && vv[0] is null) return double.NaN;

        var v = vv[0] is double d ? d : System.Convert.ToDouble(vv[0]);

        for (var i = 1; i < vv.Length; i++)
        {
            if (vv[i] is null) return double.NaN;
            v -= vv[i] is double dv ? dv : System.Convert.ToDouble(vv[i]);
        }

        return v;
    }
}