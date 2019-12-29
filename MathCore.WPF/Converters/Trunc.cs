using System;
using System.Windows.Data;

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class Trunc : DoubleValueConverter
    {
        /// <inheritdoc />
        protected override double Convert(double v, double? p = null) => Math.Truncate(v);

        /// <inheritdoc />
        protected override double ConvertBack(double v, double? p = null) => v;
    }
}