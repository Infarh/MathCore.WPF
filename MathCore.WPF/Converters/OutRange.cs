using System;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(OutRange))]
    [ValueConversion(typeof(double), typeof(bool?))]
    public class OutRange : DoubleToBool
    {
        private Interval _Interval = new(0, 0);

        [ConstructorArgument(nameof(Min))]
        public double Min { get => _Interval.Min; set => _Interval = _Interval.SetMin(value); }

        [ConstructorArgument(nameof(Max))]
        public double Max { get => _Interval.Max; set => _Interval = _Interval.SetMax(value); }

        public bool MinInclude { get => _Interval.MinInclude; set => _Interval = _Interval.IncludeMin(value); }

        public bool MaxInclude { get => _Interval.MaxInclude; set => _Interval = _Interval.IncludeMax(value); }

        public bool? IncludeLimits
        {
            get => MinInclude && MaxInclude ? true : !MinInclude && !MaxInclude ? false : null;
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

        public OutRange(double Min, double Max) : this(new Interval(Math.Min(Min, Max), Math.Max(Min, Max))) { }

        public OutRange(Interval interval) => _Interval = interval;

        /// <inheritdoc />
        protected override bool? Convert(double v) => v.IsNaN() ? null : !_Interval.Check(v);
    }
}