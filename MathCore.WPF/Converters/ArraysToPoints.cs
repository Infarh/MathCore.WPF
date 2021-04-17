using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    public class ArraysToPoints : MultiValueValueConverter
    {
        [NotNull]
        protected override object? Convert(object[]? vv, Type? t, object? p, CultureInfo? c)
        {
            if(vv is null) throw new ArgumentNullException(nameof(vv));
            if(vv.Length != 2) throw new ArgumentOutOfRangeException(nameof(vv));

            if(vv[0] is not IEnumerable) throw new InvalidCastException("Argument #0 is not IEnumerable<object>");
            if(vv[1] is not IEnumerable) throw new InvalidCastException("Argument #1 is not IEnumerable<object>");

            var e1 = (IEnumerable)vv[0];
            var e2 = (IEnumerable)vv[1];
            Func<object, object>? f1 = null;
            Func<object, object>? f2 = null;
            Point Func(object x, object y) => 
                new(
                    x: (double)(f1 ??= typeof(double).GetCasterFrom(x.GetType()))(x), 
                    y: (double)(f2 ??= typeof(double).GetCasterFrom(y.GetType()))(y));
            return new PointCollection(e1.Cast<object>().Zip(e2.Cast<object>(), Func));
        }
    }
}