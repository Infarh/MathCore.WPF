using System;
using System.Windows.Data;
using System.Windows.Markup;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    [MarkupExtensionReturnType(typeof(InRange))]
    [ValueConversion(typeof(double), typeof(bool?))]
    public class InRange : DoubleToBool
    {
        private Interval _Interval = new(double.NegativeInfinity, double.PositiveInfinity);

        [ConstructorArgument(nameof(Min))]
        public double Min { get => _Interval.Min; set => _Interval = _Interval.SetMin(value); }

        [ConstructorArgument(nameof(Max))]
        public double Max { get => _Interval.Max; set => _Interval = _Interval.SetMax(value); }

        public bool MinInclude { get => _Interval.MinInclude; set => _Interval = _Interval.IncludeMin(value); }

        public bool MaxInclude { get => _Interval.MaxInclude; set => _Interval = _Interval.IncludeMax(value); }

        public InRange() { }

        public InRange(double MinMax) : this(new Interval(-MinMax, MinMax)) { }

        public InRange(double min, double max) : this(new Interval(Math.Min(min, max), Math.Max(min, max))) { }

        public InRange(Interval interval) => _Interval = interval;

        /// <inheritdoc />
        protected override bool? Convert(double v) => v.IsNaN() ? null : _Interval.Check(v);
    }
}