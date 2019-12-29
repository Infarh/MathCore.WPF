using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(LessThenOrEqual))]
    [ValueConversion(typeof(double), typeof(bool?))]
    public class LessThenOrEqual : DoubleToBoolConverter
    {
        public double Value { get; set; } = double.PositiveInfinity;

        public LessThenOrEqual() { }

        public LessThenOrEqual(double value) => Value = value;

        protected override bool? Convert(double v) => v.IsNaN() ? null : (bool?)(v <= Value);
    }
}