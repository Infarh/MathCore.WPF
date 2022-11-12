using System.Globalization;
using System.Windows.Data;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(object), typeof(Type))]
public class GetType : ValueConverter
{
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => v?.GetType();
}