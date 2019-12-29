using System;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(OrConverter))]
    public class OrConverter : MultiValueValueConverter
    {
        /// <inheritdoc />
        protected override object Convert(object[] vv, Type t, object p, CultureInfo c) => vv.Cast<bool>().Any(v => v);
    }
}