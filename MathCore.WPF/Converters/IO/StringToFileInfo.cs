using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.IO
{
    [MarkupExtensionReturnType(typeof(StringToFileInfo))]
    [ValueConversion(typeof(string), typeof(FileInfo))]
    public class StringToFileInfo : ValueConverter
    {
        /// <inheritdoc />
        protected override object? Convert(object? v, Type t, object? p, CultureInfo c) =>
            v switch
            {
                string s => new FileInfo(s),
                FileInfo f => f,
                { } obj when (obj.ToString() is { } str) => new FileInfo(str),
                _ => null
            };

        /// <inheritdoc />
        protected override object? ConvertBack(object? v, Type t, object? p, CultureInfo c) => 
            v is FileInfo file_info ? file_info.FullName : null;
    }
}