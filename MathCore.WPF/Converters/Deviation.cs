using System.Windows.Data;

namespace MathCore.WPF.Converters
{
    /// <inheritdoc />
    [ValueConversion(typeof(double), typeof(double))]
    public class Deviation : DoubleValueConverter
    {
        private double _LastValue = double.NaN;

        protected override double Convert(double v, double? p = null)
        {
            var deviation = v - _LastValue;
            _LastValue = v;
            return deviation;
        }
    }
}