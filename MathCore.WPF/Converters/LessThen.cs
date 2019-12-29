using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(LessThen))]
    [ValueConversion(typeof(double), typeof(bool?))]
    public class LessThen : DoubleToBoolConverter
    {
        public double Value { get; set; } = double.PositiveInfinity;

        public LessThen() { }

        public LessThen(double value) => Value = value;

        protected override bool? Convert(double v) => v.IsNaN() ? null : (bool?)(v < Value);
    }
}