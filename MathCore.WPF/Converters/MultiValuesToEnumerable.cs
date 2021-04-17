using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(MultiValuesToEnumerable))]
    public class MultiValuesToEnumerable : MultiValueValueConverter
    {
        protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c) => vv;

        protected override object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) => 
            (v as IEnumerable)?.Cast<object>().Zip(tt!, System.Convert.ChangeType).ToArray()!;
    }
}