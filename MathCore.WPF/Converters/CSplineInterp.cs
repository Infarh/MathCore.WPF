using System;
using System.Linq;
using System.Windows.Media;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    // ReSharper disable once IdentifierTypo
    public class CSplineInterp : DoubleValueConverter
    {
        private MathCore.Interpolation.CubicSpline? _SplineTo;
        private MathCore.Interpolation.CubicSpline? _SplineFrom;
        private double _MinX;
        private double _MinY;
        private double _MaxX;
        private double _MaxY;

        public PointCollection Points { get; set; }

        public CSplineInterp() => Points = new PointCollection();

        public CSplineInterp(PointCollection points) => Points = points;

        public override object ProvideValue(IServiceProvider sp)
        {
            if(Points is null || Points.Count == 0) throw new FormatException();

            var x = Points.Select(p => p.X).ToArray();
            var y = Points.Select(p => p.Y).ToArray();
            x.GetMinMax(v => v, out _MinX, out _MaxX);
            y.GetMinMax(v => v, out _MinY, out _MaxY);
            _SplineTo = new MathCore.Interpolation.CubicSpline(x, y);
            _SplineFrom = new MathCore.Interpolation.CubicSpline(y, x);

            return base.ProvideValue(sp);
        }

        /// <inheritdoc />
        protected override double Convert(double v, double? p = null)
        {
            if(v < _MinX) v = _MinX;
            else if(v > _MaxX) v = _MaxX;
            return _SplineTo!.Value(v);
        }

        /// <inheritdoc />
        protected override double ConvertBack(double v, double? p = null)
        {
            if(v < _MinY) v = _MinY;
            else if(v > _MaxY) v = _MaxY;
            return _SplineFrom!.Value(v);
        }
    }
}