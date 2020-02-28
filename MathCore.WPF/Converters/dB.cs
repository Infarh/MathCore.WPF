using System;
using System.Windows.Markup;
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(dB))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Стиль", "IDE1006:Стили именования", Justification = "<Ожидание>")]
    // ReSharper disable once InconsistentNaming
    public class dB : DoubleValueConverter
    {
        public bool ByPower { get; set; }

        public bool Invert { get; set; }

        /// <inheritdoc />
        protected override double Convert(double v, double? p = null) => Invert
            ? double.IsNaN(v) ? v : ByPower ? Math.Pow(10, v / 10) : Math.Pow(10, v / 20)
            : double.IsNaN(v) ? v : ByPower ? 10 * Math.Log10(v) : 20 * Math.Log10(v);

        /// <inheritdoc />
        protected override double ConvertBack(double v, double? p = null) => Invert 
            ? double.IsNaN(v) ? v : ByPower ? 10 * Math.Log10(v) : 20 * Math.Log10(v)
            : double.IsNaN(v) ? v : ByPower ? Math.Pow(10, v / 10) : Math.Pow(10, v / 20);
    }
}