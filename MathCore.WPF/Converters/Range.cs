using System;

namespace MathCore.WPF.Converters
{
	public class Range : DoubleValueConverter
	{
		private Interval _Interval = new Interval(double.NegativeInfinity, double.PositiveInfinity);

		public double Min { get => _Interval.Min; set => _Interval = _Interval.SetMin(value); }
		public bool MinInclude { get => _Interval.MinInclude; set => _Interval = _Interval.IncludeMin(value); }
		public double Max { get => _Interval.Max; set => _Interval = _Interval.SetMax(value); }
		public bool MaxInclude { get => _Interval.MaxInclude; set => _Interval = _Interval.IncludeMax(value); }

		public Range() { }

		public Range(double MinMax) : this(new Interval(-MinMax, MinMax)) { }
		public Range(double min, double max) : this(new Interval(Math.Min(min, max), Math.Max(min, max))) { }
		public Range(Interval interval) => _Interval = interval;

		/// <inheritdoc />
		protected override double Convert(double v, double? p = null) => _Interval.Normalize(v);
	}
}