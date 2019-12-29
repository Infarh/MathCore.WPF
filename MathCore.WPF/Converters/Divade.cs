using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    /// <summary>Преобразователь деления значения на вещественное число</summary>
    [MarkupExtensionReturnType(typeof(Divade))]
    public class Divade : SimpleDoubleValueConverter
    {
        public Divade() : this(1) { }
        public Divade(double K) : base(K, (v, k) => v / k, (r, k) => r * k) { }
    }
}