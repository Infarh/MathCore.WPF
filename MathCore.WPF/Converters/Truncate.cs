using System.Windows.Data;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(double), typeof(double))]
public class Truncate : DoubleValueConverter
{
    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) => Math.Truncate(v);

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) => v;
}