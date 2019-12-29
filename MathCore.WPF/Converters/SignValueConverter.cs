using System;
using System.Windows.Data;

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class SignValueConverter : DoubleValueConverter
    {
        public double Delta { get; set; }

        public bool Inverse { get; set; }

        public SignValueConverter() { }
        public SignValueConverter(double Delta) => this.Delta = Delta;
        public SignValueConverter(bool Inverse) => this.Inverse = Inverse;

        /// <inheritdoc />
        protected override double Convert(double v, double? p = null) => double.IsNaN(v) ? v : (Math.Abs(v) <= Delta ? 0d : (Inverse ? -Math.Sign(v) : Math.Sign(v)));
    }
}