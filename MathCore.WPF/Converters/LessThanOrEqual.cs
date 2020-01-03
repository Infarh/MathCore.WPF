using System;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(LessThanOrEqual))]
    [ValueConversion(typeof(double), typeof(bool?))]
    public class LessThanOrEqual : DoubleToBool
    {
        public double Value { get; set; } = double.PositiveInfinity;

        public LessThanOrEqual() { }

        public LessThanOrEqual(double value) => Value = value;

        protected override bool? Convert(double v) => v.IsNaN() ? null : (bool?)(v <= Value);
    }
}