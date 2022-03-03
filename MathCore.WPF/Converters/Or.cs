using System;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(Or))]
public class Or : MultiValueValueConverter
{
    /// <inheritdoc />
    protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => vv?.Cast<bool>().Any(v => v);
}