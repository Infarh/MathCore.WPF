using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.IO
{
    [MarkupExtensionReturnType(typeof(StringToFileInfo))]
    [ValueConversion(typeof(string), typeof(FileInfo))]
    public class StringToFileInfo : ValueConverter
    {
        /// <inheritdoc />
        [CanBeNull]
        protected override object Convert([CanBeNull] object v, Type t, object p, CultureInfo c) => v is null ? null! : new FileInfo(v.ToString());

        /// <inheritdoc />
        [CanBeNull]
        protected override object ConvertBack(object? v, Type t, object p, CultureInfo c) => v is FileInfo file_info ? file_info.FullName : null!;
    }
}