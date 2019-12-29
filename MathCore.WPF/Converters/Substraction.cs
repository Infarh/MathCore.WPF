using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    /// <summary>Преобразователь вычитания вещественного числа из значения</summary>
    [MarkupExtensionReturnType(typeof(Substraction))]
    public class Substraction : SimpleDoubleValueConverter
    {
        public Substraction() : this(0) { }
        public Substraction(double P) : base(P, (v, p) => v - p, (r, p) => r + p) { }
    }
}