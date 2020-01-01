using System;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(Or))]
    public class Or : MultiValueValueConverter
    {
        /// <inheritdoc />
        [CanBeNull]
        protected override object? Convert([CanBeNull] object[]? vv, Type? t, object? p, CultureInfo? c) => vv?.Cast<bool>().Any(v => v);
    }
}