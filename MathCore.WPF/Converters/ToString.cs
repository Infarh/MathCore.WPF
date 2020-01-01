using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
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
            if(!converter.CanConvertFrom(typeof(string)))
                throw new NotSupportedException();
            return converter.ConvertFrom(str);
        }
    }
}