using System;
using System.Globalization;
using System.Linq;
using MathCore.Annotations;

namespace MathCore.WPF.Converters
{
    public abstract class MultiDoubleValueValueConverter : MultiValueValueConverter
    {
        /// <inheritdoc />
        [NotNull]
        protected override object? Convert([CanBeNull] object[]? vv, Type? t, object? p, CultureInfo? c) => 
            Convert(Array.ConvertAll(vv ?? Array.Empty<object>(), DoubleValueConverter.ConvertToDouble));

        /// <inheritdoc />
        [CanBeNull]
        protected override object[]? ConvertBack(object? v, Type[]? tt, object? p, CultureInfo? c) =>
            ConvertBack(DoubleValueConverter.ConvertToDouble(v))?.Cast<object>().ToArray();

        protected abstract double Convert(double[]? vv);

        [CanBeNull]
        protected virtual double[]? ConvertBack(double v)
        {
            base.ConvertBack(null, null, null, null);
            return null;
        }
    }
}