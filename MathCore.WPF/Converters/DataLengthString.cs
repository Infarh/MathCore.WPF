using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.Data;
using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

/// <summary>Преобразует числовое значение в объект DataLength</summary>
[ValueConversion(typeof(double), typeof(DataLength))]
[MarkupExtensionReturnType(typeof(DataLengthString))]
// ReSharper disable once UnusedMember.Global
public sealed class DataLengthString : ValueConverter
{
    /// <summary>Преобразует числовое значение (в байтах) в DataLength с основанием 1024</summary>
    protected override object Convert(object? v, Type? t, object? p, CultureInfo? c) =>
        DoubleValueConverter.TryConvertToDouble(v, c, out var value)
            ? new DataLength(value, 1024d)
            : Binding.DoNothing;
}