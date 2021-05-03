using System;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(GreaterThanOrEqual))]
    [ValueConversion(typeof(double), typeof(bool?))]
    public class GreaterThanOrEqual : DoubleToBool
    {
        public double Value { get; set; } = double.NegativeInfinity;

        public GreaterThanOrEqual() { }

        public GreaterThanOrEqual(double value) => Value = value;

        protected override bool? Convert(double v) => v.IsNaN() ? null : v >= Value;
    }
}