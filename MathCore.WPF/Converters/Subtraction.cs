using System.Windows.Markup;
// ReSharper disable UnusedType.Global

namespace MathCore.WPF.Converters
{
    /// <summary>Преобразователь вычитания вещественного числа из значения</summary>
    [MarkupExtensionReturnType(typeof(Subtraction))]
    public class Subtraction : SimpleDoubleValueConverter
    {
        public Subtraction() : this(0) { }

        public Subtraction(double P) : base(P, (v, p) => v - p, (r, p) => r + p) { }
    }
}