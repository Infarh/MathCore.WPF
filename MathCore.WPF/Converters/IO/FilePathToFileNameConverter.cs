using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters.IO
{
    [MarkupExtensionReturnType(typeof(FilePathToFileNameConverter))]
    [ValueConversion(typeof(string), typeof(string))]
    public class FilePathToFileNameConverter : ValueConverter
    {
        /// <inheritdoc />
        protected override object Convert(object v, Type t, object p, CultureInfo c) => Path.GetFileName((string)v);
    }
}
