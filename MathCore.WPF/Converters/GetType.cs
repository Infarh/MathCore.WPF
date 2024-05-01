using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(object), typeof(Type))]
[MarkupExtensionReturnType(typeof(GetType))]
public class GetType : ValueConverter
{
    protected override object? Convert(object? v, Type? t, object? p, CultureInfo? c) => v?.GetType();
}