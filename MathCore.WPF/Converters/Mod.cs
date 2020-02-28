using System;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    [MarkupExtensionReturnType(typeof(Mod))]
    public class Mod : DoubleValueConverter
    {
        public double M { get; set; }

        public Mod() : this(double.NaN) { }

        public Mod(double M) => this.M = M;

        /// <inheritdoc />
        protected override double Convert(double v, double? p = null) => 
            (p ?? v).IsNaN() 
                ? double.NaN 
                : M.IsNaN() 
                    ? p ?? v 
                    : (p ?? v) % M;
    }
}