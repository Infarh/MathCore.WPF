using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(IsNegative))]
    [ValueConversion(typeof(double), typeof(bool?))]
    public class IsNegative : DoubleToBoolConverter
    {
        /// <inheritdoc />
        protected override bool? Convert(double v) => v.IsNaN() ? (bool?)null : v < 0;
    }
}