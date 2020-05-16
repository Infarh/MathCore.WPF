using System;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class Round : DoubleValueConverter
    {
        [ConstructorArgument(nameof(Digits))]
        public int Digits { get; set; }

        [ConstructorArgument(nameof(Rounding))]
        public MidpointRounding Rounding { get; set; }

        public Round() {}

        public Round(int Digits) => this.Digits = Digits;

        public Round(int Digits, MidpointRounding Rounding) : this(Digits) => this.Rounding = Rounding;

        /// <inheritdoc />
        protected override double Convert(double v, double? p = null) => Math.Round(v);

        /// <inheritdoc />
        protected override double ConvertBack(double v, double? p = null) => v;
    }
}