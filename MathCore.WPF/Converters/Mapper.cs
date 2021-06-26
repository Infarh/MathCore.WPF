using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    [MarkupExtensionReturnType(typeof(Mapper))]
    public class Mapper : DoubleValueConverter
    {
        private double _k = 1;

        private double _MinScale;

        public double MinScale
        {
            get => _MinScale;
            set
            {
                _MinScale = value;
                _k = (_MaxScale - _MinScale) / (_MaxValue - _MinValue);
            }
        }

        private double _MaxScale = 1;

        public double MaxScale
        {
            get => _MaxScale;
            set
            {
                _MaxScale = value;
                _k = (_MaxScale - _MinScale) / (_MaxValue - _MinValue);
            }
        }

        private double _MinValue;

        public double MinValue
        {
            get => _MinValue;
            set
            {
                _MinValue = value;
                _k = (_MaxScale - _MinScale) / (_MaxValue - _MinValue);
            }
        }

        private double _MaxValue = 1;

        public double MaxValue
        {
            get => _MaxValue;
            set
            {
                _MaxValue = value;
                _k = (_MaxScale - _MinScale) / (_MaxValue - _MinValue);
            }
        }

        public Mapper() { }

        /// <inheritdoc />
        protected override double Convert(double v, double? p = null)
        {
            var x = (p ?? v);
            var result = (x - _MinValue) * _k + _MinScale;
            return result;
        }

        /// <inheritdoc />
        protected override double ConvertBack(double x, double? p = null) => (x - _MinScale) / _k + _MinValue;
    }
}