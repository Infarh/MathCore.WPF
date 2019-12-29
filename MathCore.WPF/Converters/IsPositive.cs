using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(IsPositive))]
    [ValueConversion(typeof(double), typeof(bool?))]
    public class IsPositive : DoubleToBoolConverter
    {
        /// <inheritdoc />
        protected override bool? Convert(double v) => v.IsNaN() ? (bool?)null : v > 0;
    }
}