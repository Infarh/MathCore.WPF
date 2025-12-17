using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.IO;

/// <summary>Возвращает имя файла из полного пути</summary>
[MarkupExtensionReturnType(typeof(FilePathToName))]
[ValueConversion(typeof(string), typeof(string))]
public class FilePathToName : ValueConverter
{
    /// <summary>Возвращает имя файла из полного пути или Binding.DoNothing при неподдерживаемом типе</summary>
    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) =>
        v is string str ? Path.GetFileName(str) : Binding.DoNothing;
}