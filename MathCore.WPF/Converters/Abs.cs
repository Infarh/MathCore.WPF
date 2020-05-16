using System;
using System.Windows.Data;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class Abs : DoubleValueConverter
    {
        /// <inheritdoc />
        protected override double Convert(double v, double? p = null) => Math.Abs(v);

        /// <inheritdoc />
        protected override double ConvertBack(double v, double? p = null) => v;
    }
}