using System;
using System.Linq;
using System.Windows.Media;

namespace MathCore.WPF.Converters
{
    public class CSplineInterp : DoubleValueConverter
    {
        private MathCore.Interpolation.CubicSpline _SplineTo;
        private MathCore.Interpolation.CubicSpline _SplineFrom;
        private double min_x;
        private double min_y;
        private double max_x;
        private double max_y;

        public PointCollection Points { get; set; }

        public CSplineInterp() { }

        public CSplineInterp(PointCollection points) { Points = points; }

        public override object ProvideValue(IServiceProvider sp)
        {
            if(Points == null || Points.Count == 0) throw new FormatException();

            var x = Points.Select(p => p.X).ToArray();
            var y = Points.Select(p => p.Y).ToArray();
            x.GetMinMax(v => v, out min_x, out max_x);
            y.GetMinMax(v => v, out min_y, out max_y);
            _SplineTo = new MathCore.Interpolation.CubicSpline(x, y);
            _SplineFrom = new MathCore.Interpolation.CubicSpline(y, x);

            return base.ProvideValue(sp);
        }

        /// <inheritdoc />
        protected override double Convert(double v, double? p = null)
        {
            if(v < min_x) v = min_x;
            else if(v > max_x) v = max_x;
            return _SplineTo.Value(v);
        }

        /// <inheritdoc />
        protected override double ConvertBack(double v, double? p = null)
        {
            if(v < min_y) v = min_y;
            else if(v > max_y) v = max_y;
            return _SplineFrom.Value(v);
        }
    }
}