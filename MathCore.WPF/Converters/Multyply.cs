using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    /// <summary>Преобразователь умножения значения на вещественное число</summary>
    [MarkupExtensionReturnType(typeof(Multipy))]
    public class Multipy : SimpleDoubleValueConverter
    {
        public Multipy() : this(1) { }
        public Multipy(double K) : base(K, (v, k) => v * k, (r, k) => r / k) { }
    }
}