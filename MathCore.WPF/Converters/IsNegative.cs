using System;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(IsNegative))]
    [ValueConversion(typeof(double), typeof(bool?))]
    public class IsNegative : DoubleToBool
    {
        /// <inheritdoc />
        protected override bool? Convert(double v) => v.IsNaN() ? (bool?)null : v < 0;
    }
}