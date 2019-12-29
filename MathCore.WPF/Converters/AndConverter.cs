using System;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(AndConverter))]
    public class AndConverter : MultiValueValueConverter
    {
        /// <inheritdoc />
        protected override object Convert(object[] vv, Type t, object p, CultureInfo c) => vv.Cast<bool>().All(v => v);
    }     
}