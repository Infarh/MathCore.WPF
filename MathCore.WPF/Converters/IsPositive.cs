using System;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(IsPositive))]
    [ValueConversion(typeof(double), typeof(bool?))]
    public class IsPositive : DoubleToBool
    {
        /// <inheritdoc />
        protected override bool? Convert(double v) => v.IsNaN() ? (bool?)null : v > 0;
    }
}