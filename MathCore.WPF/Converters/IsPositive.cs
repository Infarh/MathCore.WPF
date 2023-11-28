using System.Windows.Data;
using System.Windows.Markup;

using MathCore.WPF.Converters.Base;

// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters;

[MarkupExtensionReturnType(typeof(IsPositive))]
[ValueConversion(typeof(double), typeof(bool?))]
public class IsPositive : DoubleToBool
{
    /// <inheritdoc />
    protected override bool? Convert(double v) => v is double.NaN ? null : v > 0;
}