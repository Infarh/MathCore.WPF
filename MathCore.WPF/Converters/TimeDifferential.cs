using System;
using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
	/// <summary>Конвертер дифференцирования значения по времени</summary>
	[MarkupExtensionReturnType(typeof(TimeDifferential))]
	public class TimeDifferential : SimpleDoubleValueConverter
	{
		private DateTime _LastTime = DateTime.Now;
		private double _LastValue = double.NaN;
		public bool IgnoreNaN { get; set; }

		public TimeDifferential() : this(1) { }
		public TimeDifferential(double K, bool ignore_nan = false) : base(K) => IgnoreNaN = ignore_nan;

		/// <inheritdoc />
		protected override double To(double v, double p)
		{
			if (double.IsNaN(v)) return IgnoreNaN ? v : _LastValue = v;

			var now = DateTime.Now;
			var dt = (now - _LastTime).TotalSeconds;
			_LastTime = now;
			if (double.IsNaN(_LastValue))
			{
				if (!IgnoreNaN) _LastValue = v;
				return double.NaN;
			}

			var dv = v - _LastValue;
			_LastValue = v;
			return dv / dt * Parameter;
		}
	}
}