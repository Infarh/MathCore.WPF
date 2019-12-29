using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(InIntervalValue))]
    [ValueConversion(typeof(double), typeof(double))]
    public class InIntervalValue : DoubleValueConverter
    {
        private Interval _Interval = new Interval(double.NegativeInfinity, double.PositiveInfinity);

        public double Min { get => _Interval.Min; set => _Interval = _Interval.SetMin(value); }
        public bool MinInclude { get => _Interval.MinInclude; set => _Interval = _Interval.IncludeMin(value); }
        public double Max { get => _Interval.Max; set => _Interval = _Interval.SetMax(value); }
        public bool MaxInclude { get => _Interval.MaxInclude; set => _Interval = _Interval.IncludeMax(value); }

        public InIntervalValue() { }

        public InIntervalValue(double MinMax) : this(new Interval(-MinMax, MinMax)) { }
        public InIntervalValue(double min, double max) : this(new Interval(Math.Min(min, max), Math.Max(min, max))) { }

        public InIntervalValue(Interval interval) => _Interval = interval;

        /// <inheritdoc />
        protected override double Convert(double v, double? p = null)
        {
            var value = p ?? v;
            return value.IsNaN() ? double.NaN : _Interval.Normalize(value);
        }
    }
}