using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.IO;


/// <summary>Конвертер преобразования строки пути в объект FileInfo</summary>
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