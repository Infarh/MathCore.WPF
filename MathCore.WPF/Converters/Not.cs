using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(Not))]
    [ValueConversion(typeof(bool), typeof(bool))]
    public class Not : ValueConverter
    {
        /// <inheritdoc />
        protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => !(bool?) v;

        /// <inheritdoc />
        protected override object? ConvertBack(object? v, Type? t, object? p, CultureInfo? c) => !(bool?)v;
    }
}