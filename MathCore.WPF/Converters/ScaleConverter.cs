using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    [MarkupExtensionReturnType(typeof(ScaleConverter))]
    public class ScaleConverter : DoubleValueConverter
    {
        public double MinScale { get; set; }

        public double MaxScale { get; set; } = 1;

        public double MinValue { get; set; }

        public double MaxValue { get; set; } = 1;

        public ScaleConverter() { }

        /// <inheritdoc />
        protected override double Convert(double v, double? p = null) => (p ?? v) * (MaxScale - MinScale) / (MaxValue - MinValue) + MinScale;

        /// <inheritdoc />
        protected override double ConvertBack(double v, double? p = null) => (v - MinScale) * (MaxValue - MinValue) / (MaxScale - MinScale);
    }
}