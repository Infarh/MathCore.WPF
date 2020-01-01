using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.WPF.Converters
{
    /// <summary>Линейный конвертер вещественных величин по формуле result = K*value + B</summary>
    [ValueConversion(typeof(double), typeof(double))]
    [MarkupExtensionReturnType(typeof(Linear))]
    public class Linear : DoubleValueConverter
    {
        /// <summary>Линейный множитель (тангенс угла наклона)</summary>
        public double K { get; set; }

        /// <summary>Аддитивное смещение</summary>
        public double B { get; set; }

        public Linear() : this(1, 0) { }

        public Linear(double k) => K = k;

        public Linear(double k, double b ) : this(k) => B = b;


        /// <inheritdoc />
        protected override double Convert(double v, double? p = null) => (p ?? v) * K + B;

        /// <inheritdoc />
        protected override double ConvertBack(double v, double? p = null) => (v - B) / K;
    }
}