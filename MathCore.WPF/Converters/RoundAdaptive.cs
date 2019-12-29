using System;
using System.Windows.Data;

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class RoundAdaptive : DoubleValueConverter
    {
        public int Digits { get; set; }

        public MidpointRounding Rounding { get; set; }

        public RoundAdaptive() {}
        public RoundAdaptive(int Digits) => this.Digits = Digits;
        public RoundAdaptive(int Digits, MidpointRounding Rounding) : this(Digits) => this.Rounding = Rounding;

        /// <inheritdoc />
        protected override double Convert(double v, double? p = null) => v.RoundAdaptive(Digits);

        /// <inheritdoc />
        protected override double ConvertBack(double v, double? p = null) => v;
    }
}