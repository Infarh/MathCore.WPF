using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(ToString))]
[ValueConversion(typeof(object), typeof(string))]
public class ToString : ValueConverter
{
    /// <inheritdoc />
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => v?.ToString();

    protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c)
    {
        var str = v as string;
        if(string.IsNullOrWhiteSpace(str)) return null;
        var converter = TypeDescriptor.GetConverter(t);
        return converter.CanConvertFrom(typeof(string)) 
            ? converter.ConvertFrom(str) 
            : throw new NotSupportedException();
    }
}