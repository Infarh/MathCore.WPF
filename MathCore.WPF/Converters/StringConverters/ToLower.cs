using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters.StringConverters
{
    [MarkupExtensionReturnType(typeof(ToLower)), ValueConversion(typeof(string), typeof(string))]
    public class ToLower : ValueConverter
    {
        protected override object Convert(object v, Type t, object p, CultureInfo c) => ((string)v).ToLower(c);
    }
}
