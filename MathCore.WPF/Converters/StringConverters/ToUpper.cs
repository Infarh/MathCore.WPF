﻿using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.StringConverters;

[MarkupExtensionReturnType(typeof(ToUpper)), ValueConversion(typeof(string), typeof(string))]
public class ToUpper : ValueConverter
{
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => 
        (v as string ?? v?.ToString())?.ToUpper();
}