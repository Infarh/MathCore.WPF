using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters.IO;

[MarkupExtensionReturnType(typeof(FilePathToName))]
[ValueConversion(typeof(string), typeof(string))]
public class FilePathToName : ValueConverter
{
    /// <inheritdoc />
    protected override object? Convert(object? v, Type t, object? p, CultureInfo c) =>
        v is string str ? Path.GetFileName(str) : null;
}