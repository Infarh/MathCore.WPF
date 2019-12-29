using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(OutRange))]
    [ValueConversion(typeof(double), typeof(bool?))]
    public class OutRange : DoubleToBoolConverter
    {
        private Interval _Interval = new Interval(0, 0);

        public double Min { get => _Interval.Min; set => _Interval = _Interval.SetMin(value); }
        public bool MinInclude { get => _Interval.MinInclude; set => _Interval = _Interval.IncludeMin(value); }
        public double Max { get => _Interval.Max; set => _Interval = _Interval.SetMax(value); }
        public bool MaxInclude { get => _Interval.MaxInclude; set => _Interval = _Interval.IncludeMax(value); }

        public bool? IncludeLimits
        {
            get => MinInclude && MaxInclude ? true : !MinInclude && !MaxInclude ? (bool?)false : null;
            set
            {
                switch (value)
                {
                    case true:
                        MinInclude = true;
                        MaxInclude = true;
                        break;
                    case false:
                        MinInclude = false;
                        MaxInclude = false;
                        break;
                }
            }
        }

        public OutRange() { }

        public OutRange(double MinMax) : this(new Interval(-MinMax, MinMax)) { }
        public OutRange(double min, double max) : this(new Interval(Math.Min(min, max), Math.Max(min, max))) { }

        public OutRange(Interval interval) => _Interval = interval;

        /// <inheritdoc />
        protected override bool? Convert(double v) => v.IsNaN() ? null : (bool?)!_Interval.Check(v);
    }
}