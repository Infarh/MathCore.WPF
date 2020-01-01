using System.Windows.Markup;

namespace MathCore.WPF.Converters
{
    /// <summary>Преобразователь деления значения на вещественное число</summary>
    [MarkupExtensionReturnType(typeof(Divide))]
    // ReSharper disable once UnusedType.Global
    public class Divide : SimpleDoubleValueConverter
    {
        public Divide() : this(1) { }

        public Divide(double K) : base(K, (v, k) => v / k, (r, k) => r * k) { }
    }
}