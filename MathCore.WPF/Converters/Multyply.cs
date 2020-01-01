using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    /// <summary>Преобразователь умножения значения на вещественное число</summary>
    [MarkupExtensionReturnType(typeof(Multiply))]
    public class Multiply : SimpleDoubleValueConverter
    {
        public Multiply() : this(1) { }

        public Multiply(double K) : base(K, (v, k) => v * k, (r, k) => r / k) { }
    }
}