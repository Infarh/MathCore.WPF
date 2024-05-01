using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(ArraysToPoints))]
public class ArraysToPoints : MultiValueValueConverter
{
    protected override object Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
    {
        if (vv is not [IEnumerable e1, IEnumerable e2])
            throw new ArgumentException("Параметр конвертера должен быть массивом из двух ячеек с перечислениями IEnumerable", nameof(vv));

        Func<object, object>? f1 = null;
        Func<object, object>? f2 = null;
        return new PointCollection(e1.Cast<object>().Zip(e2.Cast<object>(), Func));

        Point Func(object x, object y) => 
            new(
                x: (double)(f1 ??= typeof(double).GetCasterFrom(x.GetType()))(x), 
                y: (double)(f2 ??= typeof(double).GetCasterFrom(y.GetType()))(y));
    }
}