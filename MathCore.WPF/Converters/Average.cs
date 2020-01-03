using MathCore.Values;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.WPF.Converters
{
    public class Average : SimpleDoubleValueConverter
    {
        private readonly AverageValue _Value;

        public int Length
        {
            get => _Value.Length;
            set => _Value.Length = value;
        }

        public Average() => _Value = new AverageValue(0);

        public Average(int Length) => _Value = new AverageValue(Length);

        /// <inheritdoc />
        protected override double To(double v, double p)
        {
            if (!double.IsNaN(v)) return _Value.AddValue(v);
            _Value.Reset();
            return v;
        }
    }
}