using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[ValueConversion(typeof(double), typeof(double))]
[MarkupExtensionReturnType(typeof(Abs))]
public class Abs : DoubleValueConverter
{
    /// <inheritdoc />
    protected override double Convert(double v, double? p = null) => Math.Abs(v);

    /// <inheritdoc />
    protected override double ConvertBack(double v, double? p = null) => v;
}