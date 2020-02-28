using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.StringConverters
{
    [MarkupExtensionReturnType(typeof(ToLower)), ValueConversion(typeof(string), typeof(string))]
    public class ToLower : ValueConverter
    {
        [CanBeNull] protected override object Convert(object v, Type t, object p, CultureInfo c) => (v as string)?.ToLower(c)!;
    }
}
