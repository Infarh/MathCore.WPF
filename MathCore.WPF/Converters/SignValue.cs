using System;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class SignValue : DoubleValueConverter
    {
        [ConstructorArgument(nameof(Delta))]
        public double Delta { get; set; }

        [ConstructorArgument(nameof(Inverse))]
        public bool Inverse { get; set; }

        public SignValue() { }

        public SignValue(double Delta) => this.Delta = Delta;

        public SignValue(bool Inverse) => this.Inverse = Inverse;

        protected override double Convert(double v, double? p = null) => 
            double.IsNaN(v) 
                ? double.NaN 
                : Math.Abs(v) <= Delta 
                    ? 0 
                    : Inverse 
                        ? -Math.Sign(v) 
                        : Math.Sign(v);
    }
}