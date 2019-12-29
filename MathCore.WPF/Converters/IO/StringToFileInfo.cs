using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters.IO
{
    [MarkupExtensionReturnType(typeof(StringToFileInfo))]
    [ValueConversion(typeof(string), typeof(FileInfo))]
    public class StringToFileInfo : ValueConverter
    {
        /// <inheritdoc />
        protected override object Convert(object v, Type t, object p, CultureInfo c) => new FileInfo((string)v);

        /// <inheritdoc />
        protected override object ConvertBack(object v, Type t, object p, CultureInfo c) => ((FileInfo) v).FullName;
    }
}