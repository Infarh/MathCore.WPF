using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(GreateThen))]
    [ValueConversion(typeof(double), typeof(bool?))]
    public class GreateThen : DoubleToBoolConverter
    {
        public double Value { get; set; } = double.NegativeInfinity;

        public GreateThen() { }

        public GreateThen(double value) => Value = value;

        protected override bool? Convert(double v) => v.IsNaN() ? null : (bool?)(v > Value);
    }
}